<template>
  <div class="page">
    <header class="page-header">
      <h1 class="title">Submit Claim</h1>
      <router-link to="/crm" class="back-link">← Back</router-link>
    </header>

    <div class="claim-wizard">
      <div class="form-section">
        <h2 class="section-title">Case Details</h2>
        <div class="form-group">
          <label>Select Funeral Case</label>
          <select v-model="claimData.caseId" class="input-field">
            <option value="">-- Select Case --</option>
            <option v-for="caseItem in availableCases" :key="caseItem.id" :value="caseItem.id">
              {{ caseItem.id }} - {{ caseItem.deceasedName }}
            </option>
          </select>
        </div>
        <div class="form-group">
          <label>Claim Type</label>
          <select v-model="claimData.type" class="input-field">
            <option value="Death">Death Benefit</option>
            <option value="Disability">Permanent Disability</option>
            <option value="Other">Other Benefit</option>
          </select>
        </div>
      </div>

      <div class="form-section">
        <h2 class="section-title">Payout Estimation</h2>
        <div class="benefit-card">
          <div class="benefit-row">
            <span>Base Cover Amount:</span>
            <strong class="amount">{{ formattedAmount }}</strong>
          </div>
          <div class="benefit-row">
            <span>Processing Fee:</span>
            <strong class="amount-neg">- {{ formattedFee }}</strong>
          </div>
          <div class="benefit-divider"></div>
          <div class="benefit-row total">
            <span>Net Payout:</span>
            <strong class="total-amount">{{ formattedNet }}</strong>
          </div>
        </div>
      </div>

      <div class="form-section">
        <h2 class="section-title">Required Documentation</h2>
        <div class="upload-grid">
          <div class="upload-item" :class="{ uploaded: docs.deathCertificate }">
            <div class="upload-icon">📄</div>
            <div class="upload-info">
              <span class="label">Death Certificate</span>
              <span class="status">{{ docs.deathCertificate ? 'Uploaded' : 'Required' }}</span>
            </div>
            <input type="file" @change="uploadDoc('deathCertificate')" class="file-input" />
          </div>
          <div class="upload-item" :class="{ uploaded: docs.idCopy }">
            <div class="upload-icon">🆔</div>
            <div class="upload-info">
              <span class="label">ID Copy (Claimant)</span>
              <span class="status">{{ docs.idCopy ? 'Uploaded' : 'Required' }}</span>
            </div>
            <input type="file" @change="uploadDoc('idCopy')" class="file-input" />
          </div>
        </div>
      </div>

      <div class="form-section">
        <h2 class="section-title">Bank Details</h2>
        <div class="form-group">
          <label>Account Holder</label>
          <input v-model="claimData.bankAccountHolder" class="input-field" placeholder="Full Name" />
        </div>
        <div class="form-group">
          <label>Bank Name</label>
          <input v-model="claimData.bankName" class="input-field" placeholder="e.g. Standard Bank" />
        </div>
        <div class="form-group">
          <label>Account Number</label>
          <input v-model="claimData.accountNumber" class="input-field" placeholder="Account number" />
        </div>
        <div class="form-group">
          <label>Branch Code</label>
          <input v-model="claimData.branchCode" class="input-field" placeholder="Branch code" />
        </div>
      </div>

      <div class="submit-area">
        <button @click="submitClaim" :disabled="!isClaimValid" class="btn-submit">
          Submit Claim for Approval
        </button>
        <p class="disclaimer">By submitting, you certify that the information provided is true and correct.</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

const availableCases = [
  { id: 'CASE-1001', deceasedName: 'Samuel Nkosi' },
  { id: 'CASE-1002', deceasedName: 'Maria Molefe' },
  { id: 'CASE-1003', deceasedName: 'Tshepo Zulu' },
];

const claimData = ref({
  caseId: '',
  type: 'Death',
  bankAccountHolder: '',
  bankName: '',
  accountNumber: '',
  branchCode: '',
});

const docs = ref({
  deathCertificate: false,
  idCopy: false,
});

const baseAmount = 25000;
const fee = 250;

const formattedAmount = computed(() => `R${baseAmount.toLocaleString()}`);
const formattedFee = computed(() => `R${fee.toLocaleString()}`);
const formattedNet = computed(() => `R${(baseAmount - fee).toLocaleString()}`);

const isClaimValid = computed(() => {
  return claimData.value.caseId &&
         claimData.value.bankAccountHolder &&
         claimData.value.accountNumber &&
         docs.value.deathCertificate &&
         docs.value.idCopy;
});

function uploadDoc(type: 'deathCertificate' | 'idCopy') {
  docs.value[type] = true;
}

async function submitClaim() {
  try {
    // Simulate API call
    alert('Claim submitted successfully! It is now pending verification by the Claims Department.');
    router.push('/crm');
  } catch (e) {
    alert('Submission failed. Please check your connection.');
  }
}
</script>

<style scoped>
.page {
  padding: 1.5rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.title {
  font-size: 1.6rem;
  font-weight: 700;
  margin: 0;
}

.back-link {
  font-size: 0.85rem;
  color: var(--khet-primary);
  text-decoration: none;
  font-weight: 600;
}

.form-section {
  background: var(--khet-surface);
  border: 1px solid var(--khet-border);
  border-radius: 16px;
  padding: 1.25rem;
  margin-bottom: 1.5rem;
}

.section-title {
  font-size: 1rem;
  font-weight: 700;
  margin: 0 0 1.25rem 0;
  color: var(--khet-text-main);
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.form-group label {
  font-size: 0.85rem;
  font-weight: 600;
  color: var(--khet-text-muted);
}

.input-field {
  padding: 12px;
  border-radius: 8px;
  border: 1px solid var(--khet-border);
  background: var(--khet-bg);
  font-size: 0.95rem;
}

.benefit-card {
  background: rgba(79, 70, 229, 0.05);
  border: 1px solid rgba(79, 70, 229, 0.2);
  border-radius: 12px;
  padding: 1rem;
}

.benefit-row {
  display: flex;
  justify-content: space-between;
  font-size: 0.9rem;
  margin-bottom: 0.5rem;
}

.amount { font-weight: 600; }
.amount-neg { color: #ef4444; font-weight: 600; }

.benefit-divider {
  height: 1px;
  background: var(--khet-border);
  margin: 0.75rem 0;
}

.benefit-row.total {
  font-size: 1.1rem;
  font-weight: 700;
}

.total-amount {
  color: var(--khet-primary);
}

.upload-grid {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.upload-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.75rem;
  border: 1px solid var(--khet-border);
  border-radius: 12px;
  position: relative;
  transition: all 0.2s;
}

.upload-item.uploaded {
  border-color: #10b981;
  background: rgba(16, 185, 129, 0.05);
}

.upload-icon {
  font-size: 1.5rem;
}

.upload-info {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.upload-info .label {
  font-size: 0.85rem;
  font-weight: 600;
}

.upload-info .status {
  font-size: 0.75rem;
  color: var(--khet-text-muted);
}

.upload-item.uploaded .status {
  color: #10b981;
  font-weight: 600;
}

.file-input {
  position: absolute;
  right: 0;
  top: 0;
  bottom: 0;
  width: 60px;
  opacity: 0;
  cursor: pointer;
}

.submit-area {
  text-align: center;
  margin-bottom: 3rem;
}

.btn-submit {
  width: 100%;
  background: var(--khet-primary);
  color: white;
  border: none;
  padding: 14px;
  border-radius: 12px;
  font-weight: 700;
  font-size: 1rem;
  cursor: pointer;
  transition: opacity 0.2s;
}

.btn-submit:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.disclaimer {
  font-size: 0.75rem;
  color: var(--khet-text-muted);
  margin-top: 1rem;
  line-height: 1.4;
}
</style>
