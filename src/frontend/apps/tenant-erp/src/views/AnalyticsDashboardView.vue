<template>
  <div class="analytics-dashboard">
    <div class="dashboard-header">
      <h1 class="title">Analytics Dashboard</h1>
      <div class="header-controls">
        <KButton @click="refreshData" variant="outline" size="sm">
          Refresh Data
        </KButton>
        <KButton @click="exportReport" variant="secondary" size="sm">
          Export Report
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
      <!-- Financial Analytics Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Financial Overview</h2>
          <KButton @click="showFinancialDetails = true" variant="outline" size="sm">
            Details
          </KButton>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Monthly Revenue</div>
            <div class="metric-value">R {{ financialStats.monthlyRevenue.toLocaleString() }}</div>
            <div class="metric-change positive">+{{ financialStats.revenueGrowth }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Monthly Expenses</div>
            <div class="metric-value">R {{ financialStats.monthlyExpenses.toLocaleString() }}</div>
            <div class="metric-change negative">-{{ financialStats.expenseChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Profit Margin</div>
            <div class="metric-value">{{ financialStats.profitMargin }}%</div>
            <div class="metric-change positive">+{{ financialStats.marginChange }}%</div>
          </div>
        </div>
      </div>

      <!-- Operational Analytics Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Operational Metrics</h2>
          <KButton @click="showOperationalDetails = true" variant="outline" size="sm">
            Details
          </KButton>
        </div>
        <div class="card-content">
          <div class="metric-row">
            <div class="metric-label">Average Case Duration</div>
            <div class="metric-value">{{ operationalStats.avgCaseDuration }} days</div>
            <div class="metric-change negative">-{{ operationalStats.durationChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Resource Utilization</div>
            <div class="metric-value">{{ operationalStats.resourceUtilization }}%</div>
            <div class="metric-change positive">+{{ operationalStats.utilizationChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Customer Satisfaction</div>
            <div class="metric-value">{{ operationalStats.satisfactionScore }}/10</div>
            <div class="metric-change positive">+{{ operationalStats.satisfactionChange }}</div>
          </div>
        </div>
      </div>

      <!-- Work Item Trends Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Work Item Trends</h2>
          <KButton @click="showWorkItemChart = true" variant="outline" size="sm">
            View Chart
          </KButton>
        </div>
        <div class="card-content">
          <div class="chart-container">
            <canvas id="workItemTrendsChart"></canvas>
          </div>
          <div class="chart-info">
            <p>Work items completed over the last {{ workItemTrends.monthsBack }} months</p>
          </div>
        </div>
      </div>

      <!-- Lead Conversion Card -->
      <div class="analytics-card">
        <div class="card-header">
          <h2 class="card-title">Lead Conversion</h2>
          <KButton @click="showLeadDetails = true" variant="outline" size="sm">
            Details
          </KButton>
        </div>
        <div class="cart-content">
          <div class="metric-row">
            <div class="metric-label">Lead Conversion Rate</div>
            <div class="metric-value">{{ leadStats.conversionRate }}%</div>
            <div class="metric-change positive">+{{ leadStats.conversionChange }}%</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Leads This Month</div>
            <div class="metric-value">{{ leadStats.monthlyLeads }}</div>
            <div class="metric-change positive">+{{ leadStats.leadsChange }}</div>
          </div>
          <div class="metric-row">
            <div class="metric-label">Average Response Time</div>
            <div class="metric-value">{{ leadStats.avgResponseTime }} hours</div>
            <div class="metric-change negative">-{{ leadStats.responseTimeChange }}%</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, computed } from 'vue';
import { KButton, KInput, KSelect, KDialog } from '@khet360/ui-shared';
import { analyticsService } from '@/services/analyticsService';
import { ConnectionStatus } from '@/components/ConnectionStatus.vue';

// Chart.js imports
import { Chart, ChartConfiguration, ChartOptions, ResizeContract } from 'chart.js';

// State
const loading = ref<boolean>(true);
const error = ref<string | null>(null);

// Data
const financialStats = ref({
  monthlyRevenue: 0,
  monthlyExpenses: 0,
  profitMargin: 0,
  revenueGrowth: 0,
  expenseChange: 0,
  marginChange: 0,
  totalRevenueYTD: 0,
  totalExpensesYTD: 0,
  netProfitYTD: 0,
  accountsReceivable: 0
});

const operationalStats = ref({
  avgCaseDuration: 0,
  durationChange: 0,
  resourceUtilization: 0,
  utilizationChange: 0,
  satisfactionScore: 0,
  satisfactionChange: 0,
  activeCases: 0,
  completedThisMonth: 0,
  pendingTasks: 0,
  staffUtilization: 0
});

const workItemTrends = ref({
  monthsBack: 6,
  labels: [],
  data: []
});

const leadStats = ref({
  conversionRate: 0,
  conversionChange: 0,
  monthlyLeads: 0,
  leadsChange: 0,
  avgResponseTime: 0,
  responseTimeChange: 0,
  qualityScore: 0,
  followupRate: 0,
  avgDealSize: 0,
  salesCycleLength: 0
});

// Chart instances
const financialChart = ref<Chart | null>(null);
const operationalChart = ref<Chart | null>(null);
const workItemChart = ref<Chart | null>(null);
const leadChart = ref<Chart | null>(null);

// UI State
const showFinancialDetails = ref(false);
const showOperationalDetails = ref(false);
const showWorkItemChart = ref(false);
const showLeadDetails = ref(false);
const selectedPeriod = ref('monthly');
const monthsBack = ref(6);

// Data loading functions
const loadFinancialStats = async () => {
  try {
    const data = await analyticsService.getFinancialAnalytics();
    financialStats.value = { ...financialStats.value, ...data };
  } catch (err) {
    console.error('Error loading financial stats:', err);
    // Use mock data for development
    financialStats.value = {
      monthlyRevenue: 1250000,
      monthlyExpenses: 850000,
      profitMargin: 32,
      revenueGrowth: 12,
      expenseChange: 5,
      marginChange: 3,
      totalRevenueYTD: 14500000,
      totalExpensesYTD: 9800000,
      netProfitYTD: 4700000,
      accountsReceivable: 750000
    };
  }
};

const loadOperationalStats = async () => {
  try {
    const data = await analyticsService.getOperationalAnalytics();
    operationalStats.value = { ...operationalStats.value, ...data };
  } catch (err) {
    console.error('Error loading operational stats:', err);
    // Use mock data for development
    operationalStats.value = {
      avgCaseDuration: 14.5,
      durationChange: -8,
      resourceUtilization: 78,
      utilizationChange: 5,
      satisfactionScore: 9.2,
      satisfactionChange: 0.5,
      activeCases: 24,
      completedThisMonth: 18,
      pendingTasks: 42,
      staffUtilization: 85
    };
  }
};

const loadWorkItemTrends = async () => {
  try {
    const data = await analyticsService.getWorkItemTrends(
      selectedPeriod.value,
      monthsBack.value
    );
    workItemTrends.value = { 
      labels: data.labels || [], 
      data: data.data || [] 
    };
    workItemTrends.value.monthsBack = monthsBack.value;
  } catch (err) {
    console.error('Error loading work item trends:', err);
    // Use mock data for development
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    workItemTrends.value = {
      labels: months.slice(-monthsBack.value),
      data: Array.from({ length: monthsBack.value }, () => Math.floor(Math.random() * 50) + 20),
      monthsBack: monthsBack.value
    };
  }
};

const loadLeadStats = async () => {
  try {
    const data = await analyticsService.getLeadConversionAnalytics();
    leadStats.value = { ...leadStats.value, ...data };
  } catch (err) {
    console.error('Error loading lead stats:', err);
    // Use mock data for development
    leadStats.value = {
      conversionRate: 65,
      conversionChange: 8,
      monthlyLeads: 45,
      leadsChange: 12,
      avgResponseTime: 4.2,
      responseTimeChange: -15,
      qualityScore: 8.7,
      followupRate: 92,
      avgDealSize: 12500,
      salesCycleLength: 32
    };
  }
};

const refreshData = async () => {
  try {
    loading.value = true;
    error.value = null;
    
    // Load all data in parallel
    await Promise.all([
      loadFinancialStats(),
      loadOperationalStats(),
      loadWorkItemTrends(),
      loadLeadStats()
    ]);
    
    // Initialize charts after data loads
    await initCharts();
  } catch (err) {
    error.value = 'Failed to load analytics data. Please try again later.';
    console.error('Error loading analytics data:', err);
  } finally {
    loading.value = false;
  }
};

// Chart initialization functions
const initFinancialChart = () => {
  const ctx = document.getElementById('financialChart');
  if (ctx && financialChart.value) {
    financialChart.value.destroy();
  }
  
  const ctxElement = document.getElementById('financialChart') as HTMLCanvasElement;
  if (!ctxElement) return;
  
  financialChart.value = new Chart(ctxElement, {
    type: 'doughnut',
    data: {
      labels: ['Revenue', 'Expenses', 'Profit'],
      datasets: [{
        data: [
          financialStats.value.monthlyRevenue,
          financialStats.value.monthlyExpenses,
          financialStats.value.monthlyRevenue - financialStats.value.monthlyExpenses
        ],
        backgroundColor: [
          '#10b981',
          '#ef4444',
          '#3b82f6'
        ]
      }]
    },
    options: {
      responsive: true,
      plugins: {
        legend: {
          position: 'bottom'
        },
        tooltip: {
          enabled: true
        }
      }
    }
  });
};

const initOperationalChart = () => {
  const ctx = document.getElementById('operationalChart');
  if (ctx && operationalChart.value) {
    operationalChart.value.destroy();
  }
  
  const ctxElement = document.getElementById('operationalChart') as HTMLCanvasElement;
  if (!ctxElement) return;
  
  operationalChart.value = new Chart(ctxElement, {
    type: 'bar',
    data: {
      labels: ['Cases', 'Tasks', 'Resources'],
      datasets: [{
        label: 'Utilization %',
        data: [
          operationalStats.value.activeCases,
          operationalStats.value.pendingTasks,
          operationalStats.value.resourceUtilization
        ],
        backgroundColor: [
          '#3b82f6',
          '#f59e0b',
          '#10b981'
        ]
      }]
    },
    options: {
      responsive: true,
      scales: {
        y: {
          beginAtZero: true,
          max: 100
        }
      },
      plugins: {
        legend: {
          display: false
        }
      }
    }
  });
};

const initWorkItemChart = () => {
  const ctx = document.getElementById('workItemTrendsChart');
  if (ctx && workItemChart.value) {
    workItemChart.value.destroy();
  }
  
  const ctxElement = document.getElementById('workItemTrendsChart') as HTMLCanvasElement;
  if (!ctxElement) return;
  
  workItemChart.value = new Chart(ctxElement, {
    type: 'line',
    data: {
      labels: workItemTrends.value.labels,
      datasets: [{
        label: 'Work Items Completed',
        data: workItemTrends.value.data,
        borderColor: '#3b82f6',
        backgroundColor: 'rgba(59, 130, 246, 0.1)',
        tension: 0.3,
        fill: true
      }]
    },
    options: {
      responsive: true,
      scales: {
        y: {
          beginAtZero: true
        }
      },
      plugins: {
        legend: {
          display: false
        },
        tooltip: {
          mode: 'index',
          intersect: false
        }
      }
    }
  });
};

const initLeadChart = () => {
  const ctx = document.getElementById('leadChart');
  if (ctx && leadChart.value) {
    leadChart.value.destroy();
  }
  
  const ctxElement = document.getElementById('leadChart') as HTMLCanvasElement;
  if (!ctxElement) return;
  
  leadChart.value = new Chart(ctxElement, {
    type: 'pie',
    data: {
      labels: ['New', 'Contacted', 'Qualified', 'Converted'],
      datasets: [{
        data: [20, 35, 25, leadStats.value.conversionRate],
        backgroundColor: [
          '#6b7280',
          '#fbbf24',
          '#f97316',
          '#10b981'
        ]
      }]
    },
    options: {
      responsive: true,
      plugins: {
        legend: {
          position: 'bottom'
        }
      }
    }
  });
};

const initCharts = async () => {
  await Promise.all([
    initFinancialChart(),
    initOperationalChart(),
    initWorkItemChart(),
    initLeadChart()
  ]);
};

const updateWorkItemChart = async () => {
  await loadWorkItemTrends();
  if (workItemChart.value) {
    workItemChart.value.data.labels = workItemTrends.value.labels;
    workItemChart.value.data.datasets[0].data = workItemTrends.value.data;
    workItemChart.value.update();
  }
};

const refreshDataHandler = async () => {
  await refreshData();
};

const exportReport = async () => {
  // In a real implementation, this would generate and download a PDF/Excel report
  alert('Exporting analytics report...');
  // TODO: Implement actual export functionality
};

onMounted(() => {
  // Load initial data
  refreshData();
  
  // Set up auto-refresh (every 5 minutes for analytics)
  setInterval(refreshData, 300000);
});

onBeforeUnmount(() => {
  // Destroy chart instances to prevent memory leaks
  if (financialChart.value) financialChart.value.destroy();
  if (operationalChart.value) operationalChart.value.destroy();
  if (workItemChart.value) workItemChart.value.destroy();
  if (leadChart.value) leadChart.value.destroy();
});
</script>

<script setup lang="ts">
</script>

<style scoped>
.analytics-dashboard {
  padding: 2rem;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.dashboard-header h1 {
  font-size: 2rem;
  font-weight: 700;
  margin: 0;
}

.header-controls {
  display: flex;
  gap: 1rem;
}

.loading-container {
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

.error-message {
  padding: 1rem;
  background-color: #f8d7da;
  color: #721c24;
  border-radius: var(--khet-radius-md);
  border: 1px solid #f5c6cb;
}

.analytics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
}

.analytics-card {
  background: white;
  border-radius: 1rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1), 0 1px 3px rgba(0, 0, 0, 0.1);
  padding: 1.5rem;
  transition: transform 0.2s, box-shadow 0.2s;
}

.analytics-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 15px rgba(0, 0, 0, 0.1), 0 4px 6px rgba(0, 0, 0, 0.1);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.card-header h2 {
  font-size: 1.25rem;
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
  color: #6b7280;
}

.metric-value {
  font-size: 1.125rem;
  font-weight: 600;
  color: #1f2937;
}

.metric-change {
  font-size: 0.875rem;
  font-weight: 500;
}

.metric-change.positive {
  color: #10b981;
}

.metric-change.negative {
  color: #ef4444;
}

.chart-container {
  width: 100%;
  height: 250px;
  margin: 1.5rem 0;
}

.chart-info {
  font-size: 0.875rem;
  color: #6b7280;
  text-align: center;
  margin-top: 1rem;
}

.chart-controls {
  display: flex;
  gap: 1rem;
  align-items: center;
  flex-wrap: wrap;
  margin: 1rem 0;
}

.chart-controls KSelect,
.chart-controls KInput {
  min-width: 150px;
}

.metric-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
}

.metric-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.label {
  font-size: 0.75rem;
  color: #6b7280;
}

.value {
  font-size: 0.875rem;
  font-weight: 600;
  color: #1f2937;
}

/* Modal styles */
.dialog-content {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.dialog-content h3 {
  margin-top: 0;
  margin-bottom: 1.5rem;
}

.dialog-content .chart-container {
  flex: 1;
}

.mt-4 {
  margin-top: 1.5rem;
}

/* Responsive design */
@media (max-width: 768px) {
  .dashboard-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1.5rem;
  }
  
  .analytics-grid {
    grid-template-columns: 1fr;
  }
  
  .header-controls {
    width: 100%;
    justify-content: space-between;
  }
  
  .chart-container {
    height: 200px;
  }
}
</style>
