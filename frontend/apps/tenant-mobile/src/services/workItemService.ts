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

export interface WorkItem {
  id: number;
  title: string;
  description: string;
  status: 'Pending' | 'In Progress' | 'Blocked' | 'Completed';
  due: string;
  caseId: string;
  priority: 'Low' | 'Medium' | 'High';
}

export class MobileWorkItemService {
  static async getWorkItems(): Promise<WorkItem[]> {
    try {
      const response = await apiClient.get('/work-items');
      return response.data;
    } catch (error) {
      console.error('Failed to fetch work items:', error);
      // Return mock data as fallback for development
      return [
        { id: 1, title: 'Verify Death Certificate', description: 'Verify the uploaded death certificate for Case #C-1024', status: 'Pending', due: 'Today', caseId: 'C-1024', priority: 'High' },
        { id: 2, title: 'Schedule Burial Service', description: 'Coordinate with venue and transport for Case #C-1021', status: 'In Progress', due: 'Tomorrow', caseId: 'C-1021', priority: 'Medium' },
        { id: 3, title: 'Process Claim Payout', description: 'Verify benefits and initiate payout for Case #C-1018', status: 'Blocked', due: '2 days', caseId: 'C-1018', priority: 'High' },
      ];
    }
  }

  static async completeWorkItem(id: number): Promise<void> {
    try {
      await apiClient.post(`/work-items/${id}/complete`);
    } catch (error) {
      console.error(`Failed to complete work item ${id}:`, error);
      throw error;
    }
  }
}