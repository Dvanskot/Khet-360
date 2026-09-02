<template>
  <div class="tax-center">
    <div class="view-header">
      <div class="header-left">
        <h1>SARS Tax Center</h1>
        <p>South African statutory tax compliance and reporting.</p>
      </div>
      <div class="header-actions">
        <KButton variant="secondary" @click="generateIrp5">Generate IRP5 Certificates</KButton>
        <KButton variant="primary" @click="submitEmp201">Submit EMP201 to SARS</KButton>
      </div>
    </div>

    <div class="tax-grid">
      <!-- EMP201 Summary -->
      <div class="emp201-section">
        <KCard elevation="sm">
          <template #header>
            <div class="section-header">
              <strong>EMP201 Monthly Summary (Sept 2026)</strong>
              <span class="status-pill">Draft</span>
            </div>
          </template>
          <div class="tax-table-container">
            <table class="tax-table">
              <thead>
                <tr>
                  <th>Tax Component</th>
                  <th>Calculation</th>
                  <th>Employee Share</th>
                  <th>Employer Share</th>
                  <th>Total Due</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="row in taxRows" :key="row.component">
                  <td class="component-name">{{ row.component }}</td>
                  <td class="calc-detail">{{ row.calculation }}</td>
                  <td>{{ row.empAmount }} ZAR</td>
                  <td>{{ row.employerAmount }} ZAR</td>
                  <td class="total">{{ row.total }} ZAR</td>
                </tr>
              </tbody>
              <tfoot>
                <tr class="total-row">
                  <td colspan="4" class="total-label">Grand Total Monthly Liability</td>
                  <td class="total-value">{{ grandTotal }} ZAR</td>
                </tr>
              </tfoot>
            </table>
          </div>
        </KCard>
      </div>

      <!-- Tax Year Progress -->
      <div class="tax-meta-section">
        <KCard elevation="sm" class="year-card">
          <template #header><strong>Tax Year: 2026/2027</strong></template>
          <div class="year-stats">
            <div class="year-stat">
              <span class="stat-label">PAYE Filed</span>
              <span class="stat-value">8 / 12 Months</span>
            </div>
            <div class="year-stat">
              <span class="stat-label">UIF Compliance</span>
              <span class="stat-value success">Compliant</span>
            </div>
            <div class="year-stat">
              <span class="stat-label">SDL Status</span>
              <span class="stat-value">Pending Review</span>
            </div>
          </div>
        </KCard>

        <KCard elevation="sm" class="reporting-card">
          <template #header><strong>Reporting Checklist</strong></template>
          <div class="checklist">
            <div v-for="item in checklist" :key="item.id" class="check-item">
              <input type="checkbox" :checked="item.done" disabled />
              <span>{{ item.label }}</span>
            </div>
          </div>
        </KCard>
      </div>
    </div>
  </template>

  <script setup lang="ts">
  import { ref, computed } from 'vue';
  import { KButton, KCard } from '@khet360/ui-shared';

  const taxRows = ref([
    { component: 'PAYE', calculation: 'Progressive Brackets', empAmount: 12400, employerAmount: 0, total: 12400 },
    { component: 'UIF', calculation: '1% Cap Limit', empAmount: 1200, employerAmount: 1200, total: 2400 },
    { component: 'SDL', calculation: '1% Total Payroll', empAmount: 0, employerAmount: 850, total: 850 },
  ]);

  const checklist = ref([
    { id: 1, label: 'Monthly EMP201 Calculation', done: true },
    { id: 2, label: 'Employer UIF Contributions Verified', done: true },
    { id: 3, label: 'Employee Tax Rebates Applied', done: true },
    { id: 4, label: 'Annual EMP501 Reconciliation', done: false },
    { id: 5, label: 'IRP5 Certificates Generated', done: false },
  ]);

  const grandTotal = computed(() => {
    return taxRows.value.reduce((sum, row) => sum + row.total, 0);
  });

  const submitEmp201 = () => alert('Submitting EMP201 to SARS eFiling...');
  const generateIrp5 = () => alert('Generating IRP5 tax certificates for all employees...');
  </script>

  <style scoped>
  .tax-center {
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

  .tax-grid {
    display: grid;
    grid-template-columns: 1fr 350px;
    gap: 2rem;
  }

  .section-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .status-pill {
    font-size: 0.75rem;
    padding: 2px 10px;
    background-color: #fff3cd;
    color: #856404;
    border-radius: 12px;
    font-weight: 600;
  }

  .tax-table-container {
    overflow-x: auto;
  }

  .tax-table {
    width: 100%;
    border-collapse: collapse;
    text-align: left;
  }

  .tax-table th {
    background-color: var(--khet-surface-alt);
    padding: 1rem;
    font-size: 0.85rem;
    font-weight: 600;
    color: var(--khet-text-muted);
    border-bottom: 1px solid var(--khet-border);
  }

  .tax-table td {
    padding: 1rem;
    border-bottom: 1px solid var(--khet-border);
    font-size: 0.9rem;
  }

  .component-name {
    font-weight: 700;
    color: var(--khet-text-main);
  }

  .calc-detail {
    font-size: 0.8rem;
    color: var(--khet-text-muted);
    font-style: italic;
  }

  .total {
    font-weight: 700;
    color: var(--khet-text-main);
  }

  .total-row {
    background-color: var(--khet-surface-alt);
    font-weight: 800;
  }

  .total-label {
    text-align: right;
    padding-right: 1rem;
  }

  .total-value {
    font-size: 1.1rem;
    color: #c0392b;
  }

  .year-stats {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
    padding: 1rem 0;
  }

  .year-stat {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .stat-label {
    font-size: 0.85rem;
    color: var(--khet-text-muted);
  }

  .stat-value {
    font-weight: 600;
    font-size: 0.9rem;
  }

  .stat-value.success {
    color: #2ecc71;
  }

  .checklist {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    padding: 1rem 0;
  }

  .check-item {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    font-size: 0.85rem;
    color: var(--khet-text-main);
  }

  .check-item input {
    width: 16px;
    height: 16px;
  }
  </style>
</template>
