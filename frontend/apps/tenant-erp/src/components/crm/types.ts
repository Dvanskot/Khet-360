export type CustomerType = 'Individual' | 'Organisation';
export type RelationshipType = 'Main Member' | 'Spouse' | 'Child' | 'Parent' | 'Extended Family' | 'Beneficiary' | 'Next of Kin';

export interface Customer {
  id: string;
  fullName: string;
  type: CustomerType;
  email: string;
  phone: string;
  address: string;
  idNumber: string;
  status: 'Active' | 'Inactive' | 'Prospect';
  createdDate: string;
  totalBalance: number;
}

export interface FamilyMember {
  id: string;
  customerId: string;
  fullName: string;
  relationship: RelationshipType;
  age: number;
  status: 'Alive' | 'Deceased';
  isPolicyMember: boolean;
}

export interface Lead {
  id: string;
  source: 'Website' | 'Referral' | 'Walk-in' | 'Phone';
  customerName: string;
  phone: string;
  email: string;
  interest: string;
  status: 'New' | 'Contacted' | 'Qualified' | 'Converted' | 'Lost';
  assignedTo: string;
  createdDate: string;
  lastContactDate?: string;
  priority: 'Low' | 'Medium' | 'High';
}
