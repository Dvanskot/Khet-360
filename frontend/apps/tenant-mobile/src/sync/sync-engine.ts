import { db } from '../db/schema';
import { SyncCommand, SyncEvent } from '../db/schema';
import axios from 'axios';

export class SyncEngine {
  private static instance: SyncEngine;
  private isSyncing = false;

  private constructor() {}

  public static getInstance(): SyncEngine {
    if (!SyncEngine.instance) {
      SyncEngine.instance = new SyncEngine();
    }
    return SyncEngine.instance;
  }

  /**
   * The main sync loop: Push Outbox -> Pull Inbox -> Apply Local
   */
  public async sync(): Promise<void> {
    if (this.isSyncing) return;
    this.isSyncing = true;

    try {
      await this.pushOutbox();
      await this.pullInbox();
    } catch (error) {
      console.error('Sync failed:', error);
    } finally {
      this.isSyncing = false;
    }
  }

  /**
   * Pushes all pending commands from the local outbox to the server.
   * Uses ClientTransactionId for server-side idempotency.
   */
  private async pushOutbox(): Promise<void> {
    const pending = await db.outbox
      .where('status')
      .equals('Pending')
      .toArray();

    for (const command of pending) {
      try {
        // In a real app, this would be a generic /sync/push endpoint
        await axios.post('/tenant/api/sync/push', command);

        await db.outbox.update(command.id, { status: 'Synced' });
      } catch (error: any) {
        console.error(`Failed to sync command ${command.id}:`, error);
        await db.outbox.update(command.id, {
          status: 'Failed',
          errorMessage: error.message
        });
      }
    }
  }

  /**
   * Pulls latest events from the server and applies them to local state.
   */
  private async pullInbox(): Promise<void> {
    try {
      const response = await axios.get('/tenant/api/sync/pull');
      const events: SyncEvent[] = response.data;

      for (const event of events) {
        await db.inbox.put(event);
        await this.applyEvent(event);
        await db.inbox.update(event.id, { processed: true });
      }
    } catch (error) {
      console.error('Failed to pull inbox:', error);
    }
  }

  /**
   * Applies a server event to the local IndexedDB state.
   * Uses a simple Last-Write-Wins (LWW) strategy based on timestamp.
   */
  private async applyEvent(event: SyncEvent): Promise<void> {
    const table = this.getTableName(event.entityType);
    if (!table) return;

    const existing = await db[table].get(event.entityId);

    if (!existing || (existing as any).updatedAt < event.timestamp) {
      await db[table].put({
        ...event.payload,
        id: event.entityId,
        updatedAt: event.timestamp,
      });
    }
  }

  private getTableName(entityType: string): string | null {
    const map: Record<string, string> = {
      'Task': 'tasks',
      'Trip': 'trips',
      'Custody': 'custody',
      'ProductionOrder': 'production',
    };
    return map[entityType] || null;
  }

  /**
   * Wrapper for all mutations to ensure they are queued locally first.
   */
  public async executeCommand(command: Omit<SyncCommand, 'id' | 'timestamp' | 'status' | 'version>): Promise<void> {
    const clientTxId = crypto.randomUUID();
    const timestamp = new Date().toISOString();

    const fullCommand: SyncCommand = {
      ...command,
      id: clientTxId,
      timestamp,
      version: 1,
      status: 'Pending',
    };

    // 1. Save to Outbox
    await db.outbox.put(fullCommand);

    // 2. Apply locally immediately (Optimistic UI)
    const table = this.getTableName(command.entityType);
    if (table) {
      await db[table].put({
        ...command.payload,
        id: command.entityId,
        updatedAt: timestamp,
      });
    }

    // 3. Trigger background sync
    this.sync();
  }
}

export const syncEngine = SyncEngine.getInstance();
