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
    config.headers.Authorization = {...config};
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

export interface Vehicle {
  id: number;
  registrationNumber: string;
  make: string;
  model: string;
  year: number;
  vehicleType: 'Hearse' | 'Funeral Coach' | 'Removal Vehicle' | 'Service Vehicle';
  capacity: number;
  fuelType: 'Diesel' | 'Petrol' | 'Electric' | 'Hybrid';
  status: 'Available' | 'In Use' | 'Maintenance' | 'Out of Service';
  lastServiceDate?: string;
  nextServiceDate?: string;
  currentMileage: number;
  assignedTo?: number;
  createdAt?: string;
  updatedAt?: string;
}

export interface Trip {
  id: number;
  tripNumber: string;
  vehicleId: number;
  driverId: number;
  funeralCaseId?: string;
  origin: string;
  destination: string;
  departureTime: string;
  arrivalTime?: string;
  status: 'Planned' | 'In Transit' | 'Completed' | 'Cancelled';
  distanceKm: number;
  estimatedDuration: string;
  actualDuration?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface FleetFilters {
  vehicleType?: string;
  status?: string;
  branchId?: number;
  search?: string;
}

export class FleetService {
  static async getVehicles(filters: FleetFilters = {}): Promise<Vehicle[]> {
    try {
      const response = await apiClient.get('/fleet/vehicles', { params: filters });
      return response.data;
    } catch (error) {
      console.error('Failed to fetch vehicles:', error);
      // Return mock data as fallback for development
      return [
        {
          id: 1,
          registrationNumber: 'JKL-001-GP',
          make: 'Mercedes-Benz',
          model: 'Sprinter',
          year: 2022,
          vehicleType: 'Funeral Coach',
          capacity: 8,
          fuelType: 'Diesel',
          status: 'Available',
          lastServiceDate: '2026-08-15',
          nextServiceDate: '2026-11-15',
          currentMileage: 45000,
          assignedTo: null,
          createdAt: '2022-03-10T08:00:00Z',
          updatedAt: '2026-09-01T10:30:00Z',
        },
        {
          id: 2,
          registrationNumber: 'JKL-002-GP',
          make: 'Toyota',
          model: 'Hilux',
          year: 2021,
          vehicleType: 'Removal Vehicle',
          capacity: 3,
          fuelType: 'Diesel',
          status: 'In Use',
          lastServiceDate: '2026-08-20',
          nextServiceDate: '2026-11-20',
          currentMileage: 38000,
          assignedTo: 2,
          createdAt: '2021-06-15T09:00:00Z',
          updatedAt: '2026-09-01T14:15:00Z',
        },
        {
          id: 3,
          registrationNumber: 'JKL-003-GP',
          make: 'Ford',
          model: 'Transit',
          year: 2020,
          vehicleType: 'Service Vehicle',
          capacity: 6,
          fuelType: 'Petrol',
          status: 'Maintenance',
          lastServiceDate: '2026-07-01',
          nextServiceDate: '2026-10-01',
          currentMileage: 52000,
          assignedTo: null,
          createdAt: '2020-01-20T10:00:00Z',
          updatedAt: '2026-09-01T16:45:00Z',
        },
      ];
    }
  }

  static async getVehicle(id: number): Promise<Vehicle> {
    try {
      const response = await apiClient.get(`/fleet/vehicles/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Failed to fetch vehicle ${id}:`, error);
      throw error;
    }
  }

  static async createVehicle(vehicle: Omit<Vehicle, 'id' | 'createdAt' | 'updatedAt'>): Promise<Vehicle> {
    try {
      const response = await apiClient.post('/fleet/vehicles', vehicle);
      return response.data;
    } catch (error) {
      console.error('Failed to create vehicle:', error);
      throw error;
    }
  }

  static async updateVehicle(id: number, vehicle: Partial<Vehicle>): Promise<Vehicle> {
    try {
      const response = await apiClient.put(`/fleet/vehicles/${id}`, vehicle);
      return response.data;
    } catch (error) {
      console.error(`Failed to update vehicle ${id}:`, error);
      throw error;
    }
  }

  static async getTrips(filters: Partial<FleetFilters> = {}): Promise<Trip[]> {
    try {
      const response = await apiClient.get('/fleet/trips', { params: filters });
      return response.data;
    } catch (error) {
      console.error('Failed to fetch trips:', error);
      // Return mock data as fallback for development
      return [
        {
          id: 1,
          tripNumber: 'TRP-00123',
          vehicleId: 1,
          driverId: 2,
          funeralCaseId: 'C-1024',
          origin: 'Funeral Home',
          destination: 'Evergreen Cemetery',
          departureTime: '2026-09-07T10:00:00',
          arrivalTime: '2026-09-07T11:30:00',
          status: 'Completed',
          distanceKm: 25.5,
          estimatedDuration: '1h 30m',
          actualDuration: '1h 25m',
          createdAt: '2026-09-01T08:00:00Z',
          updatedAt: '2026-09-07T11:45:00Z',
        },
        {
          id: 2,
          tripNumber: 'TRP-00124',
          vehicleId: 2,
          driverId: 2,
          funeralCaseId: 'C-1021',
          origin: 'Private Residence',
          destination: 'Funeral Home',
          departureTime: '2026-09-07T08:00:00',
          arrivalTime: '2026-09-07T09:15:00',
          status: 'Completed',
          distanceKm: 12.3,
          estimatedDuration: '1h 15m',
          actualDuration: '1h 10m',
          createdAt: '2026-09-01T09:00:00Z',
          updatedAt: '2026-09-07T09:30:00Z',
        },
      ];
    }
  }

  static async updateVehicleStatus(id: number, status: Vehicle['status']): Promise<Vehicle> {
    try {
      const response = await apiClient.patch(`/fleet/vehicles/${id}/status`, { status });
      return response.data;
    } catch (error) {
      console.error(`Failed to update vehicle ${id} status:`, error);
      throw error;
    }
  }

  static async assignVehicleToTrip(vehicleId: number, tripId: number): Promise<{ success: boolean }> {
    try {
      const response = await apiClient.post(`/fleet/vehicles/${vehicleId}/assign-to-trip/${tripId}`);
      return response.data;
    } catch (error) {
      console.error(`Failed to assign vehicle ${vehicleId} to trip ${tripId}:`, error);
      throw error;
    }
  }
}