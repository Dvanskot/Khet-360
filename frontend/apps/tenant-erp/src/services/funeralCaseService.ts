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

export interface FuneralCase {
  id: string;
  caseNumber: string;
  deceasedName: string;
  dateOfDeath: string;
  placeOfDeath: string;
  status: 'Pending' | 'In Progress' | 'Completed' | 'Archived';
  serviceType: 'Burial' | 'Cremation' | 'Memorial' | 'Repatriation';
  serviceDate?: string;
  serviceLocation?: string;
  assignedTo?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface FuneralCaseFilters {
  status?: string;
  serviceType?: string;
  search?: string;
  dateFrom?: string;
  dateTo?: string;
}

export class FuneralCaseService {
  static async getFuneralCases(filters: FuneralCaseFilters = {}): Promise<FuneralCase[]> {
    try {
      const response = await apiClient.get('/funeral-cases', { params: filters });
      return response.data;
    } catch (error) {
      console.error('Failed to fetch funeral cases:', error);
      // Return mock data as fallback for development
      return [
        {
          id: 'C-1024',
          caseNumber: 'C-1024',
          deceasedName: 'Johnathan Smith',
          dateOfDeath: '2026-09-01',
          placeOfDeath: 'Johannesburg General Hospital',
          status: 'In Progress',
          serviceType: 'Burial',
          serviceDate: '2026-09-07',
          serviceLocation: 'Evergreen Cemetery',
          assignedTo: 'Sarah J.',
          createdAt: '2026-09-01T10:30:00Z',
        },
        {
          id: 'C-1021',
          caseNumber: 'C-1021',
          deceasedName: 'Mary Johnson',
          dateOfDeath: '2026-08-28',
          placeOfDeath: 'Home',
          status: 'Pending',
          serviceType: 'Cremation',
          serviceDate: '2026-09-05',
          serviceLocation: 'City Crematorium',
          assignedTo: 'Mike R.',
          createdAt: '2026-08-28T14:15:00Z',
        },
        {
          id: 'C-1018',
          caseNumber: 'C-1018',
          deceasedName: 'Robert Williams',
          dateOfDeath: '2026-08-20',
          placeOfDeath: 'St. Mary\'s Hospice',
          status: 'Completed',
          serviceType: 'Burial',
          serviceDate: '2026-08-25',
          serviceLocation: 'Rosehill Memorial Park',
          assignedTo: 'Sarah J.',
          createdAt: '2026-08-20T09:45:00Z',
          updatedAt: '2026-08-26T16:30:00Z',
        },
      ];
    }
  }

  static async getFuneralCase(id: string): Promise<FuneralCase> {
    try {
      const response = await apiClient.get(`/funeral-cases/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Failed to fetch funeral case ${id}:`, error);
      throw error;
    }
  }

  static async createFuneralCase(caseData: Omit<FuneralCase, 'id' | 'createdAt' | 'updatedAt'>): Promise<FuneralCase> {
    try {
      const response = await apiClient.post('/funeral-cases', caseData);
      return response.data;
    } catch (error) {
      console.error('Failed to create funeral case:', error);
      throw error;
    }
  }

  static async updateFuneralCase(id: string, caseData: Partial<FuneralCase>): Promise<FuneralCase> {
    try {
      const response = await apiClient.put(`/funeral-cases/${id}`, caseData);
      return response.data;
    } catch (error) {
      console.error(`Failed to update funeral case ${id}:`, error);
      throw error;
    }
  }

  static async deleteFuneralCase(id: string): Promise<void> {
    try {
      await apiClient.delete(`/funeral-cases/${id}`);
    } catch (error) {
      console.error(`Failed to delete funeral case ${id}:`, error);
      throw error;
    }
  }

  static async updateStatus(id: string, status: FuneralCase['status']): Promise<FuneralCase> {
    try {
      const response = await apiClient.patch(`/funeral-cases/${id}/status`, { status });
      return response.data;
    } catch (error) {
      console.error(`Failed to update status for case ${id}:`, error);
      throw error;
    }
  }
}