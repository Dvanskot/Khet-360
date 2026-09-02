<template>
  <div class="view-container">
    <div class="view-header">
      <h1>Team Queue</h1>
      <p>Unassigned work items available for your team.</p>
    </div>

    <div class="queue-table-container">
      <table class="queue-table">
        <thead>
          <tr>
            <th>Case ID</th>
            <th>Work Item</th>
            <th>Priority</th>
            <th>SLA Status</th>
            <th>Created</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in queueItems" :key="item.id">
            <td>{{ item.caseId }}</td>
            <td>
              <div class="item-cell">
                <span class="item-title">{{ item.title }}</span>
                <span class="item-desc">{{ item.description }}</span>
              </div>
            </td>
            <td>
              <span :class="['priority-badge', item.priority]">{{ item.priority }}</span>
            </td>
            <td>
              <span :class="['sla-badge', item.slaStatus]">{{ item.slaStatus }}</span>
            </td>
            <td>{{ item.created }}</td>
            <td>
              <KButton variant="primary" size="sm" @click="claimItem(item.id)">Claim</KButton>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </template>

  <script setup lang="ts">
  import { KButton } from '@khet360/ui-shared';

  const queueItems = [
    { id: 101, caseId: 'C-2001', title: 'Review Documents', description: 'Missing ID for spouse', priority: 'Medium', slaStatus: 'Normal', created: '2 hours ago' },
    { id: 102, caseId: 'C-2005', title: 'Confirm Venue', description: 'Awaiting response from cemetery', priority: 'High', slaStatus: 'Warning', created: '5 hours ago' },
    { id: 103, caseId: 'C-2010', title: 'Draft Invoice', description: 'Finalize charges for burial plan', priority: 'Low', slaStatus: 'Normal', created: '1 day ago' },
  ];

  const claimItem = (id: number) => {
    alert(`Item ${id} claimed successfully!`);
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

  .view-header h1 {
    font-size: 2rem;
    margin: 0 0 0.5rem 0;
  }

  .view-header p {
    color: var(--khet-text-muted);
    font-size: 1.1rem;
  }

  .queue-table-container {
    background-color: white;
    border: 1px solid var(--khet-border);
    border-radius: var(--khet-radius-md);
    overflow: hidden;
  }

  .queue-table {
    width: 100%;
    border-collapse: collapse;
    text-align: left;
  }

  .queue-table th {
    background-color: var(--khet-surface-alt);
    padding: 1rem;
    font-size: 0.85rem;
    font-weight: 600;
    color: var(--khet-text-muted);
    border-bottom: 1px solid var(--khet-border);
  }

  .queue-table td {
    padding: 1rem;
    border-bottom: 1px solid var(--khet-border);
    font-size: 0.9rem;
    vertical-align: middle;
  }

  .item-cell {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .item-title {
    font-weight: 600;
    color: var(--khet-text-main);
  }

  .item-desc {
    font-size: 0.8rem;
    color: var(--khet-text-muted);
  }

  .priority-badge {
    font-size: 0.75rem;
    padding: 2px 8px;
    border-radius: 4px;
    font-weight: 600;
  }

  .priority-badge.High { background-color: #f8d7da; color: #721c24; }
  .priority-badge.Medium { background-color: #fff3cd; color: #856404; }
  .priority-badge.Low { background-color: #d1ecf1; color: #0c5460; }

  .sla-badge {
    font-size: 0.75rem;
    font-weight: 600;
  }

  .sla-badge.Normal { color: var(--khet-success); }
  .sla-badge.Warning { color: #e67e22; }
  .sla-badge.Breached { color: red; font-weight: 800; }
  </style>
</template>
