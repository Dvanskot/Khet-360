<template>
  <div class="crm-dashboard">
    <div class="view-header">
      <div class="header-left">
        <h1>CRM & Customer Hub</h1>
        <p>Manage your customer relationships, family graphs and sales pipeline.</p>
      </div>
      <div class="header-actions">
        <KButton variant="primary" @click="$router.push('/crm/leads')">View Sales Pipeline</KButton>
      </div>
    </div>

    <div class="dashboard-grid">
      <!-- Quick Stats -->
      <div class="stats-grid">
        <KCard elevation="sm" class="stat-card">
          <div class="stat-icon">👥</div>
          <div class="stat-details">
            <span class="stat-label">Total Customers</span>
            <span class="stat-value">1,248</span>
          </div>
        </KCard>
        <KCard elevation="sm" class="stat-card">
          <div class="stat-icon">⚡</div>
          <div class="stat-details">
            <span class="stat-label">Active Leads</span>
            <span class="stat-value">42</span>
          </div>
        </KCard>
        <KCard elevation="sm" class="stat-card">
          <div class="stat-icon">📈</div>
          <div class="stat-details">
            <span class="stat-label">Conversion Rate</span>
            <span class="stat-value">24%</span>
          </div>
        </KCard>
      </div>

      <div class="main-content">
        <div class="section">
          <div class="section-header">
            <h3>Recent Customers</h3>
            <KButton variant="secondary" size="sm" @click="$router.push('/crm/leads')">All Customers</KButton>
          </div>
          <div class="customer-list">
            <div v-for="customer in recentCustomers" :key="customer.id" class="customer-row" @click="$router.push(`/crm/customer/${customer.id}`)">
              <div class="customer-info">
                <span class="name">{{ customer.name }}</span>
                <span class="email">{{ customer.email }}</span>
              </div>
              <div class="customer-status">
                <span class="pill">{{ customer.status }}</span>
              </div>
              <div class="customer-action">
                <span class="chevron">→</span>
              </div>
            </div>
          </div>
        </div>

        <div class="section">
          <div class="section-header">
            <h3>Hot Leads</h3>
            <KButton variant="secondary" size="sm" @click="$router.push('/crm/leads')">Manage Pipeline</KButton>
          </div>
          <div class="leads-list">
            <div v-for="lead in hotLeads" :key="lead.id" class="lead-row" @click="$router.push('/crm/leads')">
              <div class="lead-info">
                <span class="name">{{ lead.name }}</span>
                <span class="interest">{{ lead.interest }}</span>
              </div>
              <div class="lead-meta">
                <span class="priority" :class="lead.priority">{{ lead.priority }}</span>
                <span class="date">{{ lead.date }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </template>

  <script setup lang="ts">
  import { KButton, KCard } from '@khet360/ui-shared';

  const recentCustomers = [
    { id: 'C-001', name: 'Samuel Tshikota', email: 'samuel@example.co.za', status: 'Active' },
    { id: 'C-002', name: 'Nomvula Zulu', email: 'nomvula@example.co.za', status: 'Active' },
    { id: 'C-003', name: 'Pieter Botha', email: 'pieter@example.co.za', status: 'Prospect' },
  ];

  const hotLeads = [
    { id: 'L1', name: 'Thabo Mbeki', interest: 'Burial Plan', priority: 'High', date: '2 hours ago' },
    { id: 'L2', name: 'Grace Khumalo', interest: 'Premium Package', priority: 'High', date: '5 hours ago' },
    { id: 'L3', name: 'Alice Sibiya', interest: 'Cash Payout', priority: 'Medium', date: '1 day ago' },
  ];
  </script>

  <style scoped>
  .crm-dashboard {
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

  .dashboard-grid {
    display: flex;
    flex-direction: column;
    gap: 2rem;
  }

  .stats-grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 1.5rem;
  }

  .stat-card {
    display: flex;
    align-items: center;
    gap: 1.5rem;
    padding: 1.5rem;
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

  .main-content {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 2rem;
  }

  .section {
    background-color: white;
    border: 1px solid var(--khet-border);
    border-radius: var(--khet-radius-md);
    padding: 1.5rem;
  }

  .section-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1.5rem;
  }

  .section-header h3 {
    font-size: 1.1rem;
    margin: 0;
    font-weight: 700;
  }

  .customer-list, .leads-list {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
  }

  .customer-row, .lead-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.75rem 1rem;
    border: 1px solid var(--khet-border);
    border-radius: 8px;
    cursor: pointer;
    transition: all 0.2s;
  }

  .customer-row:hover, .lead-row:hover {
    background-color: var(--khet-primary-light);
    border-color: var(--khet-primary);
  }

  .customer-info, .lead-info {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .name {
    font-weight: 600;
    font-size: 0.9rem;
  }

  .email, .interest {
    font-size: 0.8rem;
    color: var(--khet-text-muted);
  }

  .pill {
    font-size: 0.7rem;
    padding: 2px 8px;
    background-color: var(--khet-surface-alt);
    border-radius: 12px;
    font-weight: 600;
  }

  .customer-action {
    color: var(--khet-text-muted);
  }

  .lead-meta {
    display: flex;
    align-items: center;
    gap: 1rem;
  }

  .priority {
    font-size: 0.75rem;
    font-weight: 700;
    padding: 2px 6px;
    border-radius: 4px;
  }

  .priority.High { color: #c0392b; background-color: #f8d7da; }
  .priority.Medium { color: #e67e22; background-color: #fff3cd; }
  .priority.Low { color: #2ecc71; background-color: #d1ecf1; }

  .date {
    font-size: 0.75rem;
    color: var(--khet-text-muted);
  }
  </style>
</template>
