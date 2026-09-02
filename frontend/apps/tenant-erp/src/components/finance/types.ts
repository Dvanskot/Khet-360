export type AccountType = 'Asset' | 'Liability' | 'Equity' | 'Revenue' | 'Expense';
export type TransactionType = 'Debit' | 'Credit';

export interface Account {
  id: string;
  code: string;
  name: string;
  type: AccountType;
  balance: number;
  branchId: string;
}

export interface JournalEntry {
  id: string;
  date: string;
  description: string;
  reference: string;
  totalAmount: number;
  lines: JournalLine[];
}

export interface JournalLine {
  id: string;
  accountId: string;
  accountName: string;
  type: TransactionType;
  amount: number;
  description?: string;
}

export interface TaxReport {
  period: string;
  payeAmount: number;
  uifAmount: number;
  sdlAmount: number;
  totalDue: number;
  status: 'Draft' | 'Submitted' | 'Paid';
}
