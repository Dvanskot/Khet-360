<template>
  <div class="analytics-dashboard">
    <div class="dashboard-header">
      <h1 class="title">Field Operations Analytics</h1>
      <div class="header-controls">
        <ConnectionStatus />
        <KButton @click="refreshData" variant="outline" size="sm">
          Refresh
        </KButton>
      </div>
    </div>

    <div v-if="loading" class="loading-container">
      <div class="spinner"></div>
      <p>Loading analytics data...</p>
    </div>

    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-else class="analytics-grid">
      <!-- Work Completion Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Work Completion</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Tasks Completed Today</div>
            <div class="metric-value">{{ workStats.tasksCompletedToday }}</div>
            <div class="metric-change positive">+{{ workStats.tasksChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Average Response Time</div>
            <div class="metric-value">{{ workStats.avgResponseTime }} min</div>
            <div class="metric-change negative">-{{ workStats.responseTimeChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">On-Time Completion</div>
            <div class="metric-value">{{ workStats.onTimeRate }}%</div>
            <div class="metric-change positive">+{{ workStats.onTimeChange }}%</div>
          </div>
        </div>
      </div>

      <!-- Travel Efficiency Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Travel Efficiency</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Average Daily Mileage</div>
            <div class="metric-value">{{ travelStats.avgMileage }} miles</div>
            <div class="metric-change positive">+{{ travelStats.mileageChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Fuel Efficiency</div>
            <div class="metric-value">{{ travelStats.fuelEfficiency }} MPG</div>
            <div class="metric-change positive">+{{ travelStats.fuelChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Travel Time Savings</div>
            <div class="metric-value">{{ travelStats.timeSaved }} hrs/week</div>
            <div class="metric-change positive">+{{ travelStats.timeSavedChange }}%</div>
          </div>
        </div>
      </div>

      <!-- Client Satisfaction Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Client Satisfaction</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Satisfaction Score</div>
            <div class="metric-value">{{ satisfactionStats.score }}/10</div>
            <div class="metric-change positive">+{{ satisfactionStats.change }}</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Referral Rate</div>
            <div class="metric-value">{{ satisfactionStats.referralRate }}%</div>
            <div class="metric-change positive">+{{ satisfactionStats.referralChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Repeat Business</div>
            <div class="metric-value">{{ satisfactionStats.repeatRate }}%</div>
            <div class="metric-change positive">+{{ satisfactionStats.repeatChange }}%</div>
          </div>
        </div>
      </div>

      <!-- Inventory Status Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Inventory Status</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Items in Stock</div>
            <div class="metric-value">{{ inventoryStats.totalItems }}</div>
            <div class="metric-change positive">+{{ inventoryStats.stockChange }}</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Low Stock Items</div>
            <div class="metric-value">{{ inventoryStats.lowStockCount }}</div>
            <div class="metric-change negative">+{{ inventoryStats.lowStockChange }}</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Items Ordered This Week</div>
            <div class="metric-value">{{ inventoryStats.orderedThisWeek }}</div>
            <div class="metric-change positive">+{{ inventoryStats.orderedChange }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue';
import { KButton } from '@khet360/ui-shared';
import { ConnectionStatus } from '@/components/ConnectionStatus.vue';

// Mock data for mobile analytics (in a real app, this would come from analytics service)
const workStats = ref({
  tasksCompletedToday: 12,
  tasksChange: 15,
  avgResponseTime: 25,
  responseTimeChange: -10,
  onTimeRate: 92,
  onTimeChange: 5
});

const travelStats = ref({
  avgMileage: 85,
  mileageChange: 8,
  fuelEfficiency: 22,
  fuelChange: 3,
  timeSaved: 6.5,
  timeSavedChange: 12
});

const satisfactionStats = ref({
  score: 9.1,
  change: 0.4,
  referralRate: 78,
  referralChange: 10,
  repeatRate: 85,
  repeatChange: 3
});

const inventoryStats = ref({
  totalItems: 1245,
  stockChange: 45,
  lowStockCount: 23,
  lowStockChange: -8,
  orderedThisWeek: 89,
  orderedChange: 22
});

const loading = ref<boolean>(false);
const error = ref<string | null>(null);

const refreshData = async () => {
  // In a real implementation, this would call analytics service
  // For now, we'll just update with slight random variations
  loading.value = true;
  
  // Simulate data updates
  workStats.value.tasksCompletedToday += Math.floor(Math.random() * 3) - 1;
  travelStats.value.avgMileage += Math.random() * 2 - 1;
  satisfactionStats.value.score += (Math.random() * 0.2 - 0.1);
  inventoryStats.value.totalItems += Math.floor(Math.random() * 5) - 2;
  
  // Keep values in reasonable ranges
  workStats.value.tasksCompletedToday = Math.max(0, workStats.value.tasksCompletedToday);
  travelStats.value.avgMileage = Math.max(0, travelStats.value.avgMileage);
  satisfactionStats.value.score = Math.max(0, Math.min(10, satisfactionStats.value.score));
  inventoryStats.value.totalItems = Math.max(0, inventoryStats.value.totalItems);
  
  loading.value = false;
};

onMounted(() => {
  // Load initial data
  refreshData();
  
  // Refresh every 2 minutes for mobile analytics
  setInterval(refreshData, 120000);
});
</script>

<script setup lang="ts">
</script>

<style scoped>
.analytics-dashboard {
  padding: 1.5rem;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.dashboard-header h1 {
  font-size: 1.5rem;
  font-weight: 600;
  margin: 0;
}

.header-controls {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.header-controls .connection-status {
  margin-left: auto;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  color: #6b7280;
}

.spinner {
  width: 24px;
  height: 24px;
  border: 2px solid var(--khet-border);
  border-top-color: var(--khet-primary);
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin-bottom: 0.5rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.error-message {
  padding: 1rem;
  background-color: #f8d7da;
  color: #721c24;
  border-radius: var(--khet-radius-md);
  border: 1px solid #f5c6cb;
}

.analytics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
}

.analytics-card {
  background: var(--khet-surface);
  border-radius: var(--khet-radius);
  padding: 1.5rem;
  box-shadow: 0 4px 6px rgba(0,0,0,0.05);
  border: 1px solid var(--khet-border);
  transition: transform 0.2s;
}

.analytics-card:hover {
  transform: translateY(-2px);
}

.card-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.card-header h2 {
  font-size: 1.1rem;
  font-weight: 600;
  margin: 0;
}

.card-content {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.metric-row {
  display: flex;
  justify-content: space-between;
  padding: 0.75rem 0;
  border-bottom: 1px solid #f3f4f6;
}

.metric-row:last-child {
  border-bottom: none;
}

.metric-label {
  font-size: 0.875rem;
  color: var(--khet-text-muted);
}

.metric-value {
  font-size: 1rem;
  font-weight: 600;
  color: var(--khet-text-main);
}

.metric-change {
  font-size: 0.75rem;
  font-weight: 500;
}

.metric-change.positive {
  color: #10b981;
}

.metric-change.negative {
  color: #ef4444;
}

/* Responsive design */
@media (max-width: 480px) {
  .analytics-dashboard {
    padding: 1rem;
  }
  
  .dashboard-header {
    flex-direction: column;
    align-items: start;
    gap: 1rem;
  }
  
  .analytics-grid {
    grid-template-columns: 1fr;
  }
  
  .header-controls {
    width: 100%;
  }
}
</style>
