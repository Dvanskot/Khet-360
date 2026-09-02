<template>
  <div class="page">
    <header class="page-header">
      <h1 class="title">My Work</h1>
      <div class="sync-status" :class="{ syncing: isSyncing }">
        {{ isSyncing ? 'Syncing...' : 'Synced' }}
      </div>
    </header>

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
      <div v-if="tasks.length === 0" class="empty-state">
        <span class="emoji">🎉</span>
        <p>All caught up! No pending tasks.</p>
      </div>

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
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { db } from '@/db/schema';
import { syncEngine } from '@/sync/sync-engine';
import { LocalTask } from '@/db/schema';

const tasks = ref<LocalTask[]>([]);
const isSyncing = ref(false);
const activeFilter = ref('all');

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

async function loadTasks() {
  tasks.value = await db.tasks.toArray();
}

async function updateTaskStatus(task: LocalTask, newStatus: LocalTask['status']) {
  await syncEngine.executeCommand({
    entityType: 'Task',
    entityId: task.id,
    action: 'UPDATE',
    payload: { status: newStatus },
  });
  await loadTasks();
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString([], { month: 'short', day: 'numeric' });
}

onMounted(async () => {
  await loadTasks();
  // Periodically refresh tasks from local DB
  setInterval(loadTasks, 5000);
});
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

.sync-status {
  font-size: 0.75rem;
  padding: 4px 8px;
  border-radius: 12px;
  background: var(--khet-surface);
  color: var(--khet-text-muted);
  border: 1px solid var(--khet-border);
  transition: all 0.3s;
}

.sync-status.syncing {
  color: var(--khet-primary);
  border-color: var(--khet-primary);
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
