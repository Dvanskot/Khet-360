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
      
      <!-- Quick Actions -->
      <div class="quick-actions">
        <h3 class="section-title">Quick Actions</h3>
        <div class="actions-grid">
          <KButton @click="showCreateWorkItemDialog = true" variant="secondary">
            Add Work Item
          </KButton>
          <KButton @click="showCreateLeadDialog = true" variant="secondary">
            Add Lead
          </KButton>
          <KButton @click="showNotificationSettings = true" variant="outline">
            Notification Settings
          </KButton>
        </div>
      </div>
    </div>
  </div>
  
  <!-- Create Work Item Dialog -->
  <KDialog v-model:show="showCreateWorkItemDialog" title="Create New Work Item" width="500px">
    <div class="dialog-content">
      <div class="form-group">
        <label for="title" class="form-label">Title *</label>
        <KInput
          v-model="workItemForm.title"
          id="title"
          placeholder="Enter work item title"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="description" class="form-label">Description</label>
        <KTextarea
          v-model="workItemForm.description"
          id="description"
          placeholder="Enter work item description"
          rows="3"
        />
      </div>
      
      <div class="form-group">
        <label for="caseId" class="form-label">Case ID *</label>
        <KInput
          v-model="workItemForm.caseId"
          id="caseId"
          placeholder="Enter case ID"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="priority" class="form-label">Priority</label>
        <KSelect
          v-model="workItemForm.priority"
          id="priority"
          :options="priorityOptions"
          placeholder="Select priority"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="dueDate" class="form-label">Due Date</label>
        <KInput
          type="date"
          v-model="workItemForm.dueDate"
          id="dueDate"
        />
      </div>
    </div>
    
    <template #footer>
      <KButton variant="secondary" @click="showCreateWorkItemDialog = false">
        Cancel
      </KButton>
      <KButton 
        variant="primary" 
        @click="saveWorkItem"
        :loading="savingWorkItem"
      >
        Save Work Item
      </KButton>
    </template>
  </KDialog>
  
  <!-- Create Lead Dialog -->
  <KDialog v-model:show="showCreateLeadDialog" title="Create New Lead" width="500px">
    <div class="dialog-content">
      <div class="form-group">
        <label for="contactName" class="form-label">Contact Name *</label>
        <KInput
          v-model="leadForm.contactName"
          id="contactName"
          placeholder="Enter contact name"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="organization" class="form-label">Organization</label>
        <KInput
          v-model="leadForm.organization"
          id="organization"
          placeholder="Enter organization name"
        />
      </div>
      
      <div class="form-group">
        <label for="serviceInterest" class="form-label">Service Interest *</label>
        <KSelect
          v-model="leadForm.serviceInterest"
          id="serviceInterest"
          :options="serviceOptions"
          placeholder="Select service interest"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="priority" class="form-label">Priority</label>
        <KSelect
          v-model="leadForm.priority"
          id="priority"
          :options="priorityOptions"
          placeholder="Select priority"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="status" class="form-label">Status</label>
        <KSelect
          v-model="leadForm.status"
          id="status"
          :options="statusOptions"
          placeholder="Select status"
          required
        />
      </div>
    </div>
    
    <template #footer>
      <KButton variant="secondary" @click="showCreateLeadDialog = false">
        Cancel
      </KButton>
      <KButton 
        variant="primary" 
        @click="saveLead"
        :loading="savingLead"
      >
        Save Lead
      </KButton>
    </template>
  </KDialog>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, computed } from 'vue';
import { KButton, KInput, KTextarea, KSelect, KDialog } from '@khet360/ui-shared';
import { WorkItemService } from '@/services/workItemService';
import { LeadService } from '@/services/leadService';
import { signalRService } from '@/services/signalRService';
import { authService } from '@/services/authService';
import { notificationService } from '@/services/notificationService';
import { ConnectionStatus } from '@/components/ConnectionStatus.vue';

const stats = ref({
  activeCases: 0,
  monthlyRevenue: 0,
  fleetUtilization: 0,
  pendingTasks: 0
});

const recentActivities = ref([]);
const loading = ref<boolean>(true);
const error = ref<string | null>(null);

const priorityOptions = ['Low', 'Medium', 'High', 'Critical'];
const statusOptions = ['New', 'Contacted', 'Qualified', 'Proposal Sent', 'Negotiation', 'Closed Won', 'Closed Lost'];
const serviceOptions = [
  'Traditional Funeral',
  'Cremation Service',
  'Memorial Service',
  'Pre-Planning Consultation',
  'Grief Support',
  'Transportation Services',
  'Floral Arrangements',
  'Catering Services'
];

const workItemForm = ref({
  title: '',
  description: '',
  caseId: '',
  priority: 'Medium',
  dueDate: ''
});

const leadForm = ref({
  contactName: '',
  organization: '',
  serviceInterest: '',
  priority: 'Medium',
  status: 'New'
});

const savingWorkItem = ref(false);
const savingLead = ref(false);
const showCreateWorkItemDialog = ref(false);
const showCreateLeadDialog = ref(false);
const showNotificationSettings = ref(false);

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

const handleWorkItemCreated = (workItem: any) => {
  notificationService.addNotification({
    title: 'Work Item Created',
    message: `New work item "${workItem.title}" has been created`,
    type: 'success'
  });
  // Optionally refresh stats or activity feed
};

const handleLeadCreated = (lead: any) => {
  notificationService.addNotification({
    title: 'New Lead Created',
    message: `New lead "${lead.contactName}" from ${lead.organization || 'unknown'} has been created`,
    type: 'success'
  });
};

const fetchDashboardData = async () => {
  try {
    loading.value = true;
    error.value = null;
    
    // Fetch statistics
    const workItems = await WorkItemService.getWorkItems();
    const leads = await LeadService.getLeads();
    
    // Calculate statistics
    stats.value = {
      activeCases: workItems.filter(item => item.status !== 'Completed').length,
      monthlyRevenue: 1250000, // This would come from a financial service
      fleetUtilization: 78, // This would come from a fleet service
      pendingTasks: workItems.filter(item => item.status === 'Pending' || item.status === 'In Progress').length
    };
    
    // Generate recent activities
    recentActivities.value = [
      ...workItems.slice(0, 3).map(item => ({
        id: `wi-${item.id}`,
        title: item.title,
        description: item.description,
        icon: '📋',
        timestamp: new Date(item.updatedAt || item.createdAt).getTime()
      })),
      ...leads.slice(0, 2).map(lead => ({
        id: `lead-${lead.id}`,
        title: `New Lead: ${lead.contactName}`,
        description: `${lead.organization || ''} - ${lead.serviceInterest}`,
        icon: '👤',
        timestamp: new Date(lead.updatedAt || lead.createdAt).getTime()
      }))
    ].sort((a, b) => b.timestamp - a.timestamp); // Sort by timestamp descending
    
  } catch (err) {
    error.value = 'Failed to load dashboard data. Please try again later.';
    console.error('Error fetching dashboard data:', err);
    // Fallback to mock data
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
  } finally {
    loading.value = false;
  }
};

// Real-time update handlers for work items and leads
const handleWorkItemUpdated = (workItem: any) => {
  // Update stats if needed
  if (workItem.status === 'Completed') {
    // Recalculate pending tasks
    stats.value.pendingTasks = Math.max(0, stats.value.pendingTasks - 1);
  } else if (workItem.status === 'Pending' || workItem.status === 'In Progress') {
    // Recalculate pending tasks
    stats.value.pendingTasks = Math.min(999, stats.value.pendingTasks + 1);
  }
  
  notificationService.addNotification({
    title: 'Work Item Updated',
    message: `Work item "${workItem.title}" has been updated`,
    type: 'info'
  });
};

const handleWorkItemCompleted = (workItem: any) => {
  // Update completed work item stats
  stats.value.activeCases = Math.max(0, stats.value.activeCases - 1);
  stats.value.pendingTasks = Math.max(0, stats.value.pendingTasks - 1);
  
  notificationService.addNotification({
    title: 'Work Item Completed',
    message: `Work item "${workItem.title}" has been marked as complete`,
    type: 'success'
  });
};

const handleLeadUpdated = (lead: any) => {
  notificationService.addNotification({
    title: 'Lead Updated',
    message: `Lead "${lead.contactName}" has been updated`,
    type: 'info'
  });
};

const saveWorkItem = async () => {
  try {
    savingWorkItem.value = true;
    const newWorkItem = await WorkItemService.createWorkItem({
      title: workItemForm.value.title,
      description: workItemForm.value.description,
      caseId: workItemForm.value.caseId,
      priority: workItemForm.value.priority,
      dueDate: workItemForm.value.dueDate || undefined,
      status: 'Pending'
    });
    
    // Close dialog and refresh
    showCreateWorkItemDialog.value = false;
    workItemForm.value = {
      title: '',
      description: '',
      caseId: '',
      priority: 'Medium',
      dueDate: ''
    };
    
    await fetchDashboardData();
  } catch (err) {
    error.value = 'Failed to save work item. Please try again later.';
    console.error('Error saving work item:', err);
  } finally {
    savingWorkItem.value = false;
  }
};

const saveLead = async () => {
  try {
    savingLead.value = true;
    const newLead = await LeadService.createLead({
      contactName: leadForm.value.contactName,
      organization: leadForm.value.organization,
      serviceInterest: leadForm.value.serviceInterest,
      priority: leadForm.value.priority,
      status: leadForm.value.status
    });
    
    // Close dialog and refresh
    showCreateLeadDialog.value = false;
    leadForm.value = {
      contactName: '',
      organization: '',
      serviceInterest: '',
      priority: 'Medium',
      status: 'New'
    };
    
    await fetchDashboardData();
  } catch (err) {
    error.value = 'Failed to save lead. Please try again later.';
    console.error('Error saving lead:', err);
  } finally {
    savingLead.value = false;
  }
};

onMounted(() => {
  // Initialize data
  fetchDashboardData();
  
  // Set up SignalR listeners for real-time updates
  signalRService.on('DashboardStatsUpdated', handleStatsUpdate);
  signalRService.on('NewActivityAdded', handleActivityUpdate);
  signalRService.on('WorkItemCreated', handleWorkItemCreated);
  signalRService.on('WorkItemUpdated', handleWorkItemUpdated);
  signalRService.on('WorkItemCompleted', handleWorkItemCompleted);
  signalRService.on('LeadCreated', handleLeadCreated);
  signalRService.on('LeadUpdated', handleLeadUpdated);
  
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
  signalRService.off('WorkItemCreated', handleWorkItemCreated);
  signalRService.off('WorkItemUpdated', handleWorkItemUpdated);
  signalRService.off('WorkItemCompleted', handleWorkItemCompleted);
  signalRService.off('LeadCreated', handleLeadCreated);
  signalRService.off('LeadUpdated', handleLeadUpdated);
  
  // Note: We don't stop the SignalR connection here as other components might still need it
  // In a real app, you might want to track connection usage and stop when no longer needed
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

.quick-actions {
  margin-top: 2rem;
}

.actions-grid {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

/* Dialog styles */
.dialog-content {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-label {
  font-weight: 600;
  font-size: 0.875rem;
  margin-bottom: 0.25rem;
}
</style>