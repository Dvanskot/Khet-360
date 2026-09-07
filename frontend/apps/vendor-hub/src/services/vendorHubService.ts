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

export interface PurchaseOrder {
  id: string;
  poNumber: string;
  mainItem: string;
  quantity: number;
  deliveryLocation: string;
  dueDate: string;
  status: 'Pending' | 'Submitted' | 'Approved' | 'Delivered' | 'Cancelled';
  unitPrice?: number;
  totalAmount?: number;
  vendorId?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface InvoiceSubmission {
  poNumber: string;
  amount: number;
  invoicePdf: File; // In a real app, this would be handled differently
  description?: string;
}

export class VendorHubService {
  static async getPurchaseOrders(): Promise<PurchaseOrder[]> {
    try {
      const response = await apiClient.get('/vendor-hub/orders');
      return response.data;
    } catch (error) {
      console.error('Failed to fetch purchase orders:', error);
      // Return mock data as fallback for development
      return [
        { id: 'O1', poNumber: 'PO-2026-881', mainItem: 'Premium Mahogany Casket', quantity: 2, deliveryLocation: 'Cape Town Central', dueDate: '2026-09-05', status: 'Approved', unitPrice: 4500, totalAmount: 9000 },
        { id: 'O2', poNumber: 'PO-2026-885', mainItem: 'Standard Oak Casket', quantity: 5, deliveryLocation: 'Stellenbosch', dueDate: '2026-09-08', status: 'Submitted', unitPrice: 2500, totalAmount: 12500 },
        { id: 'O3', poNumber: 'PO-2026-890', mainItem: 'Ceramic Urns (White)', quantity: 10, deliveryLocation: 'Paarl', dueDate: '2026-09-10', status: 'Approved', unitPrice: 200, totalAmount: 2000 },
      ];
    }
  }

  static async confirmDelivery(orderId: string): Promise<{ success: boolean }> {
    try {
      const response = await apiClient.post(`/vendor-hub/orders/${orderId}/confirm-delivery`);
      return response.data;
    } catch (error) {
      console.error(`Failed to confirm delivery for order ${orderId}:`, error);
      throw error;
    }
  }

  static async submitInvoice(submission: Omit<InvoiceSubmission, 'invoicePdf'>): Promise<{ success: boolean; invoiceId?: string }> {
    try {
      // In a real implementation, we'd handle file upload separately
      const response = await apiClient.post('/vendor-hub/invoices', submission);
      return response.data;
    } catch (error) {
      console.error('Failed to submit invoice:', error);
      throw error;
    }
  }

  static async getVendorInvoices(vendorId?: string): Promise<Array<{
    id: string;
    invoiceNumber: string;
    poNumber: string;
    amount: number;
    status: 'Pending' | 'Approved' | 'Paid' | 'Rejected';
    submittedDate: string;
    paidDate?: string;
  }>> {
    try {
      const params = vendorId ? { vendorId } : {};
      const response = await apiClient.get('/vendor-hub/invoices', { params });
      return response.data;
    } catch (error) {
      console.error('Failed to fetch vendor invoices:', error);
      // Return mock data as fallback for development
      return [
        {
          id: 'INV-00123',
          invoiceNumber: 'INV-2026-001',
          poNumber: 'PO-2026-881',
          amount: 9000,
          status: 'Paid',
          submittedDate: '2026-09-02',
          paidDate: '2026-09-03',
        },
        {
          id: 'INV-00124',
          invoiceNumber: 'INV-2026-002',
          poNumber: 'PO-2026-885',
          amount: 12500,
          status: 'Pending',
          submittedDate: '2026-09-05',
        },
      ];
    }
  }
}