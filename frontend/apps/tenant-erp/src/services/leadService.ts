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

export interface Lead {
  id: string;
  source: string;
  customerName: string;
  phone: string;
  email?: string;
  interest?: string;
  status: 'New' | 'Contacted' | 'Qualified' | 'Converted';
  assignedTo: string;
  createdDate: string;
  lastContactDate?: string;
  priority: 'Low' | 'Medium' | 'High';
}

export interface LeadFilters {
  status?: string;
  priority?: string;
  search?: string;
}

export class LeadService {
  static async getLeads(filters: LeadFilters = {}): Promise<Lead[]> {
    try {
      const response = await apiClient.get('/leads', { params: filters });
      return response.data;
    } catch (error) {
      console.error('Failed to fetch leads:', error);
      // Return mock data as fallback for development
      return [
        { id: 'L1', source: 'Website', customerName: 'Thabo Mbeki', phone: '+27 83 111 2222', email: 'thabo@example.co.za', interest: 'Burial Plan', status: 'New', assignedTo: 'Sarah J.', createdDate: '2026-09-01', priority: 'High' },
        { id: 'L2', source: 'Referral', customerName: 'Nomvula Zulu', phone: '+27 71 333 4444', email: 'nomvula@example.co.za', interest: 'Cash Payout', status: 'Contacted', assignedTo: 'Sarah J.', createdDate: '2026-08-30', lastContactDate: '2026-08-31', priority: 'Medium' },
        { id: 'L3', source: 'Walk-in', customerName: 'Pieter Botha', phone: '+27 82 555 6666', email: 'pieter@example.co.za', interest: 'Premium Package', status: 'Qualified', assignedTo: 'Mike R.', createdDate: '2026-08-28', lastContactDate: '2026-08-29', priority: 'High' },
        { id: 'L4', source: 'Phone', customerName: 'Grace Khumalo', phone: '+27 72 777 8888', email: 'grace@example.co.za', interest: 'Burial Plan', status: 'Converted', assignedTo: 'Mike R.', createdDate: '2026-08-20', lastContactDate: '2026-08-22', priority: 'Low' },
      ];
    }
  }

  static async getLead(id: string): Promise<Lead> {
    try {
      const response = await apiClient.get(`/leads/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Failed to fetch lead ${id}:`, error);
      throw error;
    }
  }

  static async createLead(lead: Omit<Lead, 'id' | 'createdDate'>): Promise<Lead> {
    try {
      const response = await apiClient.post('/leads', lead);
      return response.data;
    } catch (error) {
      console.error('Failed to create lead:', error);
      throw error;
    }
  }

  static async updateLead(id: string, lead: Partial<Lead>): Promise<Lead> {
    try {
      const response = await apiClient.put(`/leads/${id}`, lead);
      return response.data;
    } catch (error) {
      console.error(`Failed to update lead ${id}:`, error);
      throw error;
    }
  }

  static async deleteLead(id: string): Promise<void> {
    try {
      await apiClient.delete(`/leads/${id}`);
    } catch (error) {
      console.error(`Failed to delete lead ${id}:`, error);
      throw error;
    }
  }

  static async contactLead(id: string): Promise<void> {
    try {
      await apiClient.post(`/leads/${id}/contact`);
    } catch (error) {
      console.error(`Failed to contact lead ${id}:`, error);
      throw error;
    }
  }

  static async convertLead(id: string): Promise<Lead> {
    try {
      const response = await apiClient.post(`/leads/${id}/convert`);
      return response.data;
    } catch (error) {
      console.error(`Failed to convert lead ${id}:`, error);
      throw error;
    }
  }
}