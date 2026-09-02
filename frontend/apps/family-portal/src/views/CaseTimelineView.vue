<template>
  <div class="portal-container">
    <header class="portal-header">
      <div class="header-content">
        <div class="welcome">
          <h1>In Loving Memory of {{ deceasedName }}</h1>
          <p>We are here to support you. Below is the current progress of the arrangements.</p>
        </div>
        <div class="family-badge">
          <span>Family Access Portal</span>
        </div>
      </div>
    </header>

    <main class="portal-main">
      <div class="main-grid">
        <!-- Timeline Section -->
        <div class="timeline-section">
          <div class="section-card">
            <div class="section-title">
              <h3>Service Progress</h3>
              <span class="status-pill">Current Stage: {{ currentStage }}</span>
            </div>
            <div class="timeline">
              <div v-for="(milestone, index) in milestones" :key="index" class="milestone-item" :class="{ completed: index < currentStageIndex, active: index === currentStageIndex }">
                <div class="milestone-marker">
                  <span v-if="index < currentStageIndex">✓</span>
                  <span v-else-if="index === currentStageIndex">...</span>
                  <span v-else>{{ index + 1 }}</span>
                </div>
                <div class="milestone-content">
                  <div class="milestone-title">{{ milestone.title }}</div>
                  <div class="milestone-desc">{{ milestone.description }}</div>
                  <div v-if="milestone.date" class="milestone-date">{{ milestone.date }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Actions Section -->
        <div class="actions-section">
          <div class="action-card upload-card">
            <div class="action-header">
              <span class="action-icon">📄</span>
              <h3>Upload Documents</h3>
            </div>
            <p>Please provide the required documents to avoid delays in the process.</p>
            <div class="doc-list">
              <div v-for="doc in requiredDocs" :key="doc.name" class="doc-item" :class="{ uploaded: doc.uploaded }">
                <div class="doc-info">
                  <span class="doc-name">{{ doc.name }}</span>
                  <span class="doc-status">{{ doc.uploaded ? 'Uploaded' : 'Required' }}</span>
                </div>
                <KButton v-if="!doc.uploaded" variant="secondary" size="sm" @click="uploadDoc(doc.name)">Upload</KButton>
                <span v-else class="check-icon">✓</span>
              </div>
            </div>
          </div>

          <div class="action-card payment-card">
            <div class="action-header">
              <span class="action-icon">💳</span>
              <h3>Balance Settlement</h3>
            </div>
            <div class="payment-info">
              <div class="payment-row">
                <span>Total Package Cost:</span>
                <span>R 12,000.00</span>
              </div>
              <div class="payment-row">
                <span>Insurance Cover:</span>
                <span>- R 8,000.00</span>
              </div>
              <div class="payment-divider"></div>
              <div class="payment-total">
                <span>Outstanding Balance:</span>
                <span class="amount">R 4,000.00</span>
              </div>
            </div>
            <KButton variant="primary" class="pay-btn" @click="makePayment">Pay Outstanding Balance</KButton>
          </div>
        </div>
      </div>
    </main>

    <footer class="portal-footer">
      <p>Need help? Contact your Funeral Director, Sarah Jenkins, at +27 82 123 4567</p>
    </footer>
  </template>

  <script setup lang="ts">
  import { ref } from 'vue';
  import { KButton } from '@khet360/ui-shared';

  const deceasedName = ref('Samuel Tshikota');
  const currentStage = ref('Service Planning');
  const currentStageIndex = ref(2);

  const milestones = [
    { title: 'Death Notification', description: 'Case opened and initial notification received.', date: 'Aug 25' },
    { title: 'Verification', description: 'Identity and policy verification completed.', date: 'Aug 26' },
    { title: 'Service Planning', description: 'Arranging venue, transport and casket selection.', date: 'Current' },
    { title: 'Service Delivery', description: 'The funeral service and burial/cremation.', date: 'Pending' },
    { title: 'Case Closure', description: 'Final administration and document archiving.', date: 'Pending' },
  ];

  const requiredDocs = ref([
    { name: 'Death Certificate', uploaded: true },
    { name: 'ID Copy (Deceased)', uploaded: true },
    { name: 'ID Copy (Next of Kin)', uploaded: false },
    { name: 'Marriage Certificate', uploaded: false },
  ]);

  const uploadDoc = (name: string) => {
    alert(`Opening upload dialog for ${name}...`);
  };

  const makePayment = () => {
    alert('Redirecting to secure payment gateway...');
  };
  </script>

  <style scoped>
  .portal-container {
    display: flex;
    flex-direction: column;
    min-height: 100vh;
  }

  .portal-header {
    background-color: var(--family-surface);
    padding: 3rem 2rem;
    text-align: center;
    border-bottom: 1px solid var(--family-border);
  }

  .header-content {
    max-width: 800px;
    margin: 0 auto;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 1.5rem;
  }

  .welcome h1 {
    font-size: 2.5rem;
    margin: 0 0 1rem 0;
    color: var(--family-text-main);
    font-weight: 300;
  }

  .welcome p {
    font-size: 1.1rem;
    color: var(--family-text-muted);
    max-width: 600px;
    margin: 0 auto;
  }

  .family-badge {
    background-color: var(--family-primary-light, #f3f4f6);
    color: var(--family-text-muted);
    padding: 6px 16px;
    border-radius: 20px;
    font-size: 0.8rem;
    font-weight: 600;
    border: 1px solid var(--family-border);
  }

  .portal-main {
    flex: 1;
    padding: 3rem 2rem;
    max-width: 1100px;
    margin: 0 auto;
    width: 100%;
  }

  .main-grid {
    display: grid;
    grid-template-columns: 1fr 400px;
    gap: 3rem;
  }

  .section-card {
    background-color: white;
    border-radius: var(--family-radius);
    padding: 2rem;
    box-shadow: 0 10px 30px rgba(0,0,0,0.03);
    border: 1px solid var(--family-border);
  }

  .section-title {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 2rem;
  }

  .section-title h3 {
    font-size: 1.3rem;
    margin: 0;
    color: var(--family-text-main);
  }

  .status-pill {
    font-size: 0.8rem;
    padding: 4px 12px;
    background-color: var(--family-accent);
    color: white;
    border-radius: 12px;
    font-weight: 600;
  }

  .timeline {
    display: flex;
    flex-direction: column;
    gap: 2rem;
    position: relative;
  }

  .milestone-item {
    display: flex;
    gap: 1.5rem;
    position: relative;
  }

  .milestone-marker {
    width: 32px;
    height: 32px;
    border-radius: 50%;
    background-color: white;
    border: 2px solid var(--family-border);
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 0.8rem;
    font-weight: 700;
    z-index: 2;
    transition: all 0.3s;
  }

  .milestone-item.active .milestone-marker {
    border-color: var(--family-accent);
    background-color: var(--family-accent);
    color: white;
    box-shadow: 0 0 0 4px rgba(8, 175, 175, 0.2);
  }

  .milestone-item.completed .milestone-marker {
    background-color: #2ecc71;
    border-color: #2ecc71;
    color: white;
  }

  .milestone-content {
    flex: 1;
    padding-bottom: 1rem;
  }

  .milestone-title {
    font-weight: 600;
    font-size: 1.1rem;
    margin-bottom: 0.25rem;
  }

  .milestone-desc {
    font-size: 0.9rem;
    color: var(--family-text-muted);
    margin-bottom: 0.5rem;
  }

  .milestone-date {
    font-size: 0.75rem;
    font-weight: 700;
    color: var(--family-text-muted);
    text-transform: uppercase;
  }

  .actions-section {
    display: flex;
    flex-direction: column;
    gap: 2rem;
  }

  .action-card {
    background-color: white;
    border-radius: var(--family-radius);
    padding: 2rem;
    box-shadow: 0 10px 30px rgba(0,0,0,0.03);
    border: 1px solid var(--family-border);
  }

  .action-header {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    margin-bottom: 1rem;
  }

  .action-icon {
    font-size: 1.5rem;
  }

  .action-header h3 {
    font-size: 1.2rem;
    margin: 0;
  }

  .doc-list {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    margin-top: 1.5rem;
  }

  .doc-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.75rem 1rem;
    background-color: var(--family-bg);
    border-radius: 8px;
    border: 1px solid var(--family-border);
  }

  .doc-info {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
  }

  .doc-name {
    font-size: 0.9rem;
    font-weight: 500;
  }

  .doc-status {
    font-size: 0.75rem;
    color: var(--family-text-muted);
  }

  .doc-item.uploaded {
    background-color: #f0fff4;
    border-color: #c6f6d5;
  }

  .check-icon {
    color: #2ecc71;
    font-weight: 800;
  }

  .payment-info {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
    margin-bottom: 1.5rem;
  }

  .payment-row {
    display: flex;
    justify-content: space-between;
    font-size: 0.9rem;
  }

  .payment-divider {
    height: 1px;
    background-color: var(--family-border);
    margin: 0.5rem 0;
  }

  .payment-total {
    display: flex;
    justify-content: space-between;
    font-weight: 700;
    font-size: 1.1rem;
  }

  .amount {
    color: #c0392b;
    font-weight: 800;
  }

  .pay-btn {
    width: 100%;
    padding: 1rem;
    font-weight: 700;
    font-size: 1rem;
    background-color: var(--family-accent);
  }

  .portal-footer {
    text-align: center;
    padding: 3rem 2rem;
    color: var(--family-text-muted);
    font-size: 0.9rem;
    border-top: 1px solid var(--family-border);
  }
  </style>
</template>
