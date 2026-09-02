<template>
  <div class="view-container">
    <div class="view-header">
      <div class="header-left">
        <h1>Dispatch Board</h1>
        <p>Assign vehicles and drivers to funeral case movements.</p>
      </div>
      <div class="header-actions">
        <KButton variant="primary" @click="openDispatchWizard">➕ New Dispatch</KButton>
      </div>
    </div>

    <div class="dispatch-grid">
      <div class="active-dispatches">
        <KCard elevation="sm">
          <template #header><strong>Live Movements</strong></template>
          <div class="dispatch-table">
            <table>
              <thead>
                <tr>
                  <th>Task ID</th>
                  <th>Case</th>
                  <th>Route</th>
                  <th>Driver / Vehicle</th>
                  <th>Status</th>
                  <th>Action</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="task in activeTasks" :key="task.id">
                  <td>{{ task.id }}</td>
                  <td>{{ task.caseId }}</td>
                  <td>{{ task.origin }} → {{ task.destination }}</td>
                  <td>{{ task.driver }} / {{ task.vehicle }}</td>
                  <td><span class="pill" :class="task.status">{{ task.status }}</span></td>
                  <td><KButton variant="secondary" size="sm">Update</KButton></td>
                </tr>
              </tbody>
            </table>
          </div>
        </KCard>
      </div>

      <div class="pending-requests">
        <KCard elevation="sm">
          <template #header><strong>Pending Requests</strong></template>
          <div class="request-list">
            <div v-for="req in pendingRequests" :key="req.id" class="request-item">
              <div class="req-info">
                <span class="req-case">Case #{{ req.caseId }}</span>
                <span class="req-route">{{ req.origin }} → {{ req.destination }}</span>
              </div>
              <div class="req-actions">
                <KButton variant="primary" size="sm" @click="assignResource(req.id)">Assign</KButton>
              </div>
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
  ]);

  const pendingRequests = ref([
    { id: 'R-501', caseId: 'C-1018', origin: 'Home', destination: 'Central Mortuary' },
    { id: 'R-502', caseId: 'C-1005', origin: 'Mortuary', destination: 'Graveyard', },
  ]);

  const assignResource = (id: string) => alert(`Opening resource assignment for request ${id}...`);
  const openDispatchWizard = () => alert('Opening Dispatch Wizard...');
  </script>

  <style scoped>
  .view-container {
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

  .dispatch-grid {
    display: grid;
    grid-template-columns: 2fr 1fr;
    gap: 2rem;
  }

  .dispatch-table {
    overflow-x: auto;
  }

  .dispatch-table table {
    width: 100%;
    border-collapse: collapse;
    text-align: left;
  }

  .dispatch-table th {
    background-color: var(--khet-surface-alt);
    padding: 1rem;
    font-size: 0.85rem;
    font-weight: 600;
    color: var(--khet-text-muted);
    border-bottom: 1px solid var(--khet-border);
  }

  .dispatch-table td {
    padding: 1rem;
    border-bottom: 1px solid var(--khet-border);
    font-size: 0.9rem;
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

  .request-list {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    padding: 1rem 0;
  }

  .request-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem;
    border: 1px solid var(--khet-border);
    border-radius: 8px;
  }

  .req-info {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .req-case {
    font-weight: 600;
    font-size: 0.9rem;
  }

  .req-route {
    font-size: 0.8rem;
    color: var(--khet-text-muted);
  }
  </style>
</template>
