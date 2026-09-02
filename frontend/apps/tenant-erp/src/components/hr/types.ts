export type EmploymentStatus = 'Full-Time' | 'Part-Time' | 'Contract' | 'Casual';
export type LeaveType = 'Annual' | 'Sick' | 'Family Responsibility' | 'Maternity/Paternity' | 'Unpaid';

export interface Employee {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  departmentId: string;
  positionId: string;
  branchId: string;
  status: EmploymentStatus;
  hireDate: string;
  salary: number;
  taxNumber: string;
  uifNumber: string;
}

export interface LeaveApplication {
  id: string;
  employeeId: string;
  type: LeaveType;
  startDate: string;
  endDate: string;
  reason: string;
  status: 'Pending' | 'Approved' | 'Rejected';
  approvedBy?: string;
}

export interface PayrollRun {
  id: string;
  period: string; // e.g. "August 2026"
  status: 'Draft' | 'Review' | 'Finalized' | 'Paid';
  totalGross: number;
  totalDeductions: number;
  totalNet: number;
  processedDate?: string;
}
