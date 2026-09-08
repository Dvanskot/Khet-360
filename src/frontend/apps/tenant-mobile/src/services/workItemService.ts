// Work Item Service for Mobile - Enhanced with SignalR Integration
// Handles fetching and managing work items with real-time updates and offline support

import { apiClient } from '@/khet360/api-client';
import { signalRService } from '@/services/signalRService';
import { notificationService } from '@/services/notificationService';
import { db } from '@/db/schema';
import { syncEngine } from '@/sync/sync-engine';

export class MobileWorkItemService {
  private static readonly BASE_PATH = '/api/work-items';

  /**
   * Get all work items from server
   */
  static async getWorkItems(): Promise<any[]> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}`);
      return response.data || [];
    } catch (error) {
      console.warn('Failed to fetch work items from server:', error);
      throw new Error(`Failed to fetch work items: ${error.message}`);
    }
  }

  /**
   * Get work item by ID from server
   */
  static async getWorkItemById(id: number): Promise<any> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/${id}`);
      return response.data;
    } catch (error) {
      console.warn(`Failed to fetch work item ${id} from server:`, error);
      throw new Error(`Failed to fetch work item ${id}: ${error.message}`);
    }
  }

  /**
   * Create a new work item
   */
  static async createWorkItem(workItemData: any): Promise<any> {
    try {
      const response = await apiClient.post(this.BASE_PATH, workItemData);
      
      // Notify via SignalR that a work item was created
      await signalRService.send('WorkItemCreated', response.data);
      
      // Show notification
      notificationService.addNotification({
        title: 'Work Item Created',
        message: `New work item "${response.data.title}" has been created`,
        type: 'success'
      });
      
      return response.data;
    } catch (error) {
      console.error('Failed to create work item:', error);
      throw new Error(`Failed to create work item: ${error.message}`);
    }
  }

  /**
   * Update an existing work item
   */
  static async updateWorkItem(id: number, workItemData: any): Promise<any> {
    try {
      const response = await apiClient.put(`${this.BASE_PATH}/${id}`, workItemData);
      
      // Notify via SignalR that a work item was updated
      await signalRService.send('WorkItemUpdated', response.data);
      
      // Show notification for significant updates
      if (workItemData.status || workItemData.priority) {
        notificationService.addNotification({
          title: 'Work Item Updated',
          message: `Work item "${response.data.title}" has been updated`,
          type: 'info'
        });
      }
      
      return response.data;
    } catch (error) {
      console.error('Failed to update work item:', error);
      throw new Error(`Failed to update work item ${id}: ${error.message}`);
    }
  }

  /**
   * Delete a work item
   */
  static async deleteWorkItem(id: number): Promise<void> {
    try {
      await apiClient.delete(`${this.BASE_PATH}/${id}`);
      
      // Notify via SignalR that a work item was deleted
      await signalRService.send('WorkItemDeleted', id);
      
      // Show notification
      notificationService.addNotification({
        title: 'Work Item Deleted',
        message: `Work item #${id} has been removed`,
        type: 'warning'
      });
    } catch (error) {
      console.error('Failed to delete work item:', error);
      throw new Error(`Failed to delete work item ${id}: ${error.message}`);
    }
  }

  /**
   * Complete a work item (special action)
   */
  static async completeWorkItem(id: number): Promise<any> {
    try {
      const response = await apiClient.post(`${this.BASE_PATH}/${id}/complete`);
      
      // Notify via SignalR that a work item was completed
      await signalRService.send('WorkItemCompleted', {
        id,
        ...response.data
      });
      
      // Show notification
      notificationService.addNotification({
        title: 'Work Item Completed',
        message: `Work item "${response.data.title}" has been marked as complete`,
        type: 'success'
      });
      
      return response.data;
    } catch (error) {
      console.error('Failed to complete work item:', error);
      throw new Error(`Failed to complete work item ${id}: ${error.message}`);
    }
  }

  /**
   * Get work items for a specific case
   */
  static async getWorkItemsByCaseId(caseId: string): Promise<any[]> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/case/${caseId}`);
      return response.data || [];
    } catch (error) {
      console.warn(`Failed to fetch work items for case ${caseId} from server:`, error);
      throw new Error(`Failed to fetch work items for case ${caseId}: ${error.message}`);
    }
  }

  /**
   * Subscribe to work item updates via SignalR
   * Returns a cleanup function to unsubscribe
   */
  static subscribeToWorkItemUpdates(
    onCreated: (item: any) => void = () => {},
    onUpdated: (item: any) => void = () => {},
    onDeleted: (id: number) => void = () => {},
    onCompleted: (item: any) => void = () => {}
  ): () => void {
    // Set up SignalR listeners
    signalRService.on('WorkItemCreated', onCreated);
    signalRService.on('WorkItemUpdated', onUpdated);
    signalRService.on('WorkItemDeleted', onDeleted);
    signalRService.on('WorkItemCompleted', onCompleted);
    
    // Return cleanup function
    return () => {
      signalRService.off('WorkItemCreated', onCreated);
      signalRService.off('WorkItemUpdated', onUpdated);
      signalRService.off('WorkItemDeleted', onDeleted);
      signalRService.off('WorkItemCompleted', onCompleted);
    };
  }

  /**
   * Get work items with filtering and pagination
   */
  static async getWorkItemsFiltered(
    filters: any = {},
    page: number = 1,
    pageSize: number = 10
  ): Promise<any> {
    try {
      const response = await apiClient.get(this.BASE_PATH, {
        params: {
          ...filters,
          page,
          pageSize
        }
      });
      return response.data;
    } catch (error) {
      console.error('Failed to fetch filtered work items:', error);
      throw new Error(`Failed to fetch filtered work items: ${error.message}`);
    }
  }
}

// Initialize SignalR connection when service is imported
signalRService.start().catch(error => {
  console.warn('Failed to start SignalR connection in MobileWorkItemService:', error);
});

export default MobileWorkItemService;