// Notification Service
// Manages application notifications and integrates with SignalR for real-time updates

export class NotificationService {
  private notifications: Array<{
    id: string;
    title: string;
    message: string;
    type: 'info' | 'success' | 'warning' | 'error';
    timestamp: Date;
    read: boolean;
  }> = [];
  
  private listeners: Array<(notifications: Array<any>) => void> = [];

  addNotification(notification: Omit<typeof this.notifications[0], 'id' | 'timestamp' | 'read'>): string {
    const id = Math.random().toString(36).substr(2, 9);
    const newNotification = {
      ...notification,
      id,
      timestamp: new Date(),
      read: false
    };
    
    this.notifications.unshift(newNotification);
    this.notifyListeners();
    return id;
  }

  markAsRead(id: string): void {
    const notification = this.notifications.find(n => n.id === id);
    if (notification) notification.read = true;
  }

  removeNotification(id: string): void {
    this.notifications = this.notifications.filter(n => n.id !== id);
    this.notifyListeners();
  }

  getNotifications(): Array<any> {
    return [...this.notifications];
  }

  getUnreadCount(): number {
    return this.notifications.filter(n => !n.read).length;
  }

  clear(): void {
    this.notifications = [];
    this.notifyListeners();
  }

  addListener(callback: (notifications: Array<any>) => void): () => void {
    this.listeners.push(callback);
    return () => {
      this.listeners = this.listeners.filter(l => l !== callback);
    };
  }

  private notifyListeners(): void {
    this.listeners.forEach(listener => listener(this.notifications));
  }
}

// Export singleton instance
export const notificationService = new NotificationService();