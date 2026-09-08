<template>
  <div class="analytics-dashboard">
    <div class="dashboard-header">
      <h1 class="title">Vendor Performance Analytics</h1>
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
      <!-- Supply Chain Performance Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Supply Chain Performance</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">On-Time Delivery Rate</div>
            <div class="metric-value">{{ supplyChain.onTimeRate }}%</div>
            <div class="metric-change positive">+{{ supplyChain.onTimeChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Order Accuracy</div>
            <div class="metric-value">{{ supplyChain.accuracyRate }}%</div>
            <div class="metric-change positive">+{{ supplyChain.accuracyChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Average Lead Time</div>
            <div class="metric-value">{{ supplyChain.leadTime }} days</div>
            <div class="metric-change negative">-{{ supplyChain.leadTimeChange }}%</div>
          </div>
        </div>
      </div>

      <!-- Product Demand Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Product Demand Trends</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Top Selling Category</div>
            <div class="metric-value">{{ demandStats.topCategory }}</div>
            <div class="metric-change">→</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Inventory Turnover</div>
            <div class="metric-value">{{ demandStats.turnover }}x</div>
            <div class="metric-change positive">+{{ demandStats.turnoverChange }}</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Stockout Incidents</div>
            <div class="metric-value>{{ demandStats.stockouts }}</div>
            <div class="metric-change negative">+{{ demandStats.stockoutsChange }}</div>
          </div>
        </div>
      </div>

      <!-- Financial Performance Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Financial Performance</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Monthly Revenue</div>
            <div class="metric-value">R {{ financialStats.revenue.toLocaleString() }}</div>
            <div class="metric-change positive">+{{ financialStats.revenueChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Profit Margin</div>
            <div class="metric-value">{{ financialStats.margin }}%</div>
            <div class="metric-change positive">+{{ financialStats.marginChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Accounts Receivable Days</div>
            <div class="metric-value">{{ financialStats.arDays }} days</div>
            <div class="metric-change negative">+{{ financialStats.arDaysChange }}</div>
          </div>
        </div>
      </div>

      <!-- Customer Satisfaction Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Customer Satisfaction</h2>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Customer Satisfaction Score</div>
            <div class="metric-value">{{ satisfactionStats.score }}/10</div>
            <div class="metric-change positive">+{{ satisfactionStats.change }}</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">On-Time Installation Rate</div>
            <div class="metric-value">{{ satisfactionStats.installRate }}%</div>
            <div class="metric-change positive">+{{ satisfactionStats.installChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Repeat Business Rate</div>
            <div class="metric-value>{{ satisfactionStats.repeatRate }}%</div>
            <div class="metric-change positive">+{{ satisfactionStats.repeatChange }}</div>
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

// Mock data for vendor hub analytics
const supplyChain = ref({
  onTimeRate: 94,
  onTimeChange: 3,
  accuracyRate: 96,
  accuracyChange: 2,
  leadTime: 3.2,
  leadTimeChange: -15
});

const demandStats = ref({
  topCategory: "Caskets & Urns",
  turnover: 4.2,
  turnoverChange: 0.3,
  stockouts: 8,
  stockoutsChange: -2
});

const financialStats = ref({
  revenue: 875000,
  revenueChange: 12,
  margin: 28,
  marginChange: 2,
  arDays: 32,
  arDaysChange: -4
});

const satisfactionStats = ref({
  score: 9.3,
  change: 0.5,
  installRate: 91,
  installChange: 4,
  repeatRate: 76,
  repeatChange: 5
});

const loading = ref<boolean>(false);
const error = ref<string | null>(null);

const refreshData = async () => {
  // In a real implementation, this would call analytics service
  // For now, we'll just update with slight random variations
  loading.value = true;
  
  // Simulate data updates
  supplyChain.value.onTimeRate += Math.floor(Math.random() * 3) - 1;
  demandStats.value.turnover += (Math.random() * 0.2 - 0.1);
  financialStats.value.revenue += Math.floor(Math.random() * 50000) - 25000;
  satisfactionStats.value.score += (Math.random() * 0.2 - 0.1);
  
  // Keep values in reasonable ranges
  supplyChain.value.onTimeRate = Math.max(0, Math.min(100, supplyChain.value.onTimeRate));
  supplyChain.value.accuracyRate = Math.max(0, Math.min(100, supplyChain.value.accuracyRate));
  demandStats.value.turnover = Math.max(0, demandStats.value.turnover);
  financialStats.value.revenue = Math.max(0, financialStats.value.revenue);
  satisfactionStats.value.score = Math.max(0, Math.min(10, satisfactionStats.value.score));
  
  loading.value = false;
};

onMounted(() => {
  // Load initial data
  refreshData();
  
  // Refresh every 3 minutes for vendor hub analytics
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
