// Family Portal Service - Enhanced with SignalR Integration
// Handles fetching and managing family portal data with real-time updates

import { apiClient } from '@/khet360/api-client';
import { signalRService } from '@/services/signalRService';
import { notificationService } from '@/services/notificationService';

export class FamilyPortalService {
  private static readonly BASE_PATH = '/api/family-portal';

  /**
   * Get case timeline: FuneralCaseTimeline {
    deceasedName: string;
    currentStage: string;
    currentStageIndex: number;
    milestones: Array<{ title: string; description: string; date?: string }>;
    requiredDocuments: Array<{ name: string; uploaded: boolean; uploadDate?: string }>;
    financialSummary: {
      totalPackageCost: number;
      insuranceCover: number;
      outstandingBalance: number;
      currency: string;
    };
    assignedFuneralDirector: {
      name: string;
      contactNumber: string;
    };
  }
   */
  /**
   * Get case timeline
   */
  static async getCaseTimeline(caseId: string): Promise<any> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/case/${caseId}/timeline`);
      return response.data;
    } catch (error) {
      throw new Error(`Failed to fetch case timeline: ${error.message}`);
    }
  }

  /**
   * Upload a document
   */
  static async uploadDocument(caseId: string, documentType: string, file: File): Promise<any> {
    try {
      const formData = new FormData();
      formData.append('file', file);
      formData.append('documentType', documentType);
      
      const response = await apiClient.post(`${this.BASE_PATH}/case/${caseId}/documents`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      });
      
      // Notify via SignalR that a document was uploaded
      await signalRService.send('DocumentUploaded', {
        caseId,
        documentType,
        fileName: file.name
      });
      
      // Show notification
      notificationService.addNotification({
        title: 'Document Uploaded',
        message: `Document "${file.name}" has been uploaded successfully`,
        type: 'success'
      });
      
      return response.data;
    } catch (error) {
      throw new Error(`Failed to upload document: ${error.message}`);
    }
  }

  /**
   * Make a payment
   */
  static async makePayment(caseId: string, paymentData: any): Promise<any> {
    try {
      const response = await apiClient.post(`${this.BASE_PATH}/case/${caseId}/payment`, paymentData);
      
      // Notify via SignalR that a payment was made
      await signalRService.send('PaymentMade', {
        caseId,
        amount: paymentData.amount,
        ...response.data
      });
      
      // Show notification
      notificationService.addNotification({
        title: 'Payment Processed',
        message: `Payment of R ${paymentData.amount.toLocaleString()} has been processed successfully`,
        type: 'success'
      });
      
      return response.data;
    } catch (error) {
      throw new Error(`Failed to make payment: ${error.message}`);
    }
  }

  /**
   * Get required documents for a case
   */
  static async getRequiredDocuments(caseId: string): Promise<any[]> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/case/${caseId}/documents/required`);
      return response.data || [];
    } catch (error) {
      throw new Error(`Failed to fetch required documents: ${error.message}`);
    }
  }

  /**
   * Get financial summary for a case
   */
  static async getFinancialSummary(caseId: string): Promise<any> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/case/${caseId}/financial-summary`);
      return response.data;
    } catch (error) {
      throw new Error(`Failed to fetch financial summary: ${error.message}`);
    }
  }

  /**
   * Subscribe to case timeline updates via SignalR
   * Returns a cleanup function to unsubscribe
   */
  static subscribeToCaseTimelineUpdates(
    onUpdated: (timeline: any) => void = () => {}
  ): () => void {
    // Set up SignalR listeners
    signalRService.on('CaseTimelineUpdated', onUpdated);
    
    // Return cleanup function
    return () => {
      signalRService.off('CaseTimelineUpdated', onUpdated);
    };
  }

  /**
   * Subscribe to document uploads via SignalR
   * Returns a cleanup function to unsubscribe
   */
  static subscribeToDocumentUpdates(
    onUploaded: (data: any) => void = () => {}
  ): () => void {
    // Set up SignalR listeners
    signalRService.on('DocumentUploaded', onUploaded);
    
    // Return cleanup function
    return () => {
      signalRService.off('DocumentUploaded', onUploaded);
    };
  }

  /**
   * Subscribe to payments via SignalR
   * Returns a cleanup function to unsubscribe
   */
  static subscribeToPaymentUpdates(
    onPaymentMade: (data: any) => void = () => {}
  ): () => void {
    // Set up SignalR listeners
    signalRService.on('PaymentMade', onPaymentMade);
    
    // Return cleanup function
    return () => {
      signalRService.off('PaymentMade', onPaymentMade);
    };
  }
}

// Initialize SignalR connection when service is imported
signalRService.start().catch(error => {
  console.warn('Failed to start SignalR connection in FamilyPortalService:', error);
});

export default FamilyPortalService;