export enum WorkItemPriority {
  Low = 0,
  Medium = 1,
  High = 2,
  Critical = 3,
}

export enum WorkItemStatus {
  Pending = 0,
  InProgress = 1,
  Blocked = 2,
  Completed = 3,
  Cancelled = 4,
}

export enum SlaStatus {
  OnTrack = 0,
  Warning = 1,
  Breached = 2,
}

export interface WorkItem {
  id: string;
  sourceEntityType: string;
  sourceEntityId: string;
  ownerId: string | null;
  priority: WorkItemPriority;
  dueDate: string;
  status: WorkItemStatus;
  currentState: string | null;
  nextAction: string | null;
  slaStatus: SlaStatus;
  branchId: string;
}
