// Analytics Service
// Provides data for analytics and reporting dashboards

import { apiClient } from '@/khet360/api-client';

export class AnalyticsService {
  private static readonly BASE_PATH = '/api/analytics';

  /**
   * Get dashboard statistics
   */
  static async getDashboardStats(): Promise<any> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/dashboard`);
      return response.data;
    } catch (error) {
      throw new Error(`Failed to fetch dashboard statistics: ${error.message}`);
    }
  }

  /**
   * Get financial analytics
   */
  static async getFinancialAnalytics(
    startDate?: string,
    endDate?: string
  ): Promise<any> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/financial`, {
        params: {
          startDate,
          endDate
        }
      });
      return response.data;
    } catch (error) {
      throw new Error(`Failed to fetch financial analytics: ${error.message}`);
    }
  }

  /**
   * Get operational analytics
   */
  static async getOperationalAnalytics(
    startDate?: string,
    endDate?: string
  ): Promise<any> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/operational`, {
        params: {
          startDate,
          endDate
        }
      });
      return response.data;
    } catch (error) {
      throw new Error(`Failed to fetch operational analytics: ${error.message}`);
    }
  }

  /**
   * Get work item trends
   */
  static async getWorkItemTrends(
    period: string = 'monthly',
    monthsBack: number = 6
  ): Promise<any> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/work-item-trends`, {
        params: {
          period,
          monthsBack
        }
      });
      return response.data;
    } catch (error) {
      throw new Error(`Failed to fetch work item trends: ${error.message}`);
    }
  }

  /**
   * Get lead conversion analytics
   */
  static async getLeadConversionAnalytics(
    startDate?: string,
    endDate?: string
  ): Promise<any> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/lead-conversion`, {
        params: {
          startDate,
          endDate
        }
      });
      return response.data;
    } catch (error) {
      throw new Error(`Failed to fetch lead conversion analytics: ${error.message}`);
    }
  }

  /**
   * Get monthly report data
   */
  static async getMonthlyReport(
    year: number,
    month: number
  ): Promise<any> {
    try {
      const response = await apiClient.get(`${this.BASE_PATH}/monthly-report`, {
        params: {
          year,
          month
        }
      });
      return response.data;
    } catch (error) {
      throw new Error(`Failed to fetch monthly report: ${error.message}`);
    }
  }
}

// Export singleton instance
export const analyticsService = new AnalyticsService();