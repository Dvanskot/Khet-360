<template>
  <div class="finance-dashboard">
    <div class="view-header">
      <div class="header-left">
        <h1>Financial Management</h1>
        <p>Core accounting, SARS compliance and financial reporting.</p>
      </div>
      <div class="header-actions">
        <KButton variant="secondary" @click="$router.push('/finance/ledger')">View General Ledger</KButton>
        <KButton variant="primary" @click="$router.push('/finance/tax')">SARS Tax Center</KButton>
      </div>
    </div>

    <div class="kpi-grid">
      <KCard elevation="sm" class="kpi-card">
        <div class="kpi-label">Total Receivables</div>
        <div class="kpi-value positive">R 452,000.00</div>
        <div class="kpi-trend">↑ 12% from last month</div>
      </KCard>
      <KCard elevation="sm" class="kpi-card">
        <div class="kpi-label">Total Payables</div>
        <div class="kpi-value negative">R 128,400.00</div>
        <div class="kpi-trend">↓ 4% from last month</div>
      </KCard>
      <KCard elevation="sm" class="kpi-card">
        <div class="kpi-label">Net Cash Position</div>
        <div class="kpi-value">R 323,600.00</div>
        <div class="kpi-trend">Stable</div>
      </KCard>
      <KCard elevation="sm" class="kpi-card tax-card">
        <div class="kpi-label">Pending SARS Liability</div>
        <div class="kpi-value tax-value">R 12,450.00</div>
        <div class="kpi-trend warning">Due in 5 days</div>
      </KCard>
    </div>

    <div class="main-grid">
      <!-- Account Summary -->
      <div class="summary-section">
        <KCard elevation="sm">
          <template #header>
            <div class="section-header">
              <strong>Chart of Accounts Summary</strong>
              <KButton variant="secondary" size="sm" @click="$router.push('/finance/ledger')">Full Ledger</KButton>
            </div>
          </template>
          <div class="account-list">
            <div v-for="acc in accounts" :key="acc.id" class="account-row">
              <div class="acc-info">
                <span class="code">{{ acc.code }}</span>
                <span class="name">{{ acc.name }}</span>
              </div>
              <div class="acc-balance" :class="acc.type === 'Expense' ? 'neg' : 'pos'">
                {{ acc.balance }} ZAR
              </div>
            </div>
          </div>
        </KCard>
      </div>

      <!-- Recent Transactions -->
      <div class="transactions-section">
        <KCard elevation="sm">
          <template #header>
            <div class="section-header">
              <strong>Recent Journal Entries</strong>
              <KButton variant="secondary" size="sm">New Entry</KButton>
            </div>
          </template>
          <div class="transaction-list">
            <div v-for="tx in transactions" :key="tx.id" class="tx-row">
              <div class="tx-date">{{ tx.date }}</div>
              <div class="tx-details">
                <div class="tx-desc">{{ tx.description }}</div>
                <div class="tx-ref">{{ tx.reference }}</div>
              </div>
              <div class="tx-amount" :class="tx.totalAmount < 0 ? 'neg' : 'pos'">
                {{ tx.totalAmount }} ZAR
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
  import { Account, JournalEntry } from '@/components/finance/types';

  const accounts = ref<Account[]>([
    { id: 'A1', code: '1000', name: 'Main Bank Account', type: 'Asset', balance: 323600, branchId: 'B1' },
    { id: 'A2', code: '1200', name: 'Accounts Receivable', type: 'Asset', balance: 452000, branchId: 'B1' },
    { id: 'A3', code: '2000', name: 'Accounts Payable', type: 'Liability', balance: -128400, branchId: 'B1' },
    { id: 'A4', code: '5000', name: 'Funeral Service Revenue', type: 'Revenue', balance: 850000, branchId: 'B1' },
    { id: 'A5', code: '6000', name: 'Casket Inventory Cost', type: 'Expense', balance: -210000, branchId: 'B1' },
  ]);

  const transactions = ref<JournalEntry[]>([
    { id: 'T1', date: '2026-09-01', description: 'Payment received: Case #C-1024', reference: 'PAY-9921', totalAmount: 5000, lines: [] },
    { id: 'T2', date: '2026-08-31', description: 'Supplier Payment: Oak Caskets Ltd', reference: 'SUP-441', totalAmount: -12000, lines: [] },
    { id: 'T3', date: '2026-08-30', description: 'Monthly Payroll - Aug 2026', reference: 'PR-AUG-26', totalAmount: -45000, lines: [] },
    { id: 'T4', date: '2026-08-28', description: 'Insurance Claim Payout: Case #C-1018', reference: 'INS-882', totalAmount: 15000, lines: [] },
  ]);
  </script>

  <style scoped>
  .finance-dashboard {
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

  .kpi-grid {
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 1.5rem;
  }

  .kpi-card {
    padding: 1.5rem;
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }

  .kpi-label {
    font-size: 0.85rem;
    color: var(--khet-text-muted);
    font-weight: 500;
  }

  .kpi-value {
    font-size: 1.75rem;
    font-weight: 800;
    color: var(--khet-text-main);
  }

  .kpi-value.positive { color: #2ecc71; }
  .kpi-value.negative { color: #c0392b; }

  .kpi-trend {
    font-size: 0.75rem;
    color: var(--khet-text-muted);
  }

  .kpi-trend.warning { color: #e67e22; font-weight: 700; }

  .tax-card {
    border: 2px solid #f1c40f;
    background-color: #fffdf0;
  }

  .tax-value {
    color: #b8860b;
  }

  .main-grid {
    display: grid;
    grid-template-columns: 1fr 1.5fr;
    gap: 2rem;
  }

  .section-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .account-list, .transaction-list {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }

  .account-row, .tx-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.75rem 1rem;
    border-bottom: 1px solid var(--khet-border);
    font-size: 0.9rem;
  }

  .acc-info {
    display: flex;
    gap: 1rem;
  }

  .code {
    font-family: monospace;
    color: var(--khet-text-muted);
    font-weight: 600;
  }

  .name {
    font-weight: 500;
  }

  .acc-balance {
    font-weight: 700;
  }

  .acc-balance.pos { color: #2ecc71; }
  .acc-balance.neg { color: #c0392b; }

  .tx-row {
    display: grid;
    grid-template-columns: 100px 1fr 120px;
    align-items: center;
  }

  .tx-date {
    font-size: 0.8rem;
    color: var(--khet-text-muted);
  }

  .tx-details {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .tx-desc {
    font-weight: 500;
    font-size: 0.9rem;
  }

  .tx-ref {
    font-size: 0.75rem;
    color: var(--khet-text-muted);
    font-family: monospace;
  }

  .tx-amount {
    text-align: right;
    font-weight: 700;
  }

  .tx-amount.pos { color: #2ecc71; }
  .tx-amount.neg { color: #c0392b; }
  </style>
</template>
