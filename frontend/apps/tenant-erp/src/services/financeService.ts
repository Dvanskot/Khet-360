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

export interface FinancialTransaction {
  id: number;
  transactionNumber: string;
  date: string;
  description: string;
  debitAmount: number;
  creditAmount: number;
  accountId: number;
  accountName: string;
  referenceNumber?: string;
  status: 'Pending' | 'Posted' | 'Reversed';
  createdBy?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface Account {
  id: number;
  accountNumber: string;
  accountName: string;
  accountType: 'Asset' | 'Liability' | 'Equity' | 'Revenue' | 'Expense';
  balance: number;
  currency: string;
  isActive: boolean;
}

export interface FinancialFilters {
  accountId?: number;
  dateFrom?: string;
  dateTo?: string;
  transactionType?: 'Debit' | 'Credit';
  status?: 'Pending' | 'Posted' | 'Reversed';
  search?: string;
}

export class FinanceService {
  static async getTransactions(filters: FinancialFilters = {}): Promise<FinancialTransaction[]> {
    try {
      const response = await apiClient.get('/finance/transactions', { params: filters });
      return response.data;
    } catch (error) {
      console.error('Failed to fetch financial transactions:', error);
      // Return mock data as fallback for development
      return [
        {
          id: 1,
          transactionNumber: 'TXN-001234',
          date: '2026-09-01',
          description: 'Funeral service payment - Smith Family',
          debitAmount: 0,
          creditAmount: 5500,
          accountId: 1001,
          accountName: 'Bank Account',
          referenceNumber: 'REF-2026-001',
          status: 'Posted',
          createdBy: 'Sarah J.',
          createdAt: '2026-09-01T10:30:00Z',
        },
        {
          id: 2,
          transactionNumber: 'TXN-001235',
          date: '2026-09-02',
          description: 'Casket purchase - Oak Standard',
          debitAmount: 2500,
          creditAmount: 0,
          accountId: 2001,
          accountName: 'Inventory',
          referenceNumber: 'INV-2026-045',
          status: 'Posted',
          createdBy: 'Mike R.',
          createdAt: '2026-09-02T14:15:00Z',
        },
        {
          id: 3,
          transactionNumber: 'TXN-001236',
          date: '2026-09-03',
          description: 'Florist payment - Fresh flowers',
          debitAmount: 120,
          creditAmount: 0,
          accountId: 3001,
          accountName: 'Expense Account',
          referenceNumber: 'EXP-2026-012',
          status: 'Posted',
          createdBy: 'Sarah J.',
          createdAt: '2026-09-03T09:00:00Z',
        },
      ];
    }
  }

  static async getAccounts(): Promise<Account[]> {
    try {
      const response = await apiClient.get('/finance/accounts');
      return response.data;
    } catch (error) {
      console.error('Failed to fetch accounts:', error);
      // Return mock data as fallback for development
      return [
        {
          id: 1001,
          accountNumber: '1000',
          accountName: 'Bank Account - Main',
          accountType: 'Asset',
          balance: 45000,
          currency: 'ZAR',
          isActive: true,
        },
        {
          id: 1002,
          accountNumber: '2000',
          accountName: 'Inventory - Caskets',
          accountType: 'Asset',
          balance: 75000,
          currency: 'ZAR',
          isActive: true,
        },
        {
          id: 3001,
          accountNumber: '5000',
          accountName: 'Funeral Service Revenue',
          accountType: 'Revenue',
          balance: 120000,
          currency: 'ZAR',
          isActive: true,
        },
      ];
    }
  }

  static async createTransaction(transaction: Omit<FinancialTransaction, 'id' | 'createdAt' | 'updatedAt'>): Promise<FinancialTransaction> {
    try {
      const response = await apiClient.post('/finance/transactions', transaction);
      return response.data;
    } catch (error) {
      console.error('Failed to create financial transaction:', error);
      throw error;
    }
  }

  static async getFinancialReports(reportType: string, params: Record<string, any> = {}): Promise<any> {
    try {
      const response = await apiClient.get(`/finance/reports/${reportType}`, { params });
      return response.data;
    } catch (error) {
      console.error(`Failed to fetch financial report ${reportType}:`, error);
      throw error;
    }
  }
}