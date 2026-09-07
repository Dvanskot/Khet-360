// Vendor Hub Service - Enhanced with SignalR Integration
// Handles fetching and managing vendor data with real-time updates

import { apiClient } from '@/khet360/api-client';
import { signalRService } from '@/services/signalRService';
import { notificationService } from '@/services/notificationService';

export class VendorHubService {
  private static readonly BASE_PATH = '/api/vendor';

  /**
   * Get all products
   */
  static async getProducts(): Promise<any[]> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/products`);
      return response.data || [];
    } catch (error) {
      throw new Error(`Failed to fetch products: ${error.message}`);
    }
  }

  /**
   * Get product by ID
   */
  static async getProductById(id: number): Promise<any> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/products/${id}`);
      return response.data;
    } catch (error) {
      throw new Error(`Failed to fetch product ${id}: ${error.message}`);
    }
  }

  /**
   * Create a new product
   */
  static async createProduct(productData: any): Promise<any> {
    try {
      const response = await apiClient.post(`${this.BASE_PATH}/products`, productData);
      
      // Notify via SignalR that a product was created
      await signalRService.send('ProductCreated', response.data);
      
      // Show notification
      notificationService.addNotification({
        title: 'New Product Added',
        message: `Product "${response.data.name}" has been added to the catalog`,
        type: 'success'
      });
      
      return response.data;
    } catch (error) {
      throw new Error(`Failed to create product: ${error.message}`);
    }
  }

  /**
   * Update an existing product
   */
  static async updateProduct(id: number, productData: any): Promise<any> {
    try {
      const response = await apiClient.put(`${this.BASE_PATH}/products/${id}`, productData);
      
      // Notify via SignalR that a product was updated
      await signalRService.send('ProductUpdated', response.data);
      
      // Show notification for stock updates
      if (productData.stockQuantity !== undefined) {
        notificationService.addNotification({
          title: 'Stock Update',
          message: `Product "${response.data.name}" stock updated to ${response.data.stockQuantity} units`,
          type: response.data.stockQuantity <= 5 ? 'warning' : 'info'
        });
      }
      
      return response.data;
    } catch (error) {
      throw new Error(`Failed to update product ${id}: ${error.message}`);
    }
  }

  /**
   * Delete a product
   */
  static async deleteProduct(id: number): Promise<void> {
    try {
      await apiClient.delete(`${this.BASE_PATH}/products/${id}`);
      
      // Notify via SignalR that a product was deleted
      await signalRService.send('ProductDeleted', id);
      
      // Show notification
      notificationService.addNotification({
        title: 'Product Removed',
        message: `Product has been removed from the catalog`,
        type: 'warning'
      });
    } catch (error) {
      throw new Error(`Failed to delete product ${id}: ${error.message}`);
    }
  }

  /**
   * Get inventory levels
   */
  static async getInventory(): Promise<any[]> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/inventory`);
      return response.data || [];
    } catch (error) {
      throw new Error(`Failed to fetch inventory: ${error.message}`);
    }
  }

  /**
   * Get low stock items
   */
  static async getLowStockItems(): Promise<any[]> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/inventory/low-stock`);
      return response.data || [];
    } catch (error) {
      throw new Error(`Failed to fetch low stock items: ${error.message}`);
    }
  }

  /**
   * Subscribe to product updates via SignalR
   * Returns a cleanup function to unsubscribe
   */
  static subscribeToProductUpdates(
    onCreated: (product: any) => void = () => {},
    onUpdated: (product: any) => void = () => {},
    onDeleted: (id: number) => void = () => {}
  ): () => void {
    // Set up SignalR listeners
    signalRService.on('ProductCreated', onCreated);
    signalRService.on('ProductUpdated', onUpdated);
    signalRService.on('ProductDeleted', onDeleted);
    
    // Return cleanup function
    return () => {
      signalRService.off('ProductCreated', onCreated);
      signalRService.off('ProductUpdated', onUpdated);
      signalRService.off('ProductDeleted', onDeleted);
    };
  }

  /**
   * Get products with filtering and pagination
   */
  static async getProductsFiltered(
    filters: any = {},
    page: number = 1,
    pageSize: number = 10
  ): Promise<any> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/products`, {
        params: {
          ...filters,
          page,
          pageSize
        }
      });
      return response.data;
    } catch (error) {
      throw new Error(`Failed to fetch filtered products: ${error.message}`);
    }
  }
}

// Initialize SignalR connection when service is imported
signalRService.start().catch(error => {
  console.warn('Failed to start SignalR connection in VendorHubService:', error);
});

export default VendorHubService;