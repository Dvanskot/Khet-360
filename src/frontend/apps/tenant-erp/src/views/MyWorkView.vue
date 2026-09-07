<template>
  <div class="page">
    <header class="page-header">
      <h1 class="title">My Work</h1>
      <div class="header-actions">
        <ConnectionStatus />
        <KButton @click="refreshData" variant="outline" size="sm">
          Refresh
        </KButton>
      </div>
    </header>

    <div v-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-if="loading" class="loading-indicator">
      <div class="spinner"></div>
      <p>Loading work items...</p>
    </div>

    <div class="filter-bar">
      <button
        v-for="f in filters"
        :key="f.value"
        @click="activeFilter = f.value"
        class="filter-chip"
        :class="{ active: activeFilter === f.value }"
      >
        {{ f.label }}
      </button>
    </div>

    <div class="task-list">
      <div v-if="workItems.length === 0" class="empty-state">
        <span class="emoji">🎉</span>
        <p>All caught up! No pending tasks.</p>
      </div>

      <div v-for="task in filteredWorkItems" :key="task.id" class="task-card" :class="task.priority.toLowerCase()">
        <div class="task-info">
          <div class="task-meta">
            <span class="case-id">{{ task.caseId }}</span>
            <span class="due-date">{{ formatDate(task.dueDate) }}</span>
          </div>
          <h3>{{ task.title }}</h3>
          <p>{{ task.description }}</p>
        </div>

        <div class="task-actions">
          <button
            v-if="task.status === 'Pending'"
            @click="startWorkItem(task.id)"
            class="btn-action"
          >
            Start
          </button>
          <button
            v-if="task.status === 'In Progress'"
            @click="completeWorkItem(task.id)"
            class="btn-action btn-complete"
          >
            Finish
          </button>
          <span v-if="task.status === 'Blocked'" class="status-badge">
            Blocked
          </span>
          <span v-if="task.status === 'Completed'" class="status-badge">
            Completed
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, computed } from 'vue';
import { KButton } from '@khet360/ui-shared';
import { WorkItemService, WorkItem } from '@/services/workItemService';
import { LeadService, Lead } from '@/services/leadService';
import { signalRService } from '@/services/signalRService';
import { authService } from '@/services/authService';
import { notificationService } from '@/services/notificationService';
import { ConnectionStatus } from '@/components/ConnectionStatus.vue';

const workItems = ref<WorkItem[]>([]);
const leads = ref<Lead[]>([]);
const loading = ref<boolean>(true);
const error = ref<string | null>(null);

const filters = [
  { label: 'All Tasks', value: 'all' },
  { label: 'Venue', value: 'venue' },
  { label: 'Field', value: 'field' },
  { label: 'Leads', value: 'leads' },
];

const activeFilter = ref('all');
const workItemCleanup = ref<() => void>(() => {});
const leadCleanup = ref<() => void>(() => {});

// Computed property for filtered work items
const filteredWorkItems = computed(() => {
  let items = [...workItems.value];

  if (activeFilter.value === 'venue') {
    items = items.filter(item => 
      item.description.toLowerCase().includes('venue') || 
      item.description.toLowerCase().includes('church') ||
      item.description.toLowerCase().includes('cemetery')
    );
  } else if (activeFilter.value === 'field') {
    items = items.filter(item => 
      !item.description.toLowerCase().includes('venue') && 
      !item.description.toLowerCase().includes('church') &&
      !item.description.toLowerCase().includes('cemetery')
    );
  } else if (activeFilter.value === 'leads') {
    // When filtering by leads, we'll show leads instead of work items
    return leads.value.map(lead => ({
      id: lead.id,
      title: `Follow up with ${lead.contactName}`,
      description: `${lead.organization || ''} - ${lead.serviceInterest}`,
      caseId: lead.id.toString(),
      dueDate: lead.createdAt,
      priority: lead.priority || 'Medium',
      status: lead.status
    }));
  }

  // Sort by priority and status
  const priorityMap = { Critical: 0, High: 1, Medium: 2, Low: 3 };
  const statusOrder = { 'Completed': 4, 'Blocked': 3, 'In Progress': 2, 'Pending': 1 };

  return items.sort((a, b) => {
    // Completed items go to the end
    if (a.status === 'Completed' && b.status !== 'Completed') return 1;
    if (a.status !== 'Completed' && b.status === 'Completed') return -1;
    
    // Then by priority
    if (priorityMap[a.priority] !== priorityMap[b.priority]) {
      return priorityMap[a.priority] - priorityMap[b.priority];
    }
    
    // Finally by status
    return statusOrder[a.status] - statusOrder[b.status];
  });
});

// Real-time update handlers for work items
const handleWorkItemCreated = (workItem: WorkItem) => {
  workItems.value = [...workItems.value, workItem];
  notificationService.addNotification({
    title: 'New Work Item',
    message: `New work item "${workItem.title}" has been created`,
    type: 'success'
  });
};

const handleWorkItemUpdated = (workItem: WorkItem) => {
  const index = workItems.value.findIndex(item => item.id === workItem.id);
  if (index !== -1) {
    workItems.value[index] = { ...workItems.value[index], ...workItem };
    notificationService.addNotification({
      title: 'Work Item Updated',
      message: `Work item "${workItem.title}" has been updated`,
      type: 'info'
    });
  }
};

const handleWorkItemDeleted = (workItemId: number) => {
  workItems.value = workItems.value.filter(item => item.id !== workItemId);
  notificationService.addNotification({
    title: 'Work Item Removed',
    message: `Work item #${workItemId} has been removed`,
    type: 'warning'
  });
};

const handleWorkItemCompleted = (workItem: WorkItem) => {
  const index = workItems.value.findIndex(item => item.id === workItem.id);
  if (index !== -1) {
    workItems.value[index] = { ...workItems.value[index], ...workItem };
    notificationService.addNotification({
      title: 'Work Item Completed',
      message: `Work item "${workItem.title}" has been marked as complete`,
      type: 'success'
    });
  }
};

// Real-time update handlers for leads
const handleLeadCreated = (lead: Lead) => {
  leads.value = [...leads.value, lead];
  notificationService.addNotification({
    title: 'New Lead',
    message: `New lead "${lead.contactName}" from ${lead.organization || 'unknown'} has been created`,
    type: 'success'
  });
};

const handleLeadUpdated = (lead: Lead) => {
  const index = leads.value.findIndex(item => item.id === lead.id);
  if (index !== -1) {
    leads.value[index] = { ...leads.value[index], ...lead };
    notificationService.addNotification({
      title: 'Lead Updated',
      message: `Lead "${lead.contactName}" has been updated`,
      type: 'info'
    });
  }
};

const handleLeadDeleted = (leadId: number) => {
  leads.value = leads.value.filter(item => item.id !== leadId);
  notificationService.addNotification({
    title: 'Lead Removed',
    message: `Lead #${leadId} has been removed`,
    type: 'warning'
  });
};

const handleLeadConverted = (data: any) => {
  // Remove the converted lead
  leads.value = leads.value.filter(item => item.id !== data.leadId);
  
  notificationService.addNotification({
    title: 'Lead Converted',
    message: `Lead has been converted to a case`,
    type: 'success'
  });
};

const fetchWorkItems = async () => {
  try {
    loading.value = true;
    error.value = null;
    workItems.value = await WorkItemService.getWorkItems();
  } catch (err) {
    error.value = 'Failed to load work items. Please try again later.';
    console.error('Error fetching work items:', err);
    // Fallback to mock data
    workItems.value = [
      { id: 1, title: 'Verify Death Certificate', description: 'Verify the uploaded death certificate for Case #C-1024', status: 'Pending', dueDate: 'Today', caseId: 'C-1024', priority: 'High' },
      { id: 2, title: 'Schedule Burial Service', description: 'Coordinate with venue and transport for Case #C-1021', status: 'In Progress', dueDate: 'Tomorrow', caseId: 'C-1021', priority: 'Medium' },
      { id: 3, title: 'Process Claim Payout', description: 'Verify benefits and initiate payout for Case #C-1018', status: 'Blocked', dueDate: '2 days', caseId: 'C-1018', priority: 'High' },
    ];
  } finally {
    loading.value = false;
  }
};

const fetchLeads = async () => {
  try {
    leads.value = await LeadService.getLeads();
  } catch (err) {
    console.error('Error fetching leads:', err);
    // Fallback to mock data
    leads.value = [
      { id: 1, contactName: 'John Smith', organization: 'Smith Family Funeral Home', serviceInterest: 'Traditional Funeral', priority: 'High', status: 'New', createdAt: 'Today' },
      { id: 2, contactName: 'Mary Johnson', organization: '', serviceInterest: 'Cremation Service', priority: 'Medium', status: 'Contacted', createdAt: 'Yesterday' },
    ];
  }
};

const refreshData = async () => {
  await fetchWorkItems();
  await fetchLeads();
};

const startWorkItem = async (id: number) => {
  try {
    await WorkItemService.updateWorkItem(id, { status: 'In Progress' });
    // The service will handle SignalR notification
  } catch (err) {
    error.value = 'Failed to start work item. Please try again later.';
    console.error('Error starting work item:', err);
  }
};

const completeWorkItem = async (id: number) => {
  try {
    await WorkItemService.completeWorkItem(id);
    // The service will handle SignalR notification
  } catch (err) {
    error.value = 'Failed to complete work item. Please try again later.';
    console.error('Error completing work item:', err);
  }
};

const formatDate = (dateStr: string) => {
  if (!dateStr) return 'No date';
  return new Date(dateStr).toLocaleDateString([], { month: 'short', day: 'numeric' });
};

onMounted(() => {
  // Fetch initial data
  fetchWorkItems();
  fetchLeads();
  
  // Set up SignalR listeners for real-time updates
  workItemCleanup.value = WorkItemService.subscribeToWorkItemUpdates(
    handleWorkItemCreated,
    handleWorkItemUpdated,
    handleWorkItemDeleted,
    handleWorkItemCompleted
  );
  
  leadCleanup.value = LeadService.subscribeToLeadUpdates(
    handleLeadCreated,
    handleLeadUpdated,
    handleLeadDeleted,
    handleLeadConverted
  );
  
  // Start SignalR connection if not already started
  signalRService.start().catch(err => {
    console.error('Failed to start SignalR connection:', err);
    notificationService.addNotification({
      title: 'Connection Issue',
      message: 'Unable to connect to real-time updates. Some features may not be live.',
      type: 'warning'
    });
  });
});

onBeforeUnmount(() => {
  // Cleanup SignalR subscriptions
  if (workItemCleanup.value) workItemCleanup.value();
  if (leadCleanup.value) leadCleanup.value();
  
  // Note: We don't stop the SignalR connection here as other components might still need it
  // In a real app, you might want to track connection usage and stop when no longer needed
});
</script>

<script setup lang="ts">
</script>

<style scoped>
.page {
  padding: 1.5rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.page-header h1 {
  font-size: 1.8rem;
  font-weight: 700;
  margin: 0;
}

.page-header .header-actions {
  display: flex;
  gap: 1rem;
}

.error-message {
  padding: 1rem;
  background-color: #f8d7da;
  color: #721c24;
  border-radius: var(--khet-radius-md);
  border: 1px solid #f5c6cb;
}

.loading-indicator {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  color: var(--khet-text-muted);
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

.filter-bar {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
  overflow-x: auto;
  padding-bottom: 0.5rem;
}

.filter-chip {
  background: var(--khet-surface);
  border: 1px solid var(--khet-border);
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
  color: var(--khet-text-muted);
  cursor: pointer;
  white-space: nowrap;
}

.filter-chip.active {
  background: var(--khet-primary);
  color: white;
  border-color: var(--khet-primary);
}

.task-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.task-card {
  background: var(--khet-surface);
  border-radius: 16px;
  padding: 1.25rem;
  border: 1px solid var(--khet-border);
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
  transition: transform 0.1s;
}

.task-card:active {
  transform: scale(0.98);
}

.task-info {
  flex: 1;
}

.task-meta {
  display: flex;
  justify-content: space-between;
  font-size: 0.7rem;
  font-weight: 600;
  text-transform: uppercase;
  color: var(--khet-text-muted);
  margin-bottom: 0.5rem;
}

.case-id {
  color: var(--khet-primary);
}

.task-card h3 {
  font-size: 1.1rem;
  margin: 0 0 0.25rem 0;
  font-weight: 600;
}

.task-card p {
  font-size: 0.9rem;
  color: var(--khet-text-muted);
  margin: 0;
  line-height: 1.4;
}

.task-actions {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}

.btn-action {
  background: var(--khet-primary);
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 8px;
  font-weight: 600;
  font-size: 0.85rem;
  cursor: pointer;
}

.btn-complete {
  background: #10b981;
}

.status-badge {
  font-size: 0.75rem;
  font-weight: 600;
  color: #10b981;
  background: rgba(16, 185, 129, 0.1);
  padding: 4px 8px;
  border-radius: 6px;
}

.empty-state {
  text-align: center;
  padding: 3rem 0;
  color: var(--khet-text-muted);
}

.empty-state .emoji {
  font-size: 3rem;
  display: block;
  margin-bottom: 1rem;
}

/* Priority styling */
.critical { border-left: 4px solid #ef4444; }
.high { border-left: 4px solid #f59e0b; }
.medium { border-left: 4px solid #3b82f6; }
.low { border-left: 4px solid #9ca3af; }
</style>