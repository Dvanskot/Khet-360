export type CaseStatus = 'Draft' | 'Open' | 'Arranging' | 'Confirmed' | 'InService' | 'Completed' | 'Closed';

export interface FuneralCase {
  id: string;
  caseNumber: string;
  deceasedName: string;
  status: CaseStatus;
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  nextAction: string;
  branchId: string;
  createdDate: string;
  lastUpdatedDate: string;
  readinessScore: number; // 0 to 100
  missingDocs: string[];
}

export interface WorkflowStage {
  id: CaseStatus;
  label: string;
  color: string;
}

export interface ArrangementStep {
  id: string;
  label: string;
  component: string;
  isComplete: boolean;
}
