<template>
  <div class="wizard-container">
    <KCard elevation="lg">
      <template #header>
        <div class="wizard-header">
          <div class="header-top">
            <h2>Funeral Arrangement Wizard</h2>
            <span class="case-badge">Case #C-1021</span>
          </div>
          <div class="stepper">
            <div
              v-for="(step, index) in steps"
              :key="step.id"
              class="step-item"
              :class="{ active: currentStepIndex === index, completed: currentStepIndex > index }"
            >
              <div class="step-number">{{ index + 1 }}</div>
              <div class="step-label">{{ step.label }}</div>
              <div v-if="index < steps.length - 1" class="step-line"></div>
            </div>
          </div>
        </div>
      </template>

      <div class="wizard-body">
        <div class="step-content">
          <!-- Dynamic Step Rendering -->
          <div v-if="currentStepIndex === 0" class="step-pane">
            <h3>Deceased & Family Information</h3>
            <div class="form-grid">
              <KInput label="Full Name" v-model="formData.deceasedName" placeholder="Enter full name" />
              <KInput label="Date of Death" type="date" v-model="formData.deathDate" />
              <KInput label="Place of Death" v-model="formData.placeOfDeath" placeholder="Hospital / Home Address" />
              <KInput label="Next of Kin Name" v-model="formData.nokName" />
              <KInput label="Next of Kin Contact" v-model="formData.nokContact" />
            </div>
          </div>

          <div v-if="currentStepIndex === 1" class="step-pane">
            <h3>Service Package & Cover</h3>
            <div class="package-selector">
              <div
                v-for="pkg in packages"
                :key="pkg.id"
                class="package-card"
                :class="{ active: formData.packageId === pkg.id }"
                @click="formData.packageId = pkg.id"
              >
                <div class="pkg-info">
                  <strong>{{ pkg.name }}</strong>
                  <span>{{ pkg.description }}</span>
                </div>
                <div class="pkg-price">{{ pkg.price }} ZAR</div>
              </div>
            </div>
            <div class="benefit-details">
              <h4>Included Benefits</h4>
              <ul>
                <li v-for="benefit in selectedPackageBenefits" :key="benefit">{{ benefit }}</li>
              </ul>
            </div>
          </div>

          <div v-if="currentStepIndex === 2" class="step-pane">
            <h3>Resource Allocation</h3>
            <div class="resource-grid">
              <div class="resource-item">
                <label>Casket Selection</label>
                <select v-model="formData.casketId">
                  <option v-for="item in inventory.caskets" :key="item.id" :value="item.id">{{ item.name }}</option>
                </select>
              </div>
              <div class="resource-item">
                <label>Hearse / Vehicle</label>
                <select v-model="formData.vehicleId">
                  <option v-for="item in inventory.vehicles" :key="item.id" :value="item.id">{{ item.name }}</option>
                </select>
              </div>
              <div class="resource-item">
                <label>Catering Package</label>
                <select v-model="formData.cateringId">
                  <option v-for="item in inventory.catering" :key="item.id" :value="item.id">{{ item.name }}</option>
                </select>
              </div>
            </div>
          </div>

          <div v-if="currentStepIndex === 3" class="step-pane">
            <h3>Review & Confirmation</h3>
            <div class="review-summary">
              <div class="summary-row">
                <span>Deceased:</span> <strong>{{ formData.deceasedName }}</strong>
              </div>
              <div class="summary-row">
                <span>Package:</span> <strong>{{ selectedPackageName }}</strong>
              </div>
              <div class="summary-row">
                <span>Total Cost:</span> <strong>{{ selectedPackagePrice }} ZAR</strong>
              </div>
            </div>
            <div class="confirmation-box">
              <label class="checkbox-label">
                <input type="checkbox" v-model="confirmed" />
                <span>I confirm that all details are correct and authorize the arrangement.</span>
              </label>
            </div>
          </div>
        </div>
      </div>

      <div class="wizard-footer">
        <KButton
          v-if="currentStepIndex > 0"
          variant="secondary"
          @click="currentStepIndex--"
        >
          Back
        </KButton>
        <div class="spacer"></div>
        <KButton
          v-if="currentStepIndex < steps.length - 1"
          variant="primary"
          @click="currentStepIndex++"
        >
          Continue
        </KButton>
        <KButton
          v-else
          variant="primary"
          :disabled="!confirmed"
          @click="submitArrangement"
        >
          Confirm & Post
        </KButton>
      </div>
    </KCard>
  </template>

  <script setup lang="ts">
  import { ref, computed } from 'vue';
  import { KButton, KInput, KCard } from '@khet360/ui-shared';

  const steps = [
    { id: 'info', label: 'Details' },
    { id: 'package', label: 'Package' },
    { id: 'resources', label: 'Resources' },
    { id: 'review', label: 'Review' },
  ];

  const currentStepIndex = ref(0);
  const confirmed = ref(false);

  const formData = ref({
    deceasedName: '',
    deathDate: '',
    placeOfDeath: '',
    nokName: '',
    nokContact: '',
    packageId: null as string | null,
    casketId: '',
    vehicleId: '',
    cateringId: '',
  });

  const packages = [
    { id: 'p1', name: 'Basic Burial', description: 'Essential service with standard casket', price: 5000, benefits: ['Standard Casket', 'Basic Transport', 'Graveyard Fee'] },
    { id: 'p2', name: 'Professional', description: 'Full service with premium options', price: 12000, benefits: ['Premium Casket', 'Luxury Hearse', 'Catering for 50', 'Graveyard Fee'] },
    { id: 'p3', name: 'Elite', description: 'All-inclusive luxury arrangement', price: 25000, benefits: ['Elite Casket', 'Luxury Hearse', 'Full Catering', 'Graveyard Fee', 'Memorial Stone'] },
  ];

  const inventory = {
    caskets: [{ id: 'c1', name: 'Oak Standard' }, { id: 'c2', name: 'Mahogany Premium' }, { id: 'c3', name: 'Ivory Elite' }],
    vehicles: [{ id: 'v1', name: 'Hearse A' }, { id: 'v2', name: 'Hearse B' }, { id: 'v3', name: 'Limousine' }],
    catering: [{ id: 'cat1', name: 'Basic Platter' }, { id: 'cat2', name: 'Full Buffet' }],
  };

  const selectedPackage = computed(() => packages.find(p => p.id === formData.value.packageId));
  const selectedPackageName = computed(() => selectedPackage.value?.name || 'None');
  const selectedPackagePrice = computed(() => selectedPackage.value?.price || 0);
  const selectedPackageBenefits = computed(() => selectedPackage.value?.benefits || []);

  const submitArrangement = () => {
    alert('Arrangement confirmed and posted to financial ledger!');
  };
  </script>

  <style scoped>
  .wizard-container {
    max-width: 800px;
    margin: 0 auto;
  }

  .wizard-header {
    display: flex;
    flex-direction: column;
    gap: 2rem;
  }

  .header-top {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .header-top h2 {
    margin: 0;
    font-size: 1.5rem;
  }

  .case-badge {
    background-color: var(--khet-primary-light);
    color: var(--khet-primary);
    padding: 4px 12px;
    border-radius: 12px;
    font-weight: 700;
    font-size: 0.85rem;
    border: 1px solid var(--khet-primary);
  }

  .stepper {
    display: flex;
    justify-content: space-between;
    align-items: center;
    position: relative;
  }

  .step-item {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0.5rem;
    position: relative;
    z-index: 1;
    flex: 1;
  }

  .step-number {
    width: 32px;
    height: 32px;
    border-radius: 50%;
    background-color: var(--khet-border);
    color: var(--khet-text-muted);
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 700;
    transition: all 0.3s;
  }

  .step-label {
    font-size: 0.75rem;
    font-weight: 500;
    color: var(--khet-text-muted);
  }

  .step-item.active .step-number {
    background-color: var(--khet-primary);
    color: white;
    box-shadow: 0 0 0 4px var(--khet-primary-light);
  }

  .step-item.active .step-label {
    color: var(--khet-primary);
    font-weight: 700;
  }

  .step-item.completed .step-number {
    background-color: var(--khet-success);
    color: white;
  }

  .step-line {
    position: absolute;
    top: 16px;
    left: 50%;
    width: 100%;
    height: 2px;
    background-color: var(--khet-border);
    z-index: -1;
  }

  .wizard-body {
    padding: 2rem 0;
  }

  .step-pane {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
    animation: fadeIn 0.3s ease;
  }

  @keyframes fadeIn {
    from { opacity: 0; transform: translateY(10px); }
    to { opacity: 1; transform: translateY(0); }
  }

  .step-pane h3 {
    font-size: 1.25rem;
    margin-bottom: 1rem;
    color: var(--khet-text-main);
  }

  .form-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1.5rem;
  }

  .package-selector {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 1rem;
    margin-bottom: 2rem;
  }

  .package-card {
    border: 1px solid var(--khet-border);
    border-radius: var(--khet-radius-md);
    padding: 1.5rem;
    cursor: pointer;
    transition: all 0.2s;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
  }

  .package-card:hover {
    border-color: var(--khet-primary);
  }

  .package-card.active {
    border-color: var(--khet-primary);
    background-color: var(--khet-primary-light);
    box-shadow: 0 4px 12px rgba(8, 175, 175, 0.1);
  }

  .pkg-info strong {
    display: block;
    font-size: 1.1rem;
    margin-bottom: 0.5rem;
  }

  .pkg-info span {
    font-size: 0.85rem;
    color: var(--khet-text-muted);
  }

  .pkg-price {
    font-size: 1.25rem;
    font-weight: 800;
    color: var(--khet-text-main);
    margin-top: 1rem;
    text-align: right;
  }

  .benefit-details {
    background-color: var(--khet-surface-alt);
    padding: 1.5rem;
    border-radius: var(--khet-radius-md);
  }

  .benefit-details h4 {
    font-size: 0.9rem;
    margin-bottom: 0.75rem;
    color: var(--khet-text-muted);
  }

  .benefit-details ul {
    margin: 0;
    padding-left: 1.25rem;
    font-size: 0.9rem;
    color: var(--khet-text-main);
  }

  .resource-grid {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
  }

  .resource-item {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }

  .resource-item label {
    font-size: 0.9rem;
    font-weight: 600;
  }

  .resource-item select {
    padding: 0.75rem;
    border: 1px solid var(--khet-border);
    border-radius: var(--khet-radius-md);
    font-family: inherit;
  }

  .review-summary {
    background-color: var(--khet-surface-alt);
    padding: 1.5rem;
    border-radius: var(--khet-radius-md);
    display: flex;
    flex-direction: column;
    gap: 1rem;
    margin-bottom: 2rem;
  }

  .summary-row {
    display: flex;
    justify-content: space-between;
    font-size: 1rem;
  }

  .confirmation-box {
    padding: 1rem;
    border: 1px solid var(--khet-border);
    border-radius: var(--khet-radius-md);
    background-color: #fff;
  }

  .checkbox-label {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    font-size: 0.9rem;
    cursor: pointer;
  }

  .wizard-footer {
    padding: 2rem;
    border-top: 1px solid var(--khet-border);
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .spacer {
    flex: 1;
  }
  </style>
</template>
