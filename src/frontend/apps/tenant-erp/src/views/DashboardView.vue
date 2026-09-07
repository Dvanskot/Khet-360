<template>
  <div class="dashboard-view">
    <div class="dashboard-header">
      <div class="header-left">
        <h1>Dashboard</h1>
        <p>Overview of your Khet-360 operations</p>
      </div>
      <div class="header-right">
        <ConnectionStatus />
      </div>
    </div>
    
    <div v-if="error" class="alert alert-error">
      {{ error }}
    </div>
    
    <div v-if="loading" class="loading-indicator">
      <div class="spinner"></div>
      <p>Loading dashboard data...</p>
    </div>
    
    <div v-else class="dashboard-grid">
      <!-- Stats Cards -->
      <div class="stat-card">
        <div class="stat-icon">👥</div>
        <div class="stat-content">
          <div class="stat-label">Active Cases</div>
          <div class="stat-value">{{ stats.activeCases }}</div>
          <div class="stat-change positive">+12% vs last month</div>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon">💰</div>
        <div class="stat-content">
          <div class="stat-label">Monthly Revenue</div>
          <div class="stat-value">R {{ stats.monthlyRevenue.toLocaleString() }}</div>
          <div class="stat-change positive">+8% vs last month</div>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon">🚐</div>
        <div class="stat-content">
          <div class="stat-label">Fleet Utilization</div>
          <div class="stat-value">{{ stats.fleetUtilization }}%</div>
          <div class="stat-change negative">-5% vs last month</div>
        </div>
      </div>
      
      <div class="stat-card">
        <div class="stat-icon">📋</div>
        <div class="stat-content">
          <div class="stat-label">Pending Tasks</div>
          <div class="stat-value">{{ stats.pendingTasks }}</div>
          <div class="stat-change positive">+3% vs last month</div>
        </div>
      </div>
      
      <!-- Recent Activity -->
      <div class="activity-section">
        <h2 class="section-title">Recent Activity</h2>
        <div class="activity-timeline">
          <div v-for="activity in recentActivities" :key="activity.id" class="activity-item">
            <div class="activity-icon">
              <span class="icon">{{ activity.icon }}</span>
            </div>
            <div class="activity-content">
              <h3 class="activity-title">{{ activity.title }}</h3>
              <p class="activity-description">{{ activity.description }}</p>
              <span class="activity-time">{{ formatTime(activity.timestamp) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmarked } from 'vue';
import { ConnectionStatus } from '@/components/ConnectionStatus.vue';

// Import services
import { signalRService } from '@/services/signalRService';
import { authService } from '@/services/authService';
import { notificationService } from '@/services/notificationService';

const stats = ref({
  activeCases: 0,
  monthlyRevenue: 0,
  fleetUtilization: 0,
  pendingTasks: 0
});

const recentActivities = ref([]);
const loading = ref(true);
const error = ref(null);

// Mock data fetching function (would be replaced with actual API calls)
const fetchDashboardData = async () => {
  try {
    loading.value = true;
    error.value = null;
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    // In a real app, this would be:
    // const response = await apiService.getDashboardStats();
    // stats.value = response.data;
    
    // Mock data for demonstration
    stats.value = {
      activeCases: 24,
      monthlyRevenue: 1250000,
      fleetUtilization: 78,
      pendingTasks: 15
    };
    
    recentActivities.value = [
      {
        id: 1,
        title: "New Case Created",
        description: "Case #C-1025 created for Johnson family",
        icon: "📋",
        timestamp: Date.now() - 300000 // 5 minutes ago
      },
      {
        id: 2,
        title: "Payment Received",
        description: "Payment of R 8,500 received for Case #C-1020",
        icon: "💰",
        timestamp: Date.now() - 1800000 // 30 minutes ago
      },
      {
        id: 3,
        title: "Vehicle Assigned",
        description: "Vehicle JKL-001 assigned to Case #C-1023",
        icon: "🚐",
        timestamp: Date.now() - 3600000 // 1 hour ago
      },
      {
        id: 4,
        title: "Document Uploaded",
        description: "Death certificate uploaded for Case #C-1021",
        icon: "📄",
        timestamp: Date.now() - 7200000 // 2 hours ago
      }
    ];
    
  } catch (err) {
    error.value = 'Failed to load dashboard data. Please try again later.';
    console.error('Error fetching dashboard data:', err);
  } finally {
    loading.value = false;
  }
};

// Real-time update handlers
const handleStatsUpdate = (newStats: any) => {
  stats.value = { ...stats.value, ...newStats };
  notificationService.addNotification({
    title: 'Dashboard Updated',
    message: 'Statistics have been updated in real-time',
    type: 'info'
  });
};

const handleActivityUpdate = (activity: any) => {
  // Add new activity to the beginning of the list
  recentActivities.value = [activity, ...recentActivities.value.slice(0, 9)]; // Keep only latest 10
  notificationService.addNotification({
    title: 'New Activity',
    message: activity.title,
    type: 'success'
  });
};

onMounted(() => {
  // Initialize data
  fetchDashboardData();
  
  // Set up SignalR listeners for real-time updates
  signalRService.on('DashboardStatsUpdated', handleStatsUpdate);
  signalRService.on('NewActivityAdded', handleActivityUpdate);
  
  // Start SignalR connection
  signalRService.start().catch(err => {
    console.error('Failed to start SignalR connection:', err);
    notificationService.addNotification({
      title: 'Connection Issue',
      message: 'Unable to connect to real-time updates. Some features may not be live.',
      type: 'warning'
    });
  });
  
  // Refresh data periodically (in case SignalR misses something)
  setInterval(fetchDashboardData, 60000); // Every minute
});

onBeforeUnmount(() => {
  // Clean up SignalR listeners
  signalRService.off('DashboardStatsUpdated', handleStatsUpdate);
  signalRService.off('NewActivityAdded', handleActivityUpdate);
  
  // Stop SignalR connection
  signalRService.stop();
});

const formatTime = (timestamp: number): string => {
  return new Date(timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
};
</script>

<script setup lang="ts">
</script>

<style scoped>
.dashboard-view {
  padding: 2rem;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.header-left h1 {
  font-size: 2rem;
  font-weight: 600;
}

.header-left p {
  color: #6b7280;
  font-size: 1.1rem;
}

.header-right {
  display: flex;
  align-items: center;
}

.alert {
  padding: 1rem;
  border-radius: 0.375rem;
  margin-bottom: 1.5rem;
}

.alert-error {
  background-color: #fee2e2;
  border: 1px solid #fecaca;
  color: #991b1b;
}

.loading-indicator {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  color: #6b7280;
}

.spinner {
  width: 3rem;
  height: 3rem;
  border: 3px solid #d1d5db;
  border-top-color: #3b82f6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.stat-card {
  background-color: white;
  border-radius: 0.5rem;
  padding: 1.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1), 0 1px 2px -1px rgba(0, 0, 0, 0.1);
  display: flex;
  align-items: center;
  gap: 1rem;
}

.stat-icon {
  font-size: 2rem;
  width: 3rem;
  height: 3rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f8fafc;
  border-radius: 0.375rem;
}

.stat-content {
  flex: 1;
}

.stat-label {
  font-size: 0.875rem;
  color: #6b7280;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: 600;
  color: #1f2937;
}

.stat-change {
  font-size: 0.875rem;
  font-weight: 500;
}

.stat-change.positive {
  color: #10b981;
}

.stat-change.negative {
  color: #ef4444;
}

.activity-section {
  margin-bottom: 2rem;
}

.section-title {
  font-size: 1.5rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: #1f2937;
}

.activity-timeline {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.activity-item {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  padding: 1rem;
  background-color: #f8fafc;
  border-radius: 0.375rem;
  border: 1px solid #e5e7eb;
}

.activity-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 3rem;
  height: 3rem;
  background-color: #e0f2fe;
  border-radius: 0.375rem;
}

.activity-icon .icon {
  font-size: 1.25rem;
}

.activity-content {
  flex: 1;
}

.activity-title {
  font-size: 1rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.activity-description {
  color: #6b7280;
  font-size: 0.875rem;
  margin-bottom: 0.5rem;
}

.activity-time {
  font-size: 0.75rem;
  color: #9ca3af;
}
</style>