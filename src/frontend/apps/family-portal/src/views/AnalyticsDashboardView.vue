<template>
  <div class="analytics-dashboard">
    <div class="dashboard-header">
      <h1 class="title">Family Memorial Analytics</h1>
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
      <!-- Service Preferences Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Service Preferences</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Traditional Services</div>
            <div class="metric-value">{{ servicePrefs.traditional }}%</div>
            <div class="metric-change negative">-{{ servicePrefs.traditionalChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Memorial Services</div>
            <div class="metric-value">{{ servicePrefs.memorial }}%</div>
            <div class="metric-change positive">+{{ servicePrefs.memorialChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Celebration of Life</div>
            <div class="metric-value">{{ servicePrefs.celebration }}%</div>
            <div class="metric-change positive">+{{ servicePrefs.celebrationChange }}%</div>
          </div>
        </div>
      </div>

      <!-- Engagement Metrics Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Family Engagement</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Messages Exchanged</div>
            <div class="metric-value">{{ engagementStats.messages }}</div>
            <div class="metric-change positive">+{{ engagementStats.messagesChange }}</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Photo Uploads</div>
            <div class="metric-value">{{ engagementStats.photos }}</div>
            <div class="metric-change positive">+{{ engagementStats.photosChange }}</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Virtual Visit Duration</div>
            <div class="metric-value">{{ engagementStats.visitDuration }} min</div>
            <div class="metric-change positive">+{{ engagementStats.visitDurationChange }}</div>
          </div>
        </div>
      </div>

      <!-- Grief Support Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Grief Support Utilization</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Counseling Sessions</div>
            <div class="metric-value>{{ griefStats.sessions }}</div>
            <div class="metric-change positive">+{{ griefStats.sessionsChange }}</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Resource Downloads</div>
            <div class="metric-value">{{ griefStats.resources }}</div>
            <div class="metric-change positive">+{{ griefStats.resourcesChange }}</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Support Group Attendance</div>
            <div class="metric-value>{{ griefStats.attendance }}</div>
            <div class="metric-change positive">+{{ griefStats.attendanceChange }}</div>
          </div>
        </div>
      </div>

      <!-- Pre-Planning Trends Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Pre-Planning Trends</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">New Pre-Arrangements</div>
            <div class="metric-value>{{ planningStats.newArrangements }}</div>
            <div class="metric-change positive">+{{ planningStats.newArrangementsChange }}</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Conversion Rate</div>
            <div class="metric-value>{{ planningStats.conversionRate }}%</div>
            <div class="metric-change positive">+{{ planningStats.conversionChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Average Lead Time</div>
            <div class="metric-value>{{ planningStats.leadTime }} months</div>
            <div class="metric-change negative">-{{ planningStats.leadTimeChange }}%</div>
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

// Mock data for family portal analytics
const servicePrefs = ref({
  traditional: 45,
  traditionalChange: 5,
  memorial: 35,
  memorialChange: -3,
  celebration: 20,
  celebrationChange: 8
});

const engagementStats = ref({
  messages: 1240,
  messagesChange: 15,
  photos: 890,
  photosChange: 22,
  visitDuration: 18,
  visitDurationChange: 4
});

const griefStats = ref({
  sessions: 45,
  sessionsChange: 12,
  resources: 230,
  resourcesChange: 18,
  attendance: 67,
  attendanceChange: 8
});

const planningStats = ref({
  newArrangements: 23,
  newArrangementsChange: 15,
  conversionRate: 68,
  conversionChange: 7,
  leadTime: 8.5,
  leadTimeChange: -10
});

const loading = ref<boolean>(false);
const error = ref<string | null>(null);

const refreshData = async () => {
  // In a real implementation, this would call analytics service
  // For now, we'll just update with slight random variations
  loading.value = true;
  
  // Simulate data updates
  servicePrefs.value.traditional += Math.floor(Math.random() * 3) - 1;
  engagementStats.value.messages += Math.floor(Math.random() * 20) - 10;
  griefStats.value.sessions += Math.floor(Math.random() * 3) - 1;
  planningStats.value.newArrangements += Math.floor(Math.random() * 2) - 1;
  
  // Keep values in reasonable ranges
  servicePrefs.value.traditional = Math.max(0, Math.min(100, servicePrefs.value.traditional));
  servicePrefs.value.memorial = Math.max(0, Math.min(100, servicePrefs.value.memorial));
  servicePrefs.value.celebration = Math.max(0, Math.min(100, servicePrefs.value.celebration));
  engagementStats.value.messages = Math.max(0, engagementStats.value.messages);
  engagementStats.value.photos = Math.max(0, engagementStats.value.photos);
  griefStats.value.sessions = Math.max(0, griefStats.value.sessions);
  planningStats.value.newArrangements = Math.max(0, planningStats.value.newArrangements);
  
  loading.value = false;
};

onMounted(() => {
  // Load initial data
  refreshData();
  
  // Refresh every 3 minutes for family portal analytics
  setInterval(refreshData, 180000);
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
