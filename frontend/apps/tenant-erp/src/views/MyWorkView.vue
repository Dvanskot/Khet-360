<template>
  <div class="view-container">
    <div class="view-header">
      <h1>My Work</h1>
      <p>Your assigned tasks, due items and critical actions.</p>
    </div>

    <div class="work-grid">
      <!-- Mock Work Items -->
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
            <KButton variant="primary" size="sm">Complete Task</KButton>
          </div>
        </template>
      </KCard>
    </div>
  </template>

  <script setup lang="ts">
  import { KButton, KCard } from '@khet360/ui-shared';

  const workItems = [
    { id: 1, title: 'Verify Death Certificate', description: 'Verify the uploaded death certificate for Case #C-1024', status: 'Pending', due: 'Today', caseId: 'C-1024', priority: 'High' },
    { id: 2, title: 'Schedule Burial Service', description: 'Coordinate with venue and transport for Case #C-1021', status: 'In Progress', due: 'Tomorrow', caseId: 'C-1021', priority: 'Medium' },
    { id: 3, title: 'Process Claim Payout', description: 'Verify benefits and initiate payout for Case #C-1018', status: 'Blocked', due: '2 days', caseId: 'C-1018', priority: 'High' },
  ];
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
</template>
