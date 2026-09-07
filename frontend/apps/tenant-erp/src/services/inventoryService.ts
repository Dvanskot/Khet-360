import axios from 'axios';

// Create axios instance with base URL
const apiClient = axios.create({
  baseURL: '/api', // Vite dev server will proxy this to backend
  timeout: 10000,
});

// Request interceptor to add auth token
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('access_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    // Handle 401 Unauthorized errors
    if (error.response?.status === 401) {
      // Redirect to login or refresh token
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export interface InventoryItem {
  id: number;
  sku: string;
  name: string;
  description: string;
  category: string;
  unitPrice: number;
  quantityInStock: number;
  reorderLevel: number;
  supplierId?: number;
  location?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface InventoryFilters {
  category?: string;
  search?: string;
  lowStockOnly?: boolean;
}

export class InventoryService {
  static async getInventoryItems(filters: InventoryFilters = {}): Promise<InventoryItem[]> {
    try {
      const response = await apiClient.get('/inventory/items', { params: filters });
      return response.data;
    } catch (error) {
      console.error('Failed to fetch inventory items:', error);
      // Return mock data as fallback for development
      return [
        {
          id: 1,
          sku: 'COF-001',
          name: 'Oak Coffin - Standard',
          description: 'Standard oak coffin with brass handles',
          category: 'Coffins',
          unitPrice: 2500,
          quantityInStock: 5,
          reorderLevel: 2,
          supplierId: 101,
          location: 'Warehouse A, Shelf 3',
        },
        {
          id: 2,
          sku: 'URN-005',
          name: 'Marble Urn - Classic',
          description: 'Classic white marble urn',
          category: 'Urns',
          unitPrice: 800,
          quantityInStock: 12,
          reorderLevel: 3,
          supplierId: 102,
          location: 'Warehouse B, Shelf 1',
        },
        {
          id: 3,
          sku: 'FLR-012',
          name: 'Funeral Roses - Bouquet',
          description: 'Fresh rose bouquet for funeral services',
          category: 'Flowers',
          unitPrice: 120,
          quantityInStock: 8,
          reorderLevel: 4,
          supplierId: 103,
          location: 'Cooler Unit 2',
        },
      ];
    }
  }

  static async getInventoryItem(id: number): Promise<InventoryItem> {
    try {
      const response = await apiClient.get(`/inventory/items/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Failed to fetch inventory item ${id}:`, error);
      throw error;
    }
  }

  static async createInventoryItem(item: Omit<InventoryItem, 'id' | 'createdAt' | 'updatedAt'>): Promise<InventoryItem> {
    try {
      const response = await apiClient.post('/inventory/items', item);
      return response.data;
    } catch (error) {
      console.error('Failed to create inventory item:', error);
      throw error;
    }
  }

  static async updateInventoryItem(id: number, item: Partial<InventoryItem>): Promise<InventoryItem> {
    try {
      const response = await apiClient.put(`/inventory/items/${id}`, item);
      return response.data;
    } catch (error) {
      console.error(`Failed to update inventory item ${id}:`, error);
      throw error;
    }
  }

  static async deleteInventoryItem(id: number): Promise<void> {
    try {
      await apiClient.delete(`/inventory/items/${id}`);
    } catch (error) {
      console.error(`Failed to delete inventory item ${id}:`, error);
      throw error;
    }
  }

  static async updateStock(id: number, quantityChange: number): Promise<InventoryItem> {
    try {
      const response = await apiClient.patch(`/inventory/items/${id}/stock`, { quantityChange });
      return response.data;
    } catch (error) {
      console.error(`Failed to update stock for item ${id}:`, error);
      throw error;
    }
  }
}