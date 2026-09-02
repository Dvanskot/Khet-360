export type VehicleStatus = 'Available' | 'InTransit' | 'Maintenance' | 'OutofService';

export interface Vehicle {
  id: string;
  registration: string;
  type: 'Hearse' | 'Limousine' | 'Family Car' | 'Support Vehicle';
  status: VehicleStatus;
  currentDriverId?: string;
  branchId: string;
  lastInspectionDate: string;
  mileage: number;
}

export interface DispatchTask {
  id: string;
  caseId: string;
  vehicleId: string;
  driverId: string;
  origin: string;
  destination: string;
  status: 'Scheduled' | 'Dispatched' | 'Completed' | 'Cancelled';
  scheduledTime: string;
  actualTime?: string;
}
