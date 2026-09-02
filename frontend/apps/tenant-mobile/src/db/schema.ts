import Dexie, { Table } from 'dexie';

export interface LocalTask {
  id: string;
  caseId: string;
  title: string;
  description: string;
  status: 'Pending' | 'InProgress' | 'Completed';
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  dueAt: string;
  updatedAt: string;
}

export interface LocalTrip {
  id: string;
  caseId: string;
  origin: string;
  destination: string;
  status: 'Scheduled' | 'Dispatched' | 'InTransit' | 'Completed';
  driverId: string;
  vehicleId: string;
  updatedAt: string;
}

export interface LocalCustody {
  id: string;
  caseId: string;
  location: string;
  slotId?: string;
  status: 'Admitted' | 'Moved' | 'Released';
  updatedAt: string;
}

export interface LocalProductionOrder {
  id: string;
  memorialId: string;
  stage: 'SlabSelection' | 'Cutting' | 'Polishing' | 'Engraving' | 'Finishing' | 'QualityCheck' | 'Ready';
  status: 'Active' | 'OnHold' | 'Completed';
  updatedAt: string;
}

export interface SyncCommand {
  id: string; // ClientTransactionId (UUID)
  entityType: string;
  entityId: string;
  action: 'CREATE' | 'UPDATE' | 'DELETE';
  payload: any;
  timestamp: string;
  version: number;
  status: 'Pending' | 'Synced' | 'Failed';
  errorMessage?: string;
}

export interface SyncEvent {
  id: string;
  entityType: string;
  entityId: string;
  action: 'CREATE' | 'UPDATE' | 'DELETE';
  payload: any;
  timestamp: string;
  version: number;
  processed: boolean;
}

export class KhetMobileDB extends Dexie {
  tasks!: Table<LocalTask>;
  trips!: Table<LocalTrip>;
  custody!: Table<LocalCustody>;
  production!: Table<LocalProductionOrder>;
  outbox!: Table<SyncCommand>;
  inbox!: Table<SyncEvent>;

  constructor() {
    super('KhetMobileDB');
    this.version(1).stores({
      tasks: 'id, caseId, status',
      trips: 'id, caseId, status',
      custody: 'id, caseId, status',
      production: 'id, memorialId, stage',
      outbox: 'id, status, entityType',
      inbox: 'id, processed',
    });
  }
}

export const db = new KhetMobileDB();
