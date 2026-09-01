export enum LeadStatus {
  New = 'New',
  Contacted = 'Contacted',
  Qualified = 'Qualified',
  Converted = 'Converted',
  Lost = 'Lost',
}

export interface LeadDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  source: string;
  notes: string;
  status: LeadStatus;
  createdAt: string;
  branchId: string;
}
