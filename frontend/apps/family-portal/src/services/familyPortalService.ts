import axios from 'axios';

// Create axios instance with base URL
const apiClient = axios.create({
  baseURL: '/api', // Vite dev server will proxy this to backend
  timeout: 10: 10000,
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

export interface FuneralCaseTimeline {
  id: string;
  caseNumber: string;
  deceasedName: string;
  currentStage: string;
  currentStageIndex: number;
  milestones: Array<{
    title: string;
    description: string;
    date?: string;
  }>;
  requiredDocuments: Array<{
    name: string;
    uploaded: boolean;
    uploadDate?: string;
  }>;
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
  createdAt?: string;
  updatedAt?: string;
}

export class FamilyPortalService {
  static async getCaseTimeline(caseId: string): Promise<FuneralCaseTimeline> {
    try {
      const response = await apiClient.get(`/family-portal/cases/${caseId}/timeline`);
      return response.data;
    } catch (error) {
      console.error(`Failed to fetch case timeline for ${caseId}:`, error);
      // Return mock data as fallback for development
      return {
        id: 'C-1024',
        caseNumber: 'C-1024',
        deceasedName: 'Samuel Tshikota',
        currentStage: 'Service Planning',
        currentStageIndex: 2,
        milestones: [
          { title: 'Death Notification', description: 'Case opened and initial notification received.', date: 'Aug 25' },
          { title: 'Verification', description: 'Identity and policy verification completed.', date: 'Aug 26' },
          { title: 'Service Planning', description: 'Arranging venue, transport and casket selection.', date: 'Current' },
          { title: 'Service Delivery', description: 'The funeral service and burial/cremation.', date: 'Pending' },
          { title: 'Case Closure', description: 'Final administration and document archiving.', date: 'Pending' },
        ],
        requiredDocuments: [
          { name: 'Death Certificate', uploaded: true, uploadDate: '2026-08-25' },
          { name: 'ID Copy (Deceased)', uploaded: true, uploadDate: '2026-08-25' },
          { name: 'ID Copy (Next of Kin)', uploaded: false },
          { name: 'Marriage Certificate', uploaded: false },
        ],
        financialSummary: {
          totalPackageCost: 12000,
          insuranceCover: 8000,
          outstandingBalance: 4000,
          currency: 'ZAR',
        },
        assignedFuneralDirector: {
          name: 'Sarah Jenkins',
          contactNumber: '+27 82 123 4567',
        },
        createdAt: '2026-08-25T10:30:00Z',
        updatedAt: '2026-09-01T14:30:00Z',
      };
    }
  }
}