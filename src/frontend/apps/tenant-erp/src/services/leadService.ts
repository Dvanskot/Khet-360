// Lead Service - Enhanced with SignalR Integration
// Handles fetching and managing leads with real-time updates

import { apiClient } from '@/khet360/api-client';
import { signalRService } from '@/services/signalRService';
import { notificationService } from '@/services/notificationService';

export class LeadService {
  private static readonly BASE_PATH = '/api/leads';

  /**
   * Get all leads
   */
  static async getLeads(): Promise<any[]> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}`);
      return response.data || [];
    } catch (error) {
      throw new Error(`Failed to fetch leads: ${error.message}`);
    }
  }

  /**
   * Get lead by ID
   */
  static async getLeadById(id: number): Promise<any> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/${id}`);
      return response.data;
    } catch (error) {
      throw new Error(`Failed to fetch lead ${id}: ${error.message}`);
    }
  }

  /**
   * Create a new lead
   */
  static async createLead(leadData: any): Promise<any> {
    try {
      const response = await apiClient.post(this.BASE_PATH, leadData);
      
      // Notify via SignalR that a lead was created
      await signalRService.send('LeadCreated', response.data);
      
      // Show notification
      notificationService.addNotification({
        title: 'New Lead Created',
        message: `New lead "${response.data.contactName}" from ${response.data.organization || 'unknown'} has been created`,
        type: 'success'
      });
      
      return response.data;
    } catch (error) {
      throw new Error(`Failed to create lead: ${error.message}`);
    }
  }

  /**
   * Update an existing lead
   */
  static async updateLead(id: number, leadData: any): Promise<any> {
    try {
      const response = await apiClient.put(`${this.BASE_PATH}/${id}`, leadData);
      
      // Notify via SignalR that a lead was updated
      await signalRService.send('LeadUpdated', response.data);
      
      // Show notification for status changes
      if (leadData.status) {
        notificationService.addNotification({
          title: 'Lead Updated',
          message: `Lead "${response.data.contactName}" status changed to ${response.data.status}`,
          type: 'info'
        });
      }
      
      return response.data;
    } catch (error) {
      throw new Error(`Failed to update lead ${id}: ${error.message}`);
    }
  }

  /**
   * Delete a lead
   */
  static async deleteLead(id: number): Promise<void> {
    try {
      await apiClient.delete(`${this.BASE_PATH}/${id}`);
      
      // Notify via SignalR that a lead was deleted
      await signalRService.send('LeadDeleted', id);
      
      // Show notification
      notificationService.addNotification({
        title: 'Lead Deleted',
        message: `Lead #${id} has been removed`,
        type: 'warning'
      });
    } catch (error) {
      throw new Error(`Failed to delete lead ${id}: ${error.message}`);
    }
  }

  /**
   * Convert lead to opportunity/case
   */
  static async convertLead(id: number, caseData: any): Promise<any> {
    try {
      const response = await apiClient.post(`${this.BASE_PATH}/${id}/convert`, caseData);
      
      // Notify via SignalR that a lead was converted
      await signalRService.send('LeadConverted', {
        leadId: id,
        caseId: response.data.id,
        ...response.data
      });
      
      // Show notification
      notificationService.addNotification({
        title: 'Lead Converted',
        message: `Lead "${response.data.contactName}" has been converted to a case`,
        type: 'success'
      });
      
      return response.data;
    } catch (error) {
      throw new Error(`Failed to convert lead ${id}: ${error.message}`);
    }
  }

  /**
   * Get leads by status
   */
  static async getLeadsByStatus(status: string): Promise<any[]> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/status/${status}`);
      return response.data || [];
    } catch (error) {
      throw new Error(`Failed to fetch leads with status ${status}: ${error.message}`);
    }
  }

  /**
   * Subscribe to lead updates via SignalR
   * Returns a cleanup function to unsubscribe
   */
  static subscribeToLeadUpdates(
    onCreated: (lead: any) => void = () => {},
    onUpdated: (lead: any) => void = () => {},
    onDeleted: (id: number) => void = () => {},
    onConverted: (data: any) => void = () => {}
  ): () => void {
    // Set up SignalR listeners
    signalRService.on('LeadCreated', onCreated);
    signalRService.on('LeadUpdated', onUpdated);
    signalRService.on('LeadDeleted', onDeleted);
    signalRService.on('LeadConverted', onConverted);
    
    // Return cleanup function
    return () => {
      signalRService.off('LeadCreated', onCreated);
      signalRService.off('LeadUpdated', onUpdated);
      signalRService.off('LeadDeleted', onDeleted);
      signalRService.off('LeadConverted', onConverted);
    };
  }

  /**
   * Get leads with filtering and pagination
   */
  static async getLeadsFiltered(
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
      throw new Error(`Failed to fetch filtered leads: ${error.message}`);
    }
  }
}

// Initialize SignalR connection when service is imported
signalRService.start().catch(error => {
  console.warn('Failed to start SignalR connection in LeadService:', error);
});

export default LeadService;