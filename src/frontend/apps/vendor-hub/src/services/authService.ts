// Authentication Service
// Handles JWT token management, refresh, and authentication state

import { ApiResponse } from '@/khet360/api-client';

export class AuthService {
  private static readonly TOKEN_KEY = 'access_token';
  private static readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private static readonly TOKEN_EXPIRY_KEY = 'token_expiry';
  private static readonly REFRESH_THRESHOLD = 30000; // 30 seconds before expiry

  static getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  static setToken(token: string, expiresIn: number, refreshToken?: string): void {
    const expiryTime = Date.now() + (expiresIn * 1000);
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.TOKEN_EXPIRY_KEY, expiryTime.toString());
    
    if (refreshToken) {
      localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
    }
    
    // Set up auto-refresh
    this.setupAutoRefresh(expiresIn * 1000);
  }

  static clearAuth(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.TOKEN_EXPIRY_KEY);
  }

  static isAuthenticated(): boolean {
    const token = this.getToken();
    const expiry = localStorage.getItem(this.TOKEN_EXPIRY_KEY);
    return !!token && !!expiry && Date.now() < parseInt(expiry, 10);
  }

  static getAuthHeaders(): Record<string, string> {
    const token = this.getToken();
    return token ? { Authorization: `Bearer ${token}` } : {};
  }

  private static setupAutoRefresh(delay: number): void {
    // Clear any existing timeout
    if (this.refreshTimeoutId) {
      clearTimeout(this.refreshTimeoutId);
    }
    
    // Set new refresh timer to trigger slightly before actual expiry
    this.refreshTimeoutId = setTimeout(async () => {
      try {
        await this.refreshToken();
      } catch (error) {
        console.error('Failed to refresh token:', error);
        window.location.href = '/login';
      }
    }, delay - this.REFRESH_THRESHOLD);
  }

  private static refreshTimeoutId: number | null = null;

  static async refreshToken(): Promise<void> {
    const refreshToken = localStorage.getItem(this.REFRESH_TOKEN_KEY);
    if (!refreshToken) throw new Error('No refresh token available');
    
    try {
      const response = await fetch('/api/auth/refresh-token', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken })
      });
      
      if (!response.ok) throw new Error('Token refresh failed');
      
      const data = await response.json();
      this.setToken(data.accessToken, data.expiresIn, data.refreshToken);
    } catch (error) {
      throw error;
    }
  }

  static async login(username: string, password: string): Promise<ApiResponse<any>> {
    try {
      const response = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password })
      });
      
      const data = await response.json();
      
      if (response.ok) {
        this.setToken(data.accessToken, data.expiresIn, data.refreshToken);
        return ApiResponse.Ok(data, 'Login successful');
      } else {
        return ApiResponse.Fail(data.message || 'Login failed', data.errors);
      }
    } catch (error) {
      return ApiResponse.Fail('Network error during login', [error.message]);
    }
  }

  static logout(): void {
    this.clearAuth();
    window.location.href = '/login';
  }

  static hasRole(role: string): boolean {
    // Placeholder implementation - in real app, decode JWT to check roles
    return true;
  }

  static hasPermission(permission: string): boolean {
    // Placeholder implementation - in real app, decode JWT to check permissions
    return true;
  }
}

// Export singleton instance
export const authService = new AuthService();