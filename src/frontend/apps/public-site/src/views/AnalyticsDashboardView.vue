<template>
  <div class="analytics-dashboard">
    <div class="dashboard-header">
      <h1 class="title">Public Memorial Analytics</h1>
      <div class="header-controls">
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
      <!-- Website Engagement Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Website Engagement</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Monthly Visitors</div>
            <div class="metric-value">{{ engagementStats.visitors.toLocaleString() }}</div>
            <div class="metric-change positive">+{{ engagementStats.visitorsChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Average Session Duration</div>
            <div class="metric-value">{{ engagementStats.sessionDuration }} min</div>
            <div class="metric-change positive">+{{ engagementStats.sessionDurationChange }}</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Bounce Rate</div>
            <div class="metric-value">{{ engagementStats.bounceRate }}%</div>
            <div class="metric-change negative">-{{ engagementStats.bounceRateChange }}%</div>
          </div>
        </div>
      </div>

      <!-- Memorial Services Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Memorial Services Interest</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Service Inquiry Rate</div>
            <div class="metric-value>{{ inquiryStats.rate }}%</div>
            <div class="metric-change positive">+{{ inquiryStats.rateChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Pre-Planning Interest</div>
            <div class="metric-value>{{ inquiryStats.planningInterest }}%</div>
            <div class="metric-change positive">+{{ inquiryStats.planningChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Contact Form Submissions</div>
            <div class="metric-value>{{ inquiryStats.submissions }}</div>
            <div class="metric-change positive">+{{ inquiryStats.submissionsChange }}</div>
          </div>
        </div>
      </div>

      <!-- Community Outreach Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Community Outreach</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Social Media Reach</div>
            <div class="metric-value">{{ outreachStats.reach.toLocaleString() }}</div>
            <div class="metric-change positive">+{{ outreachStats.reachChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Event Attendance</div>
            <div class="metric-value>{{ outreachStats.attendance }}</div>
            <div class="metric-change positive">+{{ outreachStats.attendanceChange }}</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Newsletter Subscribers</div>
            <div class="metric-value>{{ outreachStats.subscribers }}</div>
            <div class="metric-change positive">+{{ outreachStats.subscribersChange }}</div>
          </div>
        </div>
      </div>

      <!-- Resource Access Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Resource Access</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Grief Guide Downloads</div>
            <div class="metric-value>{{ resourceStats.guideDownloads }}</div>
            <div class="metric-change positive">+{{ resourceStats.guideChange }}</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">FAQ Page Views</div>
            <div class="metric-value>{{ resourceStats.faqViews }}</div>
            <div class="metric-change positive">+{{ resourceStats.faqChange }}</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Video Content Views</div>
            <div class="metric-value>{{ resourceStats.videoViews }}</div>
            <div class="metric-change positive">+{{ resourceStats.videoChange }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue';
import { KButton } from '@khet360/ui-shared';

// Mock data for public site analytics
const engagementStats = ref({
  visitors: 12450,
  visitorsChange: 18,
  sessionDuration: 4.2,
  sessionDurationChange: 0.5,
  bounceRate: 38,
  bounceRateChange: -5
});

const inquiryStats = ref({
  rate: 12.5,
  rateChange: 2,
  planningInterest: 28,
  planningChange: 4,
  submissions: 89,
  submissionsChange: 12
});

const outreachStats = ref({
  reach: 45600,
  reachChange: 15,
  attendance: 124,
  attendanceChange: 8,
  subscribers: 2340,
  subscribersChange: 22
});

const resourceStats = ref({
  guideDownloads: 560,
  guideChange: 25,
  faqViews: 2340,
  faqChange: 12,
  videoViews: 1890,
  videoChange: 18
});

const loading = ref<boolean>(false);
const error = ref<string | null>(null);

const refreshData = async () => {
  // In a real implementation, this would call analytics service
  // For now, we'll just update with slight random variations
  loading.value = true;
  
  // Simulate data updates
  engagementStats.value.visitors += Math.floor(Math.random() * 500) - 250;
  inquiryStats.value.rate += Math.floor(Math.random() * 3) - 1;
  outreachStats.value.reach += Math.floor(Math.random() * 1000) - 500;
  resourceStats.value.guideDownloads += Math.floor(Math.random() * 20) - 10;
  
  // Keep values in reasonable ranges
  engagementStats.value.visitors = Math.max(0, engagementStats.value.visitors);
  inquiryStats.value.rate = Math.max(0, Math.min(100, inquiryStats.value.rate));
  outreachStats.value.reach = Math.max(0, outreachStats.value.reach);
  resourceStats.value.guideDownloads = Math.max(0, resourceStats.value.guideDownloads);
  
  loading.value = false;
};

onMounted(() => {
  // Load initial data
  refreshData();
  
  // Refresh every 5 minutes for public site analytics
  setInterval(refreshData, 300000);
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
