<template>
  <div class="view-container">
    <div class="view-header">
      <h1 class="exceptions-title">Exceptions</h1>
      <p>SLA breaches, conflicts, and items awaiting critical approval.</p>
    </div>

    <div class="exception-grid">
      <KCard v-for="exception in exceptions" :key="exception.id" elevation="lg" class="exception-card">
        <template #header>
          <div class="exception-header">
            <span class="type-pill" :class="exception.type">{{ exception.type }}</span>
            <span class="severity-pill" :class="exception.severity">{{ exception.severity }}</span>
          </div>
        </template>
        <div class="exception-body">
          <h3>{{ exception.title }}</h3>
          <p>{{ exception.description }}</p>
          <div class="exception-meta">
            <span>Case: {{ exception.caseId }}</span>
            <span>SLA: {{ exception.slaImpact }}</span>
          </div>
        </div>
        <template #footer>
          <div class="exception-actions">
            <KButton variant="secondary" size="sm">Dismiss</KButton>
            <KButton variant="primary" size="sm" @click="resolveException(exception.id)">Resolve Now</KButton>
          </div>
        </template>
      </KCard>
    </div>
  </template>

  <script setup lang="ts">
  import { KButton, KCard } from '@khet360/ui-shared';

  const exceptions = [
    { id: 1, title: 'SLA Breach: Case Setup', description: 'Case #C-1001 has exceeded the 4-hour setup SLA.', type: 'SLA Breach', severity: 'Critical', caseId: 'C-1001', slaImpact: 'Overdue by 2h' },
    { id: 2, title: 'Document Conflict', description: 'Two different death certificates uploaded for Case #C-1005', type: 'Conflict', severity: 'High', caseId: 'C-1005', slaImpact: 'Blocked' },
    { id: 3, title: 'Approval Required', description: 'Payout for Case #C-1012 exceeds branch manager limit', type: 'Approval', severity: 'Medium', caseId: 'C-1012', slaImpact: 'Pending' },
  ];

  const resolveException = (id: number) => {
    alert(`Exception ${id} resolution workflow started.`);
  };
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

  .exceptions-title {
    font-size: 2rem;
    color: #c0392b;
    margin: 0 0 0.5rem 0;
  }

  .view-header p {
    color: var(--khet-text-muted);
    font-size: 1.1rem;
  }

  .exception-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
    gap: 1.5rem;
  }

  .exception-card {
    border-left: 5px solid #c0392b;
  }

  .exception-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .type-pill, .severity-pill {
    font-size: 0.7rem;
    font-weight: 700;
    text-transform: uppercase;
    padding: 2px 8px;
    border-radius: 12px;
    background-color: var(--khet-surface-alt);
    color: var(--khet-text-muted);
  }

  .severity-pill.Critical { background-color: #c0392b; color: white; }
  .severity-pill.High { background-color: #e67e22; color: white; }
  .severity-pill.Medium { background-color: #f1c40f; color: #000; }

  .exception-body h3 {
    font-size: 1.1rem;
    margin: 0 0 0.5rem 0;
  }

  .exception-body p {
    font-size: 0.9rem;
    color: var(--khet-text-muted);
    margin-bottom: 1rem;
  }

  .exception-meta {
    display: flex;
    gap: 1rem;
    font-size: 0.8rem;
    color: var(--khet-text-muted);
  }

  .exception-actions {
    display: flex;
    justify-content: flex-end;
    gap: 0.5rem;
  }
  </style>
</template>
