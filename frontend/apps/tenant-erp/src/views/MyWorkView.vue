<template>
  <div class="view-container">
    <div class="view-header">
      <h1>My Work</h1>
      <p>Your assigned tasks, due items and critical actions.</p>
    </div>

    <div v-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-if="loading" class="loading-indicator">
      <div class="spinner"></div>
      <p>Loading work items...</p>
    </div>

    <div v-else class="work-grid">
      <!-- Work Items from API -->
      <KCard v-for="item in workItems" :key="item.id" elevation="sm" class="work-card">
        <template #header>
          <div class="card-header">
            <span class="status-pill" :class="item.status">{{ item.status }}</span>
            <span class="due-date">Due: {{ item.due }}</span>
          </div>
        </template>
        <div class="card-body">
          <h3>{{ item.title }}</h3>
          <p>{{ item.description }}</p>
          <div class="card-meta">
            <span>Case: {{ item.caseId }}</span>
            <span>Priority: {{ item.priority }}</span>
          </div>
        </div>
        <template #footer>
          <div class="card-actions">
            <KButton variant="secondary" size="sm">View Details</KButton>
            <KButton variant="primary" size="sm" @click="completeWorkItem(item.id)">Complete Task</KButton>
          </div>
        </template>
      </KCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { KButton, KCard } from '@khet360/ui-shared';
import { WorkItemService, WorkItem } from '@/services/workItemService';

const workItems = ref<WorkItem[]>([]);
const loading = ref<boolean>(true);
const error = ref<string | null>(null);

const fetchWorkItems = async () => {
  try {
    loading.value = true;
    error.value = null;
    workItems.value = await WorkItemService.getWorkItems();
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
  }
};

const completeWorkItem = async (id: number) => {
  try {
    await WorkItemService.completeWorkItem(id);
    // Refresh the list after completion
    await fetchWorkItems();
  } catch (err) {
    error.value = 'Failed to complete work item. Please try again later.';
    console.error('Error completing work item:', err);
  }
};

onMounted(() => {
  fetchWorkItems();
});
</script>

<style scoped>
.view-container {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.view-header {
  margin-bottom: 1rem;
}

.view-header h1 {
  font-size: 2rem;
  margin: 0 0 0.5rem 0;
  color: var(--khet-text-main);
}

.view-header p {
  color: var(--khet-text-muted);
  font-size: 1.1rem;
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

.work-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 1.5rem;
}

.work-card {
  display: flex;
  flex-direction: column;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.status-pill {
  font-size: 0.7rem;
  font-weight: 700;
  text-transform: uppercase;
  padding: 2px 8px;
  border-radius: 12px;
  background-color: var(--khet-surface-alt);
  color: var(--khet-text-muted);
}

.status-pill.Pending { background-color: #fff3cd; color: #856404; }
.status-pill.InProgress { background-color: #d1ecf1; color: #0c5460; }
.status-pill.Blocked { background-color: #f8d7da; color: #721c24; }
.status-pill.Completed { background-color: #d4edda; color: #155724; }

.due-date {
  font-size: 0.8rem;
  color: var(--khet-text-muted);
}

.card-body h3 {
  font-size: 1.1rem;
  margin: 0 0 0.5rem 0;
}

.card-body p {
  font-size: 0.9rem;
  color: var(--khet-text-muted);
  margin-bottom: 1rem;
}

.card-meta {
  display: flex;
  gap: 1rem;
  font-size: 0.8rem;
  color: var(--khet-text-muted);
}

.card-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
}
</style>