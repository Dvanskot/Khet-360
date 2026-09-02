<template>
  <div class="fleet-dashboard">
    <div class="view-header">
      <div class="header-left">
        <h1>Fleet Management</h1>
        <p>Vehicle dispatch, driver assignments and fleet health.</p>
      </div>
      <div class="header-actions">
        <KButton variant="secondary" @click="$router.push('/fleet/vehicles')">Manage Fleet</KButton>
        <KButton variant="primary" @click="$router.push('/fleet/dispatch')">Dispatch Board</KButton>
      </div>
    </div>

    <div class="fleet-stats-grid">
      <KCard elevation="sm" class="stat-card">
        <div class="stat-icon">🚚</div>
        <div class="stat-details">
          <span class="stat-label">Total Vehicles</span>
          <span class="stat-value">12</span>
        </div>
      </KCard>
      <KCard elevation="sm" class="stat-card">
        <div class="stat-icon">🟢</div>
        <div class="stat-details">
          <span class="stat-label">Available Now</span>
          <span class="stat-value">8</span>
        </div>
      </KCard>
      <KCard elevation="sm" class="stat-card">
        <div class="stat-icon">🟡</div>
        <div class="stat-details">
          <span class="stat-label">In Transit</span>
          <span class="stat-value">3</span>
        </div>
      </KCard>
      <KCard elevation="sm" class="stat-card">
        <div class="stat-icon">🔴</div>
        <div class="stat-details">
          <span class="stat-label">Maintenance</span>
          <span class="stat-value">1</span>
        </div>
      </KCard>
    </div>

    <div class="fleet-main-grid">
      <!-- Active Dispatches -->
      <div class="dispatch-section">
        <KCard elevation="sm">
          <template #header>
            <div class="section-header">
              <strong>Active Dispatches</strong>
              <KButton variant="secondary" size="sm" @click="$router.push('/fleet/dispatch')">View All</KButton>
            </div>
          </template>
          <div class="dispatch-list">
            <div v-for="task in activeTasks" :key="task.id" class="dispatch-row">
              <div class="task-info">
                <span class="task-id">{{ task.id }}</span>
                <span class="task-case">Case {{ task.caseId }}</span>
              </div>
              <div class="task-route">
                <span class="route-from">{{ task.origin }}</span>
                <span class="route-arrow">→</span>
                <span class="route-to">{{ task.destination }}</span>
              </div>
              <div class="task-meta">
                <span class="driver">{{ task.driver }}</span>
                <span class="vehicle">{{ task.vehicle }}</span>
              </div>
              <div class="task-status">
                <span class="pill" :class="task.status">{{ task.status }}</span>
              </div>
            </div>
          </div>
        </KCard>
      </div>

      <!-- Vehicle Health -->
      <div class="health-section">
        <KCard elevation="sm">
          <template #header>
            <div class="section-header">
              <strong>Fleet Health Alerts</strong>
            </div>
          </template>
          <div class="health-list">
            <div v-for="alert in healthAlerts" :key="alert.id" class="health-item">
              <span class="alert-icon" :class="alert.severity">{{ alert.severity === 'Critical' ? '❌' : '⚠️' }}</span>
              <div class="alert-text">
                <span class="alert-title">{{ alert.vehicle }}</span>
                <span class="alert-desc">{{ alert.message }}</span>
              </div>
              <span class="alert-date">{{ alert.date }}</span>
            </div>
          </div>
        </KCard>
      </div>
    </div>
  </template>

  <script setup lang="ts">
  import { ref } from 'vue';
  import { KButton, KCard } from '@khet360/ui-shared';

  const activeTasks = ref([
    { id: 'D-101', caseId: 'C-1024', origin: 'Cape Town Central', destination: 'Constantia', driver: 'Peter Pan', vehicle: 'Hearse A', status: 'InTransit' },
    { id: 'D-102', caseId: 'C-1021', origin: 'Hospitals', destination: 'Central Mortuary', driver: 'Lindiwe K.', vehicle: 'Hearse B', status: 'Dispatched' },
    { id: 'D-103', caseId: 'C-1018', origin: 'Home', destination: 'Central Mortuary', driver: 'John Doe', vehicle: 'Limousine 1', status: 'Scheduled' },
  ]);

  const healthAlerts = ref([
    { id: 1, vehicle: 'Hearse A', message: 'Service overdue by 500km', severity: 'Warning', date: '2 days ago' },
    { id: 2, vehicle: 'Limousine 1', message: 'License expires in 10 days', severity: 'Critical', date: 'Today' },
  ]);
  </script>

  <style scoped>
  .fleet-dashboard {
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

  .fleet-stats-grid {
    display: grid;
    grid-template-columns: repeat(4, 1fr);
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

  .fleet-main-grid {
    display: grid;
    grid-template-columns: 1.5fr 1fr;
    gap: 2rem;
  }

  .section-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .dispatch-list {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
    padding: 1rem 0;
  }

  .dispatch-row {
    display: grid;
    grid-template-columns: 120px 1fr 200px 100px;
    align-items: center;
    padding: 1rem;
    border: 1px solid var(--khet-border);
    border-radius: 8px;
    font-size: 0.9rem;
  }

  .task-info {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .task-id {
    font-weight: 700;
    color: var(--khet-text-main);
  }

  .task-case {
    font-size: 0.75rem;
    color: var(--khet-text-muted);
  }

  .task-route {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    justify-content: center;
    color: var(--khet-text-main);
  }

  .route-arrow {
    color: var(--khet-primary);
    font-weight: 800;
  }

  .task-meta {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
    align-items: flex-end;
    text-align: right;
  }

  .driver, .vehicle {
    font-size: 0.85rem;
    color: var(--khet-text-main);
  }

  .task-status {
    text-align: right;
  }

  .pill {
    font-size: 0.7rem;
    padding: 2px 8px;
    border-radius: 12px;
    font-weight: 600;
    background-color: var(--khet-surface-alt);
  }

  .pill.InTransit { background-color: #d1ecf1; color: #0c5460; }
  .pill.Dispatched { background-color: #fff3cd; color: #856404; }
  .pill.Scheduled { background-color: #e2e3e5; color: #383d41; }

  .health-list {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    padding: 1rem 0;
  }

  .health-item {
    display: flex;
    align-items: center;
    gap: 1rem;
    padding: 1rem;
    background-color: var(--khet-surface-alt);
    border-radius: 8px;
  }

  .alert-icon {
    font-size: 1.2rem;
  }

  .alert-text {
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .alert-title {
    font-weight: 600;
    font-size: 0.9rem;
  }

  .alert-desc {
    font-size: 0.8rem;
    color: var(--khet-text-muted);
  }

  .alert-date {
    font-size: 0.75rem;
    color: var(--khet-text-muted);
  }
  </style>
</template>
