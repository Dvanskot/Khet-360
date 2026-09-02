<template>
  <div class="customer-360">
    <div class="page-header">
      <div class="header-main">
        <div class="customer-badge">Customer</div>
        <h1 class="customer-name">{{ customer.fullName }}</h1>
        <div class="status-pill">{{ customer.status }}</div>
      </div>
      <div class="header-actions">
        <KButton variant="secondary" @click="exportProfile">Export Profile</KButton>
        <KButton variant="primary" @click="createCase">Create Funeral Case</KButton>
      </div>
    </div>

    <div class="dashboard-grid">
      <!-- Left Column: Core Details -->
      <div class="column-left">
        <KCard elevation="sm" class="details-card">
          <template #header><strong>Personal Information</strong></template>
          <div class="info-grid">
            <div class="info-item">
              <span class="label">ID Number</span>
              <span class="value">{{ customer.idNumber }}</span>
            </div>
            <div class="info-item">
              <span class="label">Email</span>
              <span class="value">{{ customer.email }}</span>
            </div>
            <div class="info-item">
              <span class="label">Phone</span>
              <span class="value">{{ customer.phone }}</span>
            </div>
            <div class="info-item">
              <span class="label">Address</span>
              <span class="value">{{ customer.address }}</span>
            </div>
          </div>
        </KCard>

        <KCard elevation="sm" class="balance-card">
          <template #header><strong>Financial Position</strong></template>
          <div class="balance-content">
            <span class="balance-label">Current Account Balance</span>
            <span class="balance-amount" :class="{ 'overdue': customer.totalBalance > 0 }">
              {{ customer.totalBalance }} ZAR
            </span>
            <div class="balance-actions">
              <KButton variant="secondary" size="sm">View Statement</KButton>
              <KButton variant="primary" size="sm">Make Payment</KButton>
            </div>
          </div>
        </KCard>
      </div>

      <!-- Middle Column: Family Graph (Conceptual) -->
      <div class="column-mid">
        <KCard elevation="sm" class="graph-card">
          <template #header>
            <div class="graph-header">
              <strong>Family Relationship Graph</strong>
              <KButton variant="secondary" size="sm" @click="addMember">➕ Add Member</KButton>
            </div>
          </template>
          <div class="graph-container">
            <div class="family-node main-member">
              <div class="node-avatar">C</div>
              <span class="node-name">{{ customer.fullName }}</span>
              <span class="node-role">Main Member</span>
            </div>
            <div class="graph-connections">
              <div v-for="member in family" :key="member.id" class="family-link">
                <div class="link-line"></div>
                <div class="family-node">
                  <div class="node-avatar">{{ member.fullName[0] }}</div>
                  <div class="node-details">
                    <span class="node-name">{{ member.fullName }}</span>
                    <span class="node-role">{{ member.relationship }}</span>
                  </div>
                  <span class="status-dot" :class="member.status"></span>
                </div>
              </div>
            </div>
          </div>
        </KCard>
      </div>

      <!-- Right Column: Associated History -->
      <div class="column-right">
        <KCard elevation="sm" class="history-card">
          <template #header><strong>Related Records</strong></template>
          <div class="history-list">
            <div v-for="record in records" :key="record.id" class="history-item">
              <div class="record-icon">{{ record.type === 'policy' ? '📜' : '⚰️' }}</div>
              <div class="record-info">
                <div class="record-title">{{ record.title }}</div>
                <div class="record-date">{{ record.date }}</div>
              </div>
              <span class="record-status">{{ record.status }}</span>
            </div>
          </div>
        </KCard>
      </div>
    </div>
  </template>

  <script setup lang="ts">
  import { ref } from 'vue';
  import { KButton, KCard } from '@khet360/ui-shared';
  import { Customer, FamilyMember } from '@/components/crm/types';

  const customer = ref<Customer>({
    id: 'C-001',
    fullName: 'Samuel Tshikota',
    type: 'Individual',
    email: 'samuel@example.co.za',
    phone: '+27 82 123 4567',
    address: '123 Main St, Cape Town, 8001',
    idNumber: '8501015000081',
    status: 'Active',
    createdDate: '2020-01-15',
    totalBalance: 1250.00,
  });

  const family = ref<FamilyMember[]>([
    { id: 'F1', customerId: 'C-001', fullName: 'Sarah Tshikota', relationship: 'Spouse', age: 42, status: 'Alive', isPolicyMember: true },
    { id: 'F2', customerId: 'C-001', fullName: 'Junior Tshikota', relationship: 'Child', age: 15, status: 'Alive', isPolicyMember: true },
    { id: 'F3', customerId: 'C-001', fullName: 'Elizabeth Tshikota', relationship: 'Parent', age: 68, status: 'Deceased', isPolicyMember: true },
    { id: 'F4', customerId: 'C-001', fullName: 'Peter Tshikota', relationship: 'Extended Family', age: 55, status: 'Alive', isPolicyMember: false },
  ]);

  const records = ref([
    { id: 'R1', type: 'policy', title: 'Family Burial Plan Gold', date: '2020-01-15', status: 'Active' },
    { id: 'R2', type: 'case', title: 'Case #C-1024 - Funeral Service', date: '2026-08-25', status: 'Closed' },
    { id: 'R3', type: 'policy', title: 'Life Cover Add-on', date: '2022-06-10', status: 'Active' },
  ]);

  const createCase = () => alert('Opening New Case Wizard for this customer...');
  const exportProfile = () => alert('Exporting customer profile to PDF...');
  const addMember = () => alert('Opening Add Family Member form...');
  </script>

  <style scoped>
  .customer-360 {
    display: flex;
    flex-direction: column;
    gap: 2rem;
  }

  .page-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .header-main {
    display: flex;
    align-items: center;
    gap: 1rem;
  }

  .customer-badge {
    background-color: var(--khet-primary-light);
    color: var(--khet-primary);
    padding: 4px 12px;
    border-radius: 12px;
    font-size: 0.75rem;
    font-weight: 700;
    text-transform: uppercase;
    border: 1px solid var(--khet-primary);
  }

  .customer-name {
    font-size: 2rem;
    margin: 0;
    color: var(--khet-text-main);
  }

  .status-pill {
    padding: 4px 12px;
    background-color: #d1ecf1;
    color: #0c5460;
    border-radius: 12px;
    font-size: 0.8rem;
    font-weight: 600;
  }

  .dashboard-grid {
    display: grid;
    grid-template-columns: 300px 1fr 350px;
    gap: 1.5rem;
    align-items: start;
  }

  .info-grid {
    display: grid;
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .info-item {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .label {
    font-size: 0.75rem;
    color: var(--khet-text-muted);
  }

  .value {
    font-size: 0.9rem;
    font-weight: 500;
  }

  .balance-content {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0.5rem;
    padding: 1rem 0;
    text-align: center;
  }

  .balance-label {
    font-size: 0.85rem;
    color: var(--khet-text-muted);
  }

  .balance-amount {
    font-size: 2rem;
    font-weight: 800;
    color: var(--khet-text-main);
  }

  .balance-amount.overdue {
    color: #c0392b;
  }

  .balance-actions {
    display: flex;
    gap: 0.5rem;
    margin-top: 1rem;
  }

  .graph-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .graph-container {
    padding: 2rem 0;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 2rem;
  }

  .family-node {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0.5rem;
    position: relative;
    z-index: 2;
  }

  .node-avatar {
    width: 50px;
    height: 50px;
    background-color: var(--khet-primary);
    color: white;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 700;
    font-size: 1.2rem;
    border: 3px solid white;
    box-shadow: 0 4px 8px rgba(0,0,0,0.1);
  }

  .main-member .node-avatar {
    width: 70px;
    height: 70px;
    font-size: 1.8rem;
    background-color: var(--khet-text-main);
  }

  .node-name {
    font-size: 0.9rem;
    font-weight: 600;
    text-align: center;
  }

  .node-role {
    font-size: 0.75rem;
    color: var(--khet-text-muted);
    text-align: center;
  }

  .graph-connections {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
    gap: 2rem;
    width: 100%;
  }

  .family-link {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 1rem;
  }

  .link-line {
    width: 2px;
    height: 2rem;
    background-color: var(--khet-border);
  }

  .family-node {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0.5rem;
  }

  .family-node .node-avatar {
    width: 40px;
    height: 40px;
    font-size: 1rem;
  }

  .node-details {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0.1rem;
  }

  .status-dot {
    width: 8px;
    height: 8px;
    border-radius: 50%;
    position: absolute;
    top: 0;
    right: 0;
  }

  .status-dot.Alive { background-color: #2ecc71; }
  .status-dot.Deceased { background-color: #95a5a6; }

  .history-list {
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  .history-item {
    display: flex;
    align-items: center;
    gap: 1rem;
    padding: 0.75rem;
    border-radius: var(--khet-radius-md);
    background-color: var(--khet-surface-alt);
    cursor: pointer;
    transition: background 0.2s;
  }

  .history-item:hover {
    background-color: var(--khet-primary-light);
  }

  .record-icon {
    font-size: 1.5rem;
    width: 40px;
    height: 40px;
    background-color: white;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 50%;
    border: 1px solid var(--khet-border);
  }

  .record-info {
    flex: 1;
    display: flex;
    flex-direction: column;
  }

  .record-title {
    font-size: 0.9rem;
    font-weight: 600;
    color: var(--khet-text-main);
  }

  .record-date {
    font-size: 0.75rem;
    color: var(--khet-text-muted);
  }

  .record-status {
    font-size: 0.75rem;
    font-weight: 600;
    color: var(--khet-primary);
  }
  </style>
</template>
