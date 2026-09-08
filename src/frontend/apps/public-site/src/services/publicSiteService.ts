// Public Site Service - Enhanced with SignalR Integration
// Handles fetching and managing public site data with real-time updates

import { apiClient } from '@/khet360/api-client';
import { signalRService } from '@/services/signalRService';
import { notificationService } from '@/services/notificationService';

export class PublicSiteService {
  private static readonly BASE_PATH = '/api/public';

  /**
   * Get site statistics
   */
  static async getStatistics(): Promise<any> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/statistics`);
      return response.data;
    } catch (error) {
      throw new Error(`Failed to fetch site statistics: ${error.message}`);
    }
  }

  /**
   * Get testimonials
   */
  static async getTestimonials(): Promise<any[]> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/testimonials`);
      return response.data || [];
    } catch (error) {
      throw new Error(`Failed to fetch testimonials: ${error.message}`);
    }
  }

  /**
   * Get services
   */
  static async getServices(): Promise<any[]> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/services`);
      return response.data || [];
    } catch (error) {
      throw new Error(`Failed to fetch services: ${error.message}`);
    }
  }

  /**
   * Submit a contact form
   */
  static async submitContactForm(formData: any): Promise<any> {
    try {
      const response = await apiClient.post(`${this.BASE_PATH}/contact`, formData);
      
      // Notify via SignalR that a new contact form was submitted
      await signalRService.send('NewContactSubmission', {
        name: formData.name,
        email: formData.email,
        timestamp: new Date().toISOString()
      });
      
      // Show notification
      notificationService.addNotification({
        title: 'New Contact Form Submission',
        message: `New contact form submission from ${formData.name}`,
        type: 'info'
      });
      
      return response.data;
    } catch (error) {
      throw new Error(`Failed to submit contact form: ${error.message}`);
    }
  }

  /**
   * Subscribe to statistics updates via SignalR
   * Returns a cleanup function to unsubscribe
   */
  static subscribeToStatisticsUpdates(
    onUpdated: (stats: any) => void = () => {}
  ): () => void {
    // Set up SignalR listeners
    signalRService.on('StatisticsUpdated', onUpdated);
    
    // Return cleanup function
    return () => {
      signalRService.off('StatisticsUpdated', onUpdated);
    };
  }

  /**
   * Subscribe to new testimonials via SignalR
   * Returns a cleanup function to unsubscribe
   */
  static subscribeToNewTestimonials(
    onAdded: (testimonial: any) => void = () => {}
  ): () => void {
    // Set up SignalR listeners
    signalRService.on('NewTestimonialAdded', onAdded);
    
    // Return cleanup function
    return () => {
      signalRService.off('NewTestimonialAdded', onAdded);
    };
  }

  /**
   * Subscribe to service updates via SignalR
   * Returns a cleanup function to unsubscribe
   */
  static subscribeToServiceUpdates(
    onUpdated: (service: any) => void = () => {},
    onAdded: (service: any) => void = () => {},
    onRemoved: (id: number) => void = () => {}
  ): () => void {
    // Set up SignalR listeners
    signalRService.on('ServiceUpdated', onUpdated);
    signalRService.on('ServiceAdded', onAdded);
    signalRService.on('ServiceRemoved', onRemoved);
    
    // Return cleanup function
    return () => {
      signalRService.off('ServiceUpdated', onUpdated);
      signalRService.off('ServiceAdded', onAdded);
      signalRService.off('ServiceRemoved', onRemoved);
    };
  }
}

// Initialize SignalR connection when service is imported
signalRService.start().catch(error => {
  console.warn('Failed to start SignalR connection in PublicSiteService:', error);
});

export default PublicSiteService;