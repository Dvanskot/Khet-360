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

export interface MortuarySlot {
  id: number;
  slotNumber: string;
  date: string;
  startTime: string;
  endTime: string;
  status: 'Available' | 'Occupied' | 'Reserved' | 'Maintenance';
  funeralCaseId?: string;
  deceasedName?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface RepatriationCase {
  id: string;
  caseNumber: string;
  deceasedName: string;
  nationality: string;
  destinationCountry: string;
  destinationCity: string;
  status: 'Pending Documentation' | 'Documentation Complete' | 'In Transit' | 'Delivered' | 'Completed';
  dateOfDeath: string;
  estimatedCompletionDate: string;
  assignedTo?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface OperationFilters {
  dateFrom?: string;
  dateTo?: string;
  status?: string;
  search?: string;
}

export class OperationsService {
  static async getMortuarySlots(filters: OperationFilters = {}): Promise<MortuarySlot[]> {
    try {
      const response = await apiClient.get('/operations/mortuary/slots', { params: filters });
      return response.data;
    } catch (error) {
      console.error('Failed to fetch mortuary slots:', error);
      // Return mock data as fallback for development
      return [
        {
          id: 1,
          slotNumber: 'M-001',
          date: '2026-09-07',
          startTime: '08:00',
          endTime: '10:00',
          status: 'Occupied',
          funeralCaseId: 'C-1024',
          deceasedName: 'Johnathan Smith',
          createdAt: '2026-09-01T10:30:00Z',
          updatedAt: '2026-09-01T10:30:00Z',
        },
        {
          id: 2,
          slotNumber: 'M-002',
          date: '2026-09-07',
          startTime: '10:30',
          endTime: '12:30',
          status: 'Available',
          createdAt: '2026-09-01T10:35:00Z',
          updatedAt: '2026-09-01T10:35:00Z',
        },
        {
          id: 3,
          slotNumber: 'M-003',
          date: '2026-09-07',
          startTime: '14:00',
          endTime: '16:00',
          status: 'Reserved',
          funeralCaseId: 'C-1021',
          deceasedName: 'Mary Johnson',
          createdAt: '2026-09-01T10:40:00Z',
          updatedAt: '2026-09-01T10:40:00Z',
        },
      ];
    }
  }

  static async getRepatriationCases(filters: OperationFilters = {}): Promise<RepatriationCase[]> {
    try {
      const response = await apiClient.get('/operations/repatriation/cases', { params: filters });
      return response.data;
    } catch (error) {
      console.error('Failed to fetch repatriation cases:', error);
      // Return mock data as fallback for development
      return [
        {
          id: 'RP-00123',
          caseNumber: 'RP-00123',
          deceasedName: 'Robert Thompson',
          nationality: 'British',
          destinationCountry: 'United Kingdom',
          destinationCity: 'London',
          status: 'Documentation Complete',
          dateOfDeath: '2026-08-25',
          estimatedCompletionDate: '2026-09-10',
          assignedTo: 'Sarah J.',
          createdAt: '2026-08-26T09:00:00Z',
          updatedAt: '2026-09-01T14:30:00Z',
        },
        {
          id: 'RP-00124',
          caseNumber: 'RP-00124',
          deceasedName: 'Lisa Williams',
          nationality: 'American',
          destinationCountry: 'United States',
          destinationCity: 'New York',
          status: 'Pending Documentation',
          dateOfDeath: '2026-08-28',
          estimatedCompletionDate: '2026-09-15',
          assignedTo: 'Mike R.',
          createdAt: '2026-08-29T10:00:00Z',
          updatedAt: '2026-09-01T16:00:00Z',
        },
      ];
    }
  }

  static async reserveMortuarySlot(slotId: number, funeralCaseId: string): Promise<{ success: boolean }> {
    try {
      const response = await apiClient.post(`/operations/mortuary/slots/${slotId}/reserve`, { funeralCaseId });
      return response.data;
    } catch (error) {
      console.error(`Failed to reserve mortuary slot ${slotId}:`, error);
      throw error;
    }
  }

  static async updateRepatriationStatus(id: string, status: RepatriationCase['status']): Promise<RepatriationCase> {
    try {
      const response = await apiClient.patch(`/operations/repatriation/cases/${id}/status`, { status });
      return response.data;
    } catch (error) {
      console.error(`Failed to update repatriation case ${id} status:`, error);
      throw error;
    }
  }

  static async getOperationsDashboard(): Promise<{
    mortalityStats: {
      totalCases: number;
      pendingCases: number;
      completedCases: number;
      averageDuration: number;
    };
    fleetStats: {
      totalVehicles: number;
      availableVehicles: number;
      inMaintenance: number;
    };
    mortuaryStats: {
      totalSlots: number;
      availableSlots: number;
      occupiedSlots: number;
    };
  }> {
    try {
      const response = await apiClient.get('/operations/dashboard');
      return response.data;
    } catch (error) {
      console.error('Failed to fetch operations dashboard:', error);
      // Return mock data as fallback for development
      return {
        mortalityStats: {
          totalCases: 45,
          pendingCases: 8,
          completedCases: 37,
          averageDuration: 3.2,
        },
        fleetStats: {
          totalVehicles: 8,
          availableVehicles: 5,
          inMaintenance: 2,
        },
        mortuaryStats: {
          totalSlots: 12,
          availableSlots: 7,
          occupiedSlots: 5,
        },
      };
    }
  }
}