<template>
  <div class="page">
    <header class="page-header">
      <h1 class="title">My Work</h1>
      <div class="header-actions">
        <ConnectionStatus />
        <div class="sync-status" :class="{ syncing: isSyncing }">
          {{ isSyncing ? 'Syncing...' : 'Synced' }}
        </div>
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
      <div v-if="(workItems.length === 0 && tasks.length === 0) && !loading" class="empty-state">
        <span class="emoji">🎉</span>
        <p>All caught up! No pending tasks.</p>
      </div>

      <!-- API Work Items -->
      <div v-if="workItems.length > 0" class="api-work-items">
        <h3 class="section-title">Server Work Items</h3>
        <div
          v-for="task in workItems"
          :key="task.id"
          class="task-card"
          :class="task.priority.toLowerCase()"
        >
          <div class="task-info">
            <div class="task-meta">
              <span class="case-id">{{ task.caseId }}</span>
              <span class="due-date">{{ formatDate(task.due) }}</span>
            </div>
            <h3>{{ task.title }}</h3>
            <p>{{ task.description }}</p>
          </div>

          <div class="task-actions">
            <button
              v-if="task.status === 'Pending'"
              @click="completeWorkItem(task.id)"
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

      <!-- Local Tasks -->
      <div v-if="tasks.length > 0" class="local-tasks">
        <h3 class="section-title">Local Tasks</h3>
        <div
          v-for="task in sortedTasks"
          :key="task.id"
          class="task-card"
          :class="task.priority.toLowerCase()"
        >
          <div class="task-info">
            <div class="task-meta">
              <span class="case-id">{{ task.caseId }}</span>
              <span class="due-date">{{ formatDate(task.dueAt) }}</span>
            </div>
            <h3>{{ task.title }}</h3>
            <p>{{ task.description }}</p>
          </div>

          <div class="task-actions">
            <button
              v-if="task.status === 'Pending'"
              @click="updateTaskStatus(task, 'InProgress')"
              class="btn-action"
            >
              Start
            </button>
            <button
              v-if="task.status === 'InProgress'"
              @click="updateTaskStatus(task, 'Completed')"
              class="btn-action btn-complete"
            >
              Finish
            </button>
            <span v-if="task.status === 'Completed'" class="status-badge">
              Completed
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, computed } from 'vue';
import { MobileWorkItemService, WorkItem } from '@/services/workItemService';
import { signalRService } from '@/services/signalRService';
import { authService } from '@/services/authService';
import { notificationService } from '@/services/notificationService';
import { ConnectionStatus } from '@/components/ConnectionStatus.vue';
import { db } from '@/db/schema';
import { syncEngine } from '@/sync/sync-engine';
import { LocalTask } from '@/db/schema';

const workItems = ref<WorkItem[]>([]);
const tasks = ref<LocalTask[]>([]);
const isSyncing = ref(false);
const activeFilter = ref('all');
const loading = ref<boolean>(true);
const error = ref<string | null>(null);

const filters = [
  { label: 'All Tasks', value: 'all' },
  { label: 'Venue', value: 'venue' },
  { label: 'Field', value: 'field' },
];

const sortedTasks = computed(() => {
  let filtered = [...tasks.value];

  if (activeFilter.value === 'venue') {
    filtered = filtered.filter(t => t.description.toLowerCase().includes('venue') || t.description.toLowerCase().includes('church'));
  } else if (activeFilter.value === 'field') {
    filtered = filtered.filter(t => !t.description.toLowerCase().includes('venue') && !t.description.toLowerCase().includes('church'));
  }

  const priorityMap = { Critical: 0, High: 1, Medium: 2, Low: 3 };
  return filtered.sort((a, b) => {
    if (a.status === 'Completed' && b.status !== 'Completed') return 1;
    if (a.status !== 'Completed' && b.status === 'Completed') return -1;
    return priorityMap[a.priority] - priorityMap[b.priority];
  });
});

const fetchWorkItems = async () => {
  try {
    loading.value = true;
    error.value = null;
    isSyncing.value = true;
    workItems.value = await MobileWorkItemService.getWorkItems();
  } catch (err) {
    error.value = 'Failed to load work items. Please try again later.';
    console.error('Error fetching work items:', err);
    // Fallback to mock data in case of API failure
    workItems.value = [
      { id: 1, title: 'Verify Death Certificate', description: 'Verify the uploaded death certificate for Case #C-1024', status: 'Pending', due: 'Today', caseId: 'C-1024', priority: 'High' },
      { id: 2, title: 'Schedule Burial Service', description: 'Coordinate with venue and transport for Case #C-1021', status: 'In Progress', due: 'Tomorrow', caseId: 'C-1021', priority: 'Medium' },
      { id: 3, title: 'Process Claim Payout', description: 'Verify benefits and initiate payout for Case #C-1018', status: 'Blocked', due: '2 days', caseId: 'C-1018', priority: 'High' },
    ];
  } finally {
    loading.value = false;
    isSyncing.value = false;
  }
};

const completeWorkItem = async (id: number) => {
  try {
    await MobileWorkItemService.completeWorkItem(id);
    // Notify via SignalR that we completed a work item
    signalRService.send('WorkItemCompleted', id);
    // Refresh the list after completion
    await fetchWorkItems();
  } catch (err) {
    error.value = 'Failed to complete work item. Please try again later.';
    console.error('Error completing work item:', err);
  }
};

async function loadTasks() {
  try {
    tasks.value = await db.tasks.toArray();
  } catch (err) {
    console.error('Error loading tasks from local DB:', err);
  }
}

async function updateTaskStatus(task: LocalTask, newStatus: LocalTask['status']) {
  try {
    await syncEngine.executeCommand({
      entityType: 'Task',
      entityId: task.id,
      action: 'UPDATE',
      payload: { status: newStatus },
    });
    await loadTasks();
  } catch (err) {
    console.error('Error updating task status:', err);
  }
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString([], { month: 'short', day: 'numeric' });
}

// Real-time update handlers
const handleWorkItemUpdated = (updatedWorkItem: WorkItem) => {
  // Find and update the specific work item
  const index = workItems.value.findIndex(item => item.id === updatedWorkItem.id);
  if (index !== -1) {
    workItems.value[index] = { ...workItems.value[index], ...updatedWorkItem };
    notificationService.addNotification({
      title: 'Work Item Updated',
      message: `Work item "${updatedWorkItem.title}" has been updated`,
      type: 'info'
    });
  }
};

const handleWorkItemCreated = (newWorkItem: WorkItem) => {
  // Add new work item to the list
  workItems.value = [...workItems.value, newWorkItem];
  notificationService.addNotification({
    title: 'New Work Item',
    message: `New work item "${newWorkItem.title}" has been created`,
    type: 'success'
  });
};

const handleWorkItemDeleted = (workItemId: number) => {
  // Remove work item from the list
  workItems.value = workItems.value.filter(item => item.id !== workItemId);
  notificationService.addNotification({
    title: 'Work Item Removed',
    message: `Work item #${workItemId} has been removed`,
    type: 'warning'
  });
};

const handleWorkItemCompleted = (completedWorkItem: WorkItem) => {
  // Find and update the specific work item
  const index = workItems.value.findIndex(item => item.id === completedWorkItem.id);
  if (index !== -1) {
    workItems.value[index] = { ...workItems.value[index], ...completedWorkItem };
    notificationService.addNotification({
      title: 'Work Item Completed',
      message: `Work item "${completedWorkItem.title}" has been marked as complete`,
      type: 'success'
    });
  }
};

onMounted(async () => {
  await loadTasks();
  await fetchWorkItems(); // Fetch from API as well
  
  // Set up SignalR listeners for real-time updates
  const workItemCleanup = MobileWorkItemService.subscribeToWorkItemUpdates(
    handleWorkItemCreated,
    handleWorkItemUpdated,
    handleWorkItemDeleted,
    handleWorkItemCompleted
  );
  
  // Start SignalR connection
  signalRService.start().catch(err => {
    console.error('Failed to start SignalR connection:', err);
    notificationService.addNotification({
      title: 'Connection Issue',
      message: 'Unable to connect to real-time updates. Some features may not be live.',
      type: 'warning'
    });
  });
  
  // Periodically refresh tasks from local DB
  setInterval(loadTasks, 5000);
  // Periodically refresh from API
  setInterval(fetchWorkItems, 30000);
});

onBeforeUnmount(() => {
  // Cleanup SignalR subscriptions
  if (workItemCleanup) workItemCleanup();
  
  // Stop SignalR connection
  signalRService.stop();
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
  align-items: center;
}

.page-header .sync-status {
  font-size: 0.75rem;
  padding: 4px 8px;
  border-radius: 12px;
  background: var(--khet-surface);
  color: var(--khet-text-muted);
  border: 1px solid var(--khet-border);
  transition: all 0.3s;
}

.page-header .sync-status.syncing {
  color: var(--khet-primary);
  border-color: var(--khet-primary);
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