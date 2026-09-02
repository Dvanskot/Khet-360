<template>
  <div class="hr-dashboard">
    <div class="view-header">
      <div class="header-left">
        <h1>HR & People Management</h1>
        <p>Employee records, leave tracking and payroll administration.</p>
      </div>
      <div class="header-actions">
        <KButton variant="secondary" @click="$router.push('/hr/employees')">Manage Employees</KButton>
        <KButton variant="primary" @click="$router.push('/hr/payroll')">Run Payroll</KButton>
      </div>
    </div>

    <div class="hr-stats-grid">
      <KCard elevation="sm" class="stat-card">
        <div class="stat-icon">👥</div>
        <div class="stat-details">
          <span class="stat-label">Total Headcount</span>
          <span class="stat-value">84</span>
        </div>
      </KCard>
      <KCard elevation="sm" class="stat-card">
        <div class="stat-icon">🌴</div>
        <div class="stat-details">
          <span class="stat-label">Leave Requests</span>
          <span class="stat-value">12</span>
          <span class="stat-sub">Awaiting Approval</span>
        </div>
      </KCard>
      <KCard elevation="sm" class="stat-card">
        <div class="stat-icon">💰</div>
        <div class="stat-details">
          <span class="stat-label">Payroll Status</span>
          <span class="stat-value payroll-status">Draft</span>
          <span class="stat-sub">September 2026 Run</span>
        </div>
      </KCard>
    </div>

    <div class="hr-main-grid">
      <!-- Leave Approvals -->
      <div class="leave-section">
        <KCard elevation="sm">
          <template #header>
            <div class="section-header">
              <strong>Pending Leave Applications</strong>
              <KButton variant="secondary" size="sm">View All</KButton>
            </div>
          </template>
          <div class="leave-list">
            <div v-for="app in leaveApps" :key="app.id" class="leave-row">
              <div class="emp-info">
                <span class="emp-name">{{ app.employee }}</span>
                <span class="leave-type">{{ app.type }}</span>
              </div>
              <div class="leave-dates">
                {{ app.start }} to {{ app.end }}
              </div>
              <div class="leave-actions">
                <KButton variant="secondary" size="sm" @click="handleLeave(app.id, 'reject')">Reject</KButton>
                <KButton variant="primary" size="sm" @click="handleLeave(app.id, 'approve')">Approve</KButton>
              </div>
            </div>
          </div>
        </KCard>
      </div>

      <!-- Payroll Summary -->
      <div class="payroll-section">
        <KCard elevation="sm">
          <template #header>
            <div class="section-header">
              <strong>Current Payroll Preview</strong>
              <KButton variant="primary" size="sm" @click="$router.push('/hr/payroll')">Finalize Run</KButton>
            </div>
          </template>
          <div class="payroll-preview">
            <div class="preview-row">
              <span class="label">Total Gross Pay:</span>
              <span class="value">R 1,240,000.00</span>
            </div>
            <div class="preview-row">
              <span class="label">Total Deductions:</span>
              <span class="value neg">- R 185,000.00</span>
            </div>
            <div class="preview-divider"></div>
            <div class="preview-row total">
              <span class="label">Net Payable:</span>
              <span class="value">R 1,055,000.00</span>
            </div>
          </div>
        </KCard>
      </div>
    </div>
  </template>

  <script setup lang="ts">
  import { ref } from 'vue';
  import { KButton, KCard } from '@khet360/ui-shared';

  const leaveApps = ref([
    { id: 1, employee: 'Sarah Jenkins', type: 'Annual', start: '2026-09-15', end: '2026-09-20' },
    { id: 2, employee: 'Mike Ross', type: 'Sick', start: '2026-09-02', end: '2026-09-03' },
    { id: 3, employee: 'Lindiwe Khoza', type: 'Family Responsibility', start: '2026-09-10', end: '2026-09-11' },
  ]);

  const handleLeave = (id: number, action: string) => {
    alert(`Leave application ${id} ${action}ed.`);
  };
  </script>

  <style scoped>
  .hr-dashboard {
    display: flex;
    flex-direction: column;
    gap: 2rem;
  }

  .view-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .header-left h1 {
    font-size: 2rem;
    margin: 0 0 0.5rem 0;
  }

  .header-left p {
    color: var(--khet-text-muted);
    font-size: 1.1rem;
  }

  .hr-stats-grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 1.5rem;
  }

  .stat-card {
    padding: 1.5rem;
    display: flex;
    align-items: center;
    gap: 1.5rem;
  }

  .stat-icon {
    font-size: 2rem;
    background-color: var(--khet-primary-light);
    width: 60px;
    height: 60px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 50%;
  }

  .stat-details {
    display: flex;
    flex-direction: column;
  }

  .stat-label {
    font-size: 0.85rem;
    color: var(--khet-text-muted);
    font-weight: 500;
  }

  .stat-value {
    font-size: 1.5rem;
    font-weight: 800;
    color: var(--khet-text-main);
  }

  .stat-sub {
    font-size: 0.75rem;
    color: var(--khet-text-muted);
  }

  .payroll-status {
    color: #e67e22;
  }

  .hr-main-grid {
    display: grid;
    grid-template-columns: 1.2fr 0.8fr;
    gap: 2rem;
  }

  .section-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .leave-list {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
    padding: 1rem 0;
  }

  .leave-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem;
    border: 1px solid var(--khet-border);
    border-radius: 8px;
    transition: background 0.2s;
  }

  .leave-row:hover {
    background-color: var(--khet-surface-alt);
  }

  .emp-info {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .emp-name {
    font-weight: 600;
    font-size: 0.9rem;
  }

  .leave-type {
    font-size: 0.75rem;
    color: var(--khet-text-muted);
    background-color: var(--khet-surface-alt);
    padding: 2px 6px;
    border-radius: 4px;
    width: fit-content;
  }

  .leave-dates {
    font-size: 0.85rem;
    color: var(--khet-text-muted);
  }

  .leave-actions {
    display: flex;
    gap: 0.5rem;
  }

  .payroll-preview {
    padding: 1rem 0;
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  .preview-row {
    display: flex;
    justify-content: space-between;
    font-size: 0.9rem;
  }

  .preview-row .label {
    color: var(--khet-text-muted);
  }

  .preview-row .value {
    font-weight: 600;
  }

  .preview-row .value.neg {
    color: #c0392b;
  }

  .preview-divider {
    height: 1px;
    background-color: var(--khet-border);
    margin: 0.5rem 0;
  }

  .preview-row.total {
    display: flex;
    justify-content: space-between;
    font-size: 1.1rem;
    font-weight: 800;
    color: var(--khet-text-main);
  }
  </style>
</template>
