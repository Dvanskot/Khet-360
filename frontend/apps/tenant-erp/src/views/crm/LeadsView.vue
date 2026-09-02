<template>
  <div class="leads-view">
    <div class="view-header">
      <div class="header-left">
        <h1>Sales Pipeline</h1>
        <p>Manage leads and convert prospects into active customers.</p>
      </div>
      <div class="header-actions">
        <KButton variant="primary" @click="openCreateLeadModal">➕ Capture New Lead</KButton>
      </div>
    </div>

    <div class="filters-bar">
      <div class="filter-item">
        <span class="filter-label">Status:</span>
        <select v-model="filters.status">
          <option value="All">All Statuses</option>
          <option value="New">New</option>
          <option value="Contacted">Contacted</option>
          <option value="Qualified">Qualified</option>
          <option value="Converted">Converted</option>
        </select>
      </div>
      <div class="filter-item">
        <span class="filter-label">Priority:</span>
        <select v-model="filters.priority">
          <option value="All">All Priorities</option>
          <option value="High">High</option>
          <option value="Medium">Medium</option>
          <option value="Low">Low</option>
        </select>
      </div>
      <div class="filter-item search">
        <KInput v-model="filters.search" placeholder="Search leads..." />
      </div>
    </div>

    <div class="leads-table-container">
      <table class="leads-table">
        <thead>
          <tr>
            <th>Lead Name</th>
            <th>Source</th>
            <th>Status</th>
            <th>Priority</th>
            <th>Last Contact</th>
            <th>Assigned To</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="lead in filteredLeads" :key="lead.id">
            <td>
              <div class="lead-cell">
                <span class="lead-name">{{ lead.customerName }}</span>
                <span class="lead-contact">{{ lead.phone }}</span>
              </div>
            </td>
            <td>{{ lead.source }}</td>
            <td>
              <span :class="['status-pill', lead.status]">{{ lead.status }}</span>
            </td>
            <td>
              <span :class="['priority-pill', lead.priority]">{{ lead.priority }}</span>
            </td>
            <td>{{ lead.lastContactDate || 'Never' }}</td>
            <td>{{ lead.assignedTo }}</td>
            <td>
              <div class="action-group">
                <KButton variant="secondary" size="sm" @click="contactLead(lead.id)">📞 Contact</KButton>
                <KButton v-if="lead.status !== 'Converted'" variant="primary" size="sm" @click="convertLead(lead)">Convert</KButton>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </template>

  <script setup lang="ts">
  import { ref, computed } from 'vue';
  import { KButton, KInput } from '@khet360/ui-shared';
  import { Lead } from '@/components/crm/types';

  const filters = ref({
    status: 'All',
    priority: 'All',
    search: '',
  });

  const leads = ref<Lead[]>([
    { id: 'L1', source: 'Website', customerName: 'Thabo Mbeki', phone: '+27 83 111 2222', email: 'thabo@example.co.za', interest: 'Burial Plan', status: 'New', assignedTo: 'Sarah J.', createdDate: '2026-09-01', priority: 'High' },
    { id: 'L2', source: 'Referral', customerName: 'Nomvula Zulu', phone: '+27 71 333 4444', email: 'nomvula@example.co.za', interest: 'Cash Payout', status: 'Contacted', assignedTo: 'Sarah J.', createdDate: '2026-08-30', lastContactDate: '2026-08-31', priority: 'Medium' },
    { id: 'L3', source: 'Walk-in', customerName: 'Pieter Botha', phone: '+27 82 555 6666', email: 'pieter@example.co.za', interest: 'Premium Package', status: 'Qualified', assignedTo: 'Mike R.', createdDate: '2026-08-28', lastContactDate: '2026-08-29', priority: 'High' },
    { id: 'L4', source: 'Phone', customerName: 'Grace Khumalo', phone: '+27 72 777 8888', email: 'grace@example.co.za', interest: 'Burial Plan', status: 'Converted', assignedTo: 'Mike R.', createdDate: '2026-08-20', lastContactDate: '2026-08-22', priority: 'Low' },
  ]);

  const filteredLeads = computed(() => {
    return leads.value.filter(l => {
      const statusMatch = filters.value.status === 'All' || l.status === filters.value.status;
      const priorityMatch = filters.value.priority === 'All' || l.priority === filters.value.priority;
      const searchMatch = !filters.value.search ||
        l.customerName.toLowerCase().includes(filters.value.search.toLowerCase()) ||
        l.phone.includes(filters.value.search);
      return statusMatch && priorityMatch && searchMatch;
    });
  });

  const contactLead = (id: string) => alert(`Opening communication hub for Lead ${id}...`);
  const openCreateLeadModal = () => alert('Opening Quick Capture Lead form...');
  const convertLead = (lead: Lead) => {
    if (confirm(`Convert ${lead.customerName} to a full Customer?`)) {
      alert(`Lead ${lead.id} converted. Creating Customer profile...`);
    }
  };
  </script>

  <style scoped>
  .leads-view {
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

  .filters-bar {
    display: flex;
    gap: 1.5rem;
    align-items: center;
    background-color: white;
    padding: 1rem;
    border-radius: var(--khet-radius-md);
    border: 1px solid var(--khet-border);
  }

  .filter-item {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-size: 0.9rem;
  }

  .filter-label {
    color: var(--khet-text-muted);
    font-weight: 500;
  }

  .filter-item select {
    padding: 0.4rem;
    border: 1px solid var(--khet-border);
    border-radius: 4px;
    font-family: inherit;
  }

  .filter-item.search {
    margin-left: auto;
    flex: 1;
    max-width: 300px;
  }

  .leads-table-container {
    background-color: white;
    border: 1px solid var(--khet-border);
    border-radius: var(--khet-radius-md);
    overflow: hidden;
  }

  .leads-table {
    width: 100%;
    border-collapse: collapse;
    text-align: left;
  }

  .leads-table th {
    background-color: var(--khet-surface-alt);
    padding: 1rem;
    font-size: 0.85rem;
    font-weight: 600;
    color: var(--khet-text-muted);
    border-bottom: 1px solid var(--khet-border);
  }

  .leads-table td {
    padding: 1rem;
    border-bottom: 1px solid var(--khet-border);
    font-size: 0.9rem;
  }

  .lead-cell {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .lead-name {
    font-weight: 600;
    color: var(--khet-text-main);
  }

  .lead-contact {
    font-size: 0.8rem;
    color: var(--khet-text-muted);
  }

  .status-pill {
    font-size: 0.75rem;
    padding: 2px 8px;
    border-radius: 12px;
    font-weight: 600;
  }

  .status-pill.New { background-color: #d1ecf1; color: #0c5460; }
  .status-pill.Contacted { background-color: #fff3cd; color: #856404; }
  .status-pill.Qualified { background-color: #d4edda; color: #155724; }
  .status-pill.Converted { background-color: #e2e3e5; color: #383d41; }

  .priority-pill {
    font-size: 0.75rem;
    font-weight: 600;
  }

  .priority-pill.High { color: #c0392b; }
  .priority-pill.Medium { color: #e67e22; }
  .priority-pill.Low { color: #2ecc71; }

  .action-group {
    display: flex;
    gap: 0.5rem;
  }
  </style>
</template>
