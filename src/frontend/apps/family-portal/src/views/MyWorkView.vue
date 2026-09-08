<template>
  <div class="page">
    <header class="page-header">
      <h1 class="title">My Tasks</h1>
      <div class="header-actions">
        <ConnectionStatus />
        <KButton @click="refreshTasks" variant="outline" size="sm">
          Refresh
        </KButton>
      </div>
    </header>

    <div v-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-if="loading" class="loading-indicator">
      <div class="spinner"></div>
      <p>Loading tasks...</p>
    </div>

    <div class="filters-section">
      <div class="filter-group">
        <label for="search" class="form-label">Search Tasks</label>
        <KInput
          v-model="searchTerm"
          placeholder="Search by description or case number..."
          class="w-full"
        />
      </div>
      
      <div class="filter-group">
        <label for="status" class="form-label">Status</label>
        <KSelect
          v-model="selectedStatus"
          :options="statusOptions"
          placeholder="All Statuses"
          class="w-full"
        />
      </div>
    </div>

    <div v-if="filteredTasks.length === 0 && !loading" class="empty-state">
      <span class="emoji">🎉</span>
      <p>All caught up! No pending tasks.</p>
    </div>

    <div class="tasks-table">
      <table class="tasks-table">
        <thead>
          <tr>
            <th>Case #</th>
            <th>Task Description</th>
            <th>Assigned To</th>
            <th>Due Date</th>
            <th>Priority</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="task in filteredTasks" :key="task.id" class="table-row">
            <td>{{ task.caseId }}</td>
            <td>{{ task.description }}</td>
            <td>{{ task.assignedTo || '-' }}</td>
            <td>{{ formatDate(task.dueDate) }}</td>
            <td>
              <span 
                class="priority-badge" 
                :class="[task.priority.toLowerCase()]"
              >
                {{ task.priority }}
              </span>
            </td>
            <td>
              <span 
                class="status-badge" 
                :class="[task.status.toLowerCase().replace(' ', '-')]"
              >
                {{ task.status }}
              </span>
            </td>
            <td class="actions-cell">
              <KButton 
                variant="secondary" 
                size="sm" 
                @click="editTask(task)"
                class="mr-2"
              >
                Edit
              </KButton>
              <KButton 
                variant="success" 
                size="sm" 
                @click="completeTask(task.id)"
              >
                Complete
              </KButton>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, computed } from 'vue';
import { KButton, KInput, KSelect, KDialog } from '@khet360/ui-shared';
import { signalRService } from '@/services/signalRService';
import { notificationService } from '@/services/notificationService';
import { ConnectionStatus } from '@/components/ConnectionStatus.vue';
import { db } from '@/db/schema';
import { Task } from '@/db/schema';

const tasks = ref<Task[]>([]);
const filteredTasks = ref<Task[]>([]);
const loading = ref<boolean>(true);
const error = ref<string | null>(null);

const searchTerm = ref('');
const selectedStatus = ref<string | null>(null);
const statusOptions = ['Pending', 'In Progress', 'Completed', 'Blocked', 'Cancelled'];

const fetchTasks = async () => {
  try {
    loading.value = true;
    error.value = null;
    tasks.value = await db.tasks.toArray();
  } catch (err) {
    error.value = 'Failed to load tasks. Please try again later.';
    console.error('Error fetching tasks:', err);
    // Fallback to mock data
    tasks.value = [
      { id: 1, caseId: 'C-1024', description: 'Verify death certificate', assignedTo: 'John Doe', dueDate: '2026-09-09', priority: 'High', status: 'Pending' },
      { id: 2, caseId: 'C-1021', description: 'Schedule burial service', assignedTo: 'Jane Smith', dueDate: '2026-09-10', priority: 'Medium', status: 'In Progress' },
      { id: 3, caseId: 'C-1018', description: 'Process insurance claim', assignedTo: 'Bob Johnson', dueDate: '2026-09-15', priority: 'High', status: 'Blocked' },
      { id: 4, caseId: 'C-1030', description: 'Arrange transportation', assignedTo: 'Alice Brown', dueDate: '2026-09-08', priority: 'Low', status: 'Completed' }
    ];
  } finally {
    loading.value = false;
  }
};

const refreshTasks = async () => {
  await fetchTasks();
};

// Real-time update handlers
const handleTaskCreated = (task: Task) => {
  tasks.value = [...tasks.value, task];
  notificationService.addNotification({
    title: 'New Task Created',
    message: `New task "${task.description}" for case #${task.caseId} has been created`,
    type: 'success'
  });
};

const handleTaskUpdated = (task: Task) => {
  const index = tasks.value.findIndex(t => t.id === task.id);
  if (index !== -1) {
    tasks.value[index] = { ...tasks.value[index], ...task };
    notificationService.addNotification({
      title: 'Task Updated',
      message: `Task "${task.description}" has been updated`,
      type: 'info'
    });
  }
};

const handleTaskDeleted = (taskId: number) => {
  tasks.value = tasks.value.filter(t => t.id !== taskId);
  notificationService.addNotification({
    title: 'Task Deleted',
    message: `Task #${taskId} has been removed`,
    type: 'warning'
  });
};

const handleTaskCompleted = (task: Task) => {
  const index = tasks.value.findIndex(t => t.id === task.id);
  if (index !== -1) {
    tasks.value[index] = { ...tasks.value[index], ...task };
    notificationService.addNotification({
      title: 'Task Completed',
      message: `Task "${task.description}" for case #${task.caseId} has been completed`,
      type: 'success'
    });
  }
};

const editTask = (task: Task) => {
  // In a real implementation, this would open an edit dialog
  alert(`Editing task: ${task.description}`);
};

const completeTask = async (taskId: number) => {
  try {
    await db.tasks.update(taskId, { status: 'Completed' });
    // Notify via SignalR that we completed a task
    signalRService.send('TaskCompleted', { id: taskId, status: 'Completed' });
    
    await fetchTasks();
  } catch (err) {
    error.value = 'Failed to complete task. Please try again later.';
    console.error('Error completing task:', err);
  }
};

const formatDate = (dateStr: string) => {
  if (!dateStr) return 'No date';
  return new Date(dateStr).toLocaleDateString([], { month: 'short', day: 'numeric' });
};

// Computed property for filtered tasks
computed(() => {
  return tasks.value.filter(task => {
    const matchesSearch = task.description.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
                         task.caseId.toLowerCase().includes(searchTerm.value.toLowerCase());
    
    const matchesStatus = !selectedStatus.value || task.status === selectedStatus.value;
    
    return matchesSearch && matchesStatus;
  });
});

onMounted(() => {
  // Fetch initial data
  fetchTasks();
  
  // Set up SignalR listeners for real-time updates
  const taskCleanup = db.tasks.subscribeToChanges(
    handleTaskCreated,
    handleTaskUpdated,
    handleTaskDeleted,
    handleTaskCompleted
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
});

onBeforeUnmount(() => {
  // Cleanup SignalR subscriptions
  if (taskCleanup) taskCleanup();
  
  // Note: We don't stop the SignalR connection here as other components might still need it
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
  justify-center: center;
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

.filters-section {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-label {
  font-weight: 600;
  font-size: 0.875rem;
  margin-bottom: 0.25rem;
}

.empty-state {
  text-align: center;
  padding: 3rem;
  color: var(--khet-text-muted);
}

.empty-state .emoji {
  font-size: 3rem;
  display: block;
  margin-bottom: 1rem;
}

.tasks-table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
}

.tasks-table th,
.tasks-table td {
  padding: 1rem;
  text-align: left;
  border-bottom: 1px solid #f3f4f6;
}

.tasks-table th {
  background-color: #f8fafc;
  font-weight: 600;
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.table-row {
  transition: background-color 0.2s;
}

.table-row:hover {
  background-color: #f8fafc;
}

.priority-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
}

.priority-low {
  background-color: #e2e8f0;
  color: #64748b;
}

.priority-medium {
  background-color: #fef3c7;
  color: #d97706;
}

.priority-high {
  background-color: #fed7d7;
  color: #dc2626;
}

.priority-critical {
  background-color: #fecaca;
  color: #991b1b;
}

.status-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
}

.status-pending {
  background-color: #dbeafe;
  color: #2563eb;
}

.status-in-progress {
  background-color: #dcfce7;
  color: #16a34a;
}

.status-completed {
  background-color: #bbf7d0;
  color: #16a34a;
}

.status-blocked {
  background-color: #fecaca;
  color: #991b1b;
}

.status-cancelled {
  background-color: #f3f4f6;
  color: #6b7280;
}

.actions-cell {
  display: flex;
  gap: 0.5rem;
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
</style>