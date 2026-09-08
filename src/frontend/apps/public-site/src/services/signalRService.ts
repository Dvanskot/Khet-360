// SignalR Service for Real-time Updates
// Handles connection to the notification hub for real-time updates

import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

export class SignalRService {
  private connection: HubConnection | null = null;
  private isConnected = false;
  private reconnectAttempts = 0;
  private readonly maxReconnectAttempts = 5;
  private readonly reconnectDelay = 3000; // 3 seconds

  constructor() {
    this.initializeConnection();
  }

  private initializeConnection() {
    try {
      this.connection = new HubConnectionBuilder()
        .withUrl('/hubs/notifications', {
          accessTokenFactory: () => localStorage.getItem('access_token') || ''
        })
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: retryContext => {
            if (retryContext.elapsedMilliseconds < 60000) {
              // If we've been reconnecting for less than 60 seconds, use exponential backoff
              return Math.min(
                10000, // max delay 10 seconds
                Math.pow(2, retryContext.retryCount) * 1000
              );
            } else {
              // After 60 seconds, try every 10 seconds
              return 10000;
            }
          }
        })
        .configureLogging(LogLevel.Information)
        .build();

      this.setupEventListeners();
    } catch (error) {
      console.error('Failed to initialize SignalR connection:', error);
    }
  }

  private setupEventListeners() {
    if (!this.connection) return;

    this.connection.onclose(async (error) => {
      console.warn('SignalR connection closed', error);
      this.isConnected = false;
      await this.handleReconnection();
    });

    this.connection.onreconnecting((error) => {
      console.warn('SignalR reconnecting', error);
      this.isConnected = false;
      this.notifyConnectionStatusChange(false, 'reconnecting');
    });

    this.connection.onreconnected((connectionId) => {
      console.log('SignalR reconnected', connectionId);
      this.isConnected = true;
      this.reconnectAttempts = 0;
      this.notifyConnectionStatusChange(true, 'connected');
    });
  }

  private async handleReconnection() {
    if (this.reconnectAttempts < this.maxReconnectAttempts) {
      this.reconnectAttempts++;
      setTimeout(() => this.initializeConnection(), this.reconnectDelay);
    } else {
      console.error('Max reconnection attempts reached');
      this.notifyConnectionStatusChange(false, 'failed');
    }
  }

  private notifyConnectionStatusChange(isConnected: boolean, status: string) {
    // Dispatch a custom event that components can listen for
    window.dispatchEvent(new CustomEvent('signalr-connection-change', {
      detail: { connected: isConnected, status }
    }));
  }

  public async start(): Promise<void> {
    if (!this.connection) {
      this.initializeConnection();
    }

    try {
      await this.connection?.start();
      this.isConnected = true;
      this.reconnectAttempts = 0;
      console.log('SignalR connected successfully');
      this.notifyConnectionStatusChange(true, 'connected');
    } catch (error) {
      console.error('Failed to start SignalR connection:', error);
      this.isConnected = false;
      this.notifyConnectionStatusChange(false, 'failed');
      throw error;
    }
  }

  public async stop(): Promise<void> {
    if (this.connection) {
      try {
        await this.connection.stop();
        this.isConnected = false;
        console.log('SignalR connection stopped');
        this.notifyConnectionStatusChange(false, 'stopped');
      } catch (error) {
        console.error('Error stopping SignalR connection:', error);
      }
    }
  }

  public isConnectedStatus(): boolean {
    return this.isConnected && this.connection?.state === 'Connected';
  }

  public on<T>(eventName: string, callback: (data: T) => void): void {
    if (this.connection) {
      this.connection.on(eventName, callback);
    }
  }

  public off<T>(eventName: string, callback: (data: T) => void): void {
    if (this.connection) {
      this.connection.off(eventName, callback);
    }
  }

  public send<T>(methodName: string, ...args: any[]): Promise<void> {
    if (!this.connection || !this.isConnectedStatus()) {
      return Promise.reject(new Error('SignalR connection not available'));
    }
    return this.connection.invoke(methodName, ...args);
  }
}

// Export a singleton instance for easy access throughout the application
export const signalRService = new SignalRService();