<template>
  <div class="payroll-view">
    <div class="view-header">
      <div class="header-left">
        <h1>Payroll Administration</h1>
        <p>Manage monthly payroll cycles, deductions and payment processing.</p>
      </div>
      <div class="header-actions">
        <KButton variant="secondary" @click="exportPayroll">Export to CSV</KButton>
        <KButton variant="primary" @click="finalizeRun">Finalize & Post Payroll</KButton>
      </div>
    </div>

    <div class="payroll-container">
      <div class="payroll-summary-card">
        <div class="summary-item">
          <span class="label">Pay Period:</span>
          <span class="value">September 2026</span>
        </div>
        <div class="summary-item">
          <span class="label">Run Status:</span>
          <span class="value status-draft">Draft</span>
        </div>
        <div class="summary-item">
          <span class="label">Total Employees:</span>
          <span class="value">84</span>
        </div>
      </div>

      <div class="payroll-table-container">
        <table class="payroll-table">
          <thead>
            <tr class="table-header">
              <th>Employee</th>
              <th>Basic Salary</th>
              <th>Allowances</th>
              <th>PAYE (Tax)</th>
              <th>UIF</th>
              <th>Other Deductions</th>
              <th>Net Pay</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="entry in payrollEntries" :key="entry.id">
              <td>
                <div class="emp-cell">
                  <span class="emp-name">{{ entry.employeeName }}</span>
                  <span class="emp-role">{{ entry.role }}</span>
                </div>
              </td>
              <td>{{ entry.basic }} ZAR</td>
              <td>{{ entry.allowances }} ZAR</td>
              <td class="tax-cell">{{ entry.paye }} ZAR</td>
              <td class="tax-cell">{{ entry.uif }} ZAR</td>
              <td>{{ entry.otherDeductions }} ZAR</td>
              <td class="net-cell">{{ entry.netPay }} ZAR</td>
              <td>
                <KButton variant="secondary" size="sm" @click="editEntry(entry.id)">Edit</KButton>
              </div>
            </tr>
          </tbody>
          <tfoot>
            <tr class="footer-row">
              <td colspan="2">Totals:</td>
              <td>{{ totals.allowances }} ZAR</td>
              <td>{{ totals.paye }} ZAR</td>
              <td>{{ totals.uif }} ZAR</td>
              <td>{{ totals.others }} ZAR</td>
              <td class="total-net">{{ totals.net }} ZAR</td>
              <td></td>
            </tr>
          </tfoot>
        </table>
      </div>
    </div>
  </template>

  <script setup lang="ts">
  import { ref, computed } from 'vue';
  import { KButton } from '@khet360/ui-shared';

  const payrollEntries = ref([
    { id: 1, employeeName: 'Sarah Jenkins', role: 'Branch Manager', basic: 45000, allowances: 2000, paye: 11000, uif: 177, otherDeductions: 500, netPay: 34323 },
    { id: 2, employeeName: 'Mike Ross', role: 'Accountant', basic: 32000, allowances: 1000, UIFont: 0, paye: 6500, uif: 177, otherDeductions: 200, netPay: 25323 },
    { id: 3, employeeName: 'Lindiwe Khoza', role: 'Fleet Supervisor', basic: 28000, allowances: 1500, paye: 4200, uif: 177, otherDeductions: 0, netPay: 25123 },
    { id: 4, employeeName: 'Peter Pan', role: 'Driver', basic: 15000, allowances: 500, paye: 1200, uif: 177, otherDeductions: 100, netPay: 14023 },
  ]);

  const totals = computed(() => {
    return payrollEntries.value.reduce((acc, curr) => ({
      allowances: acc.allowances + curr.allowances,
      paye: acc.paye + curr.paye,
      uif: acc.uif + curr.uif,
      others: acc.others + curr.otherDeductions,
      net: acc.net + curr.netPay,
    }), { allowances: 0, paye: 0, uif: 0, others: 0, net: 0 });
  });

  const finalizeRun = () => alert('Finalizing payroll and generating payslips...');
  const exportPayroll = () => alert('Exporting payroll report to CSV...');
  const editEntry = (id: number) => alert(`Editing payroll entry for ID ${id}...`);
  </script>

  <style scoped>
  .payroll-view {
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

  .payroll-container {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
  }

  .payroll-summary-card {
    background-color: white;
    border: 1px solid var(--khet-border);
    border-radius: var(--khet-radius-md);
    padding: 1.5rem;
    display: flex;
    justify-content: space-around;
    align-items: center;
  }

  .summary-item {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .summary-item .label {
    font-size: 0.85rem;
    color: var(--khet-text-muted);
  }

  .summary-item .value {
    font-size: 1.1rem;
    font-weight: 700;
    color: var(--khet-text-main);
  }

  .status-draft {
    color: #e67e22;
  }

  .payroll-table-container {
    background-color: white;
    border: 1px solid var(--khet-border);
    border-radius: var(--khet-radius-md);
    overflow: hidden;
  }

  .payroll-table {
    width: 100%;
    border-collapse: collapse;
    text-align: left;
  }

  .table-header {
    background-color: var(--khet-surface-alt);
  }

  .payroll-table th {
    padding: 1rem;
    font-size: 0.85rem;
    font-weight: 600;
    color: var(--khet-text-muted);
    border-bottom: 1px solid var(--khet-border);
  }

  .payroll-table td {
    padding: 1rem;
    border-bottom: 1px solid var(--khet-border);
    font-size: 0.9rem;
  }

  .emp-cell {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .emp-name {
    font-weight: 600;
  }

  .emp-role {
    font-size: 0.75rem;
    color: var(--khet-text-muted);
  }

  .tax-cell {
    color: #c0392b;
    font-weight: 500;
  }

  .net-cell {
    font-weight: 800;
    color: var(--khet-primary);
  }

  .footer-row {
    background-color: var(--khet-surface-alt);
    font-weight: 700;
  }

  .total-net {
    font-size: 1.1rem;
    color: var(--khet-primary);
  }
  </style>
</template>
