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

export interface Employee {
  id: number;
  employeeNumber: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  gender: 'Male' | 'Female' | 'Other';
  hireDate: string;
  position: string;
  department: string;
  branchId: number;
  employmentStatus: 'Active' | 'On Leave' | 'Terminated' | 'Suspended';
  salary: number;
  contactNumber?: string;
  email?: string;
  emergencyContactName?: string;
  emergencyContactNumber?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface EmployeeFilters {
  department?: string;
  position?: string;
  employmentStatus?: string;
  branchId?: number;
  search?: string;
}

export interface LeaveRequest {
  id: number;
  employeeId: number;
  leaveType: 'Annual' | 'Sick' | 'Family Responsibility' | 'Maternity' | 'Paternity' | 'Bereavement' | 'Unpaid';
  startDate: string;
  endDate: string;
  reason: string;
  status: 'Pending' | 'Approved' | 'Rejected';
  requestedDate: string;
  approvedBy?: number;
  approvedDate?: string;
}

export class HRService {
  static async getEmployees(filters: EmployeeFilters = {}): Promise<Employee[]> {
    try {
      const response = await apiClient.get('/hr/employees', { params: filters });
      return response.data;
    } catch (error) {
      console.error('Failed to fetch employees:', error);
      // Return mock data as fallback for development
      return [
        {
          id: 1,
          employeeNumber: 'EMP-001',
          firstName: 'Sarah',
          lastName: 'Johnson',
          dateOfBirth: '1985-03-15',
          gender: 'Female',
          hireDate: '2020-01-15',
          position: 'Funeral Director',
          department: 'Operations',
          branchId: 1,
          employmentStatus: 'Active',
          salary: 35000,
          contactNumber: '+27 82 111 2222',
          email: 'sarah.j@khet360.co.za',
          emergencyContactName: 'Mike Johnson',
          emergencyContactNumber: '+27 82 333 4444',
          createdAt: '2020-01-15T08:00:00Z',
          updatedAt: '2026-09-01T10:30:00Z',
        },
        {
          id: 2,
          employeeNumber: 'EMP-002',
          firstName: 'Mike',
          lastName: 'Roberts',
          dateOfBirth: '1980-07-22',
          gender: 'Male',
          hireDate: '2019-06-01',
          position: 'Operations Manager',
          department: 'Operations',
          branchId: 1,
          employmentStatus: 'Active',
          salary: 42000,
          contactNumber: '+27 71 333 4444',
          email: 'mike.r@khet360.co.za',
          emergencyContactName: 'Lisa Roberts',
          emergencyContactNumber: '+27 71 555 6666',
          createdAt: '2019-06-01T09:00:00Z',
          updatedAt: '2026-09-01T14:15:00Z',
        },
        {
          id: 3,
          employeeNumber: 'EMP-003',
          firstName: 'Grace',
          lastName: 'Khumbulani',
          dateOfBirth: '1992-11-08',
          gender: 'Female',
          hireDate: '2021-09-01',
          position: 'Administrator',
          department: 'Administration',
          branchId: 1,
          employmentStatus: 'Active',
          salary: 28000,
          contactNumber: '+27 72 777 8888',
          email: 'grace.k@khet360.co.za',
          emergencyContactName: 'Peter Khumbulani',
          emergencyContactNumber: '+27 72 999 0000',
          createdAt: '2021-09-01T10:00:00Z',
          updatedAt: '2026-09-01T16:45:00Z',
        },
      ];
    }
  }

  static async getEmployee(id: number): Promise<Employee> {
    try {
      const response = await apiClient.get(`/hr/employees/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Failed to fetch employee ${id}:`, error);
      throw error;
    }
  }

  static async createEmployee(employee: Omit<Employee, 'id' | 'createdAt' | 'updatedAt'>): Promise<Employee> {
    try {
      const response = await apiClient.post('/hr/employees', employee);
      return response.data;
    } catch (error) {
      console.error('Failed to create employee:', error);
      throw error;
    }
  }

  static async updateEmployee(id: number, employee: Partial<Employee>): Promise<Employee> {
    try {
      const response = await apiClient.put(`/hr/employees/${id}`, employee);
      return response.data;
    } catch (error) {
      console.error(`Failed to update employee ${id}:`, error);
      throw error;
    }
  }

  static async getLeaveRequests(employeeId?: number): Promise<LeaveRequest[]> {
    try {
      const params = employeeId ? { employeeId } : {};
      const response = await apiClient.get('/hr/leave-requests', { params });
      return response.data;
    } catch (error) {
      console.error('Failed to fetch leave requests:', error);
      // Return mock data as fallback for development
      return [
        {
          id: 1,
          employeeId: 1,
          leaveType: 'Annual',
          startDate: '2026-09-15',
          endDate: '2026-09-22',
          reason: 'Family vacation',
          status: 'Pending',
          requestedDate: '2026-09-01',
        },
        {
          id: 2,
          employeeId: 2,
          leaveType: 'Sick',
          startDate: '2026-09-05',
          endDate: '2026-09-07',
          reason: 'Medical appointment',
          status: 'Approved',
          requestedDate: '2026-09-03',
          approvedBy: 1,
          approvedDate: '2026-09-04',
        },
      ];
    }
  }

  static async createLeaveRequest(leaveRequest: Omit<LeaveRequest, 'id' | 'requestedDate'>): Promise<LeaveRequest> {
    try {
      const response = await apiClient.post('/hr/leave-requests', {
        ...leaveRequest,
        requestedDate: new Date().toISOString().split('T')[0]
      });
      return response.data;
    } catch (error) {
      console.error('Failed to create leave request:', error);
      throw error;
    }
  }
}