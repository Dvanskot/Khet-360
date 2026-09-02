<template>
  <div class="page">
    <header class="page-header">
      <h1 class="title">Policy Enrollment</h1>
      <router-link to="/crm" class="back-link">← Back</router-link>
    </header>

    <div class="stepper">
      <div
        v-for="(step, idx) in steps"
        :key="idx"
        class="step"
        :class="{ active: currentStep === idx, completed: currentStep > idx }"
      >
        <div class="step-indicator">{{ currentStep > idx ? '✓' : idx + 1 }}</div>
        <div class="step-label">{{ step }}</div>
      </div>
    </div>

    <div class="wizard-content">
      <!-- Step 1: Plan Selection -->
      <div v-if="currentStep === 0" class="step-pane">
        <h2>Select Plan</h2>
        <p class="subtitle">Choose the coverage level for the policy holder.</p>
        <div class="plan-grid">
          <div
            v-for="plan in availablePlans"
            :key="plan.code"
            class="plan-card"
            :class="{ selected: selectedPlan?.code === plan.code }"
            @click="selectedPlan = plan"
          >
            <div class="plan-header">
              <h3 :class="{ 'highlight': selectedPlan?.code === plan.code }">{{ plan.name }}</h3>
              <span class="plan-code">{{ plan.code }}</span>
            </div>
            <div class="plan-details">
              <div class="detail-row">
                <span>Main Member Cover</span>
                <strong>{{ plan.mainCover }}</strong>
              </div>
              <div class="detail-row">
                <span>Dependents</span>
                <strong>{{ plan.dependentLimit }}</strong>
              </div>
              <div class="detail-row">
                <span>Monthly Premium</span>
                <strong>{{ plan.premium }}</strong>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Step 2: Main Member -->
      <div v-if="currentStep === 1" class="step-pane">
        <h2>Main Member</h2>
        <p class="subtitle">Enter details for the primary policy holder.</p>
        <form class="enroll-form">
          <div class="form-group">
            <label>Full Name</label>
            <input v-model="formData.mainMember.name" placeholder="Full legal name" required />
          </div>
          <div class="form-group">
            <label>ID / Passport Number</label>
            <input v-model="formData.mainMember.idNumber" placeholder="ID number" required />
          </div>
          <div class="form-group">
            <label>Date of Birth</label>
            <input type="date" v-model="formData.mainMember.dob" required />
          </div>
          <div class="form-group">
            <label>Contact Number</label>
            <input type="tel" v-model="formData.mainMember.phone" placeholder="082..." required />
          </div>
          <div class="form-group">
            <label>ID Document (Photo)</label>
            <div class="file-upload-sim">
              <input type="file" @change="simulateUpload" accept="image/*" />
              <span v-if="formData.mainMember.idUploaded">✅ Uploaded</span>
            </div>
          </div>
        </form>
      </div>

      <!-- Step 3: Dependents & Beneficiaries -->
      <div v-if="currentStep === 2" class="step-pane">
        <h2>Family Members</h2>
        <p class="subtitle">Add dependents and beneficiaries to the policy.</p>

        <div class="member-list">
          <div v-for="(member, idx) in formData.members" :key="idx" class="member-card">
            <div class="member-header">
              <span class="member-role">{{ member.role }}</span>
              <button @click="removeMember(idx)" class="btn-remove">×</button>
            </div>
            <div class="form-group">
              <label>Name</label>
              <input v-model="member.name" />
            </div>
            <div class="form-group">
              <label>Relationship</label>
              <select v-model="member.relationship">
                <option value="Spouse">Spouse</option>
                <option value="Child">Child</option>
                <option value="Parent">Parent</option>
                <option value="Other">Other</option>
              </select>
            </div>
          </div>
        </div>

        <button @click="addMember" class="btn-add-member">
          + Add Member
        </button>
      </div>

      <!-- Step 4: Review & Submit -->
      <div v-if="currentStep === 3" class="step-pane">
        <h2>Review Policy</h2>
        <p class="subtitle">Please verify all details before final submission.</p>

        <div class="review-box">
          <div class="review-section">
            <h3 class="section-title">Plan</h3>
            <div class="review-row">
              <span>Plan Name</span>
              <strong>{{ selectedPlan?.name }}</strong>
            </div>
          </div>

          <div class="review-section">
            <h3 class="section-title">Main Member</h3>
            <div class="review-row">
              <span>Name</span>
              <strong>{{ formData.mainMember.name }}</strong>
            </div>
            <div class="review-row">
              <span>ID Number</span>
              <strong>{{ formData.mainMember.idNumber }}</strong>
            </div>
          </div>

          <div class="review-section">
            <h3 class="section-title">Family Members ({{ formData.members.length }})</h3>
            <div v-for="m in formData.members" class="review-row">
              <span>{{ m.relationship }}: {{ m.name }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="wizard-footer">
      <button
        v-if="currentStep > 0"
        @click="currentStep--"
        class="btn-secondary"
      >
        Back
      </button>
      <button
        v-if="currentStep < 3"
        @click="currentStep++"
        class="btn-primary"
        :disabled="!isStepValid"
      >
        Continue
      </button>
      <button
        v-if="currentStep === 3"
        @click="submitEnrollment"
        class="btn-success"
      >
        Finalize Enrollment
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const currentStep = ref(0);
const steps = ['Plan', 'Main Member', 'Family', 'Review'];

const availablePlans = [
  { code: 'SILVER', name: 'Silver Care', mainCover: 'R15,000', dependentLimit: 4, premium: 'R150/mo' },
  { code: 'GOLD', name: 'Gold Elite', mainCover: 'R30,000', dependentLimit: 8, premium: 'R300/mo' },
  { code: 'PLATINUM', name: 'Platinum Peace', mainCover: 'R60,000', dependentLimit: 12, premium: 'R500/mo' },
];

const selectedPlan = ref(availablePlans[0]);

const formData = ref({
  mainMember: {
    name: '',
    idNumber: '',
    dob: '',
    phone: '',
    idUploaded: false,
  },
  members: [] as any[],
});

const isStepValid = computed(() => {
  if (currentStep.value === 0) return !!selectedPlan.value;
  if (currentStep.value === 1) {
    return formData.value.mainMember.name &&
           formData.value.mainMember.idNumber &&
           formData.value.mainMember.phone;
  }
  return true;
});

function simulateUpload() {
  formData.value.mainMember.idUploaded = true;
}

function addMember() {
  formData.value.members.push({
    name: '',
    relationship: 'Child',
    idNumber: '',
  });
}

function removeMember(idx: number) {
  formData.value.members.splice(idx, 1);
}

async function submitEnrollment() {
  try {
    // Simulate API Call
    alert('Policy submitted successfully! Application is under review.');
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
  margin-bottom: 1.5rem;
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

.stepper {
  display: flex;
  justify-content: space-between;
  margin-bottom: 2rem;
  position: relative;
}

.step {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  flex: 1;
  position: relative;
  z-index: 1;
}

.step-indicator {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: var(--khet-surface);
  border: 2px solid var(--khet-border);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.85rem;
  font-weight: 600;
  color: var(--khet-text-muted);
  transition: all 0.3s;
}

.step.active .step-indicator {
  border-color: var(--khet-primary);
  color: var(--khet-primary);
  box-shadow: 0 0 0 4px rgba(79, 70, 229, 0.1);
}

.step.completed .step-indicator {
  background: var(--khet-primary);
  border-color: var(--khet-primary);
  color: white;
}

.step-label {
  font-size: 0.7rem;
  font-weight: 500;
  color: var(--khet-text-muted);
}

.wizard-content {
  min-height: 300px;
  margin-bottom: 5rem;
}

.step-pane {
  animation: fadeIn 0.3s ease-out;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.subtitle {
  color: var(--khet-text-muted);
  font-size: 0.9rem;
  margin-bottom: 1.5rem;
}

.plan-grid {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.plan-card {
  background: var(--khet-surface);
  border: 1px solid var(--khet-border);
  border-radius: 16px;
  padding: 1.25rem;
  cursor: pointer;
  transition: all 0.2s;
}

.plan-card.selected {
  border-color: var(--khet-primary);
  background: rgba(79, 70, 229, 0.02);
}

.plan-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.plan-header h3 {
  margin: 0;
  font-size: 1.1rem;
}

.plan-header h3.highlight {
  color: var(--khet-primary);
}

.plan-code {
  font-size: 0.7rem;
  font-weight: 700;
  background: rgba(0,0,0,0.05);
  padding: 2px 6px;
  border-radius: 4px;
}

.plan-details {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  font-size: 0.85rem;
}

.enroll-form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group label {
  font-size: 0.85rem;
  font-weight: 600;
  color: var(--khet-text-muted);
}

.form-group input, .form-group select {
  padding: 12px;
  border-radius: 8px;
  border: 1px solid var(--khet-border);
  background: var(--khet-bg);
}

.file-upload-sim {
  border: 2px dashed var(--khet-border);
  padding: 1rem;
  border-radius: 12px;
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
}

.member-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.member-card {
  background: var(--khet-surface);
  border: 1px solid var(--khet-border);
  border-radius: 12px;
  padding: 1rem;
}

.member-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.75rem;
}

.member-role {
  font-size: 0.75rem;
  font-weight: 700;
  color: var(--khet-primary);
  text-transform: uppercase;
}

.btn-remove {
  background: transparent;
  border: none;
  color: #ef4444;
  font-size: 1.2rem;
  cursor: pointer;
}

.btn-add-member {
  width: 100%;
  padding: 12px;
  background: transparent;
  border: 2px dashed var(--khet-border);
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  color: var(--khet-text-muted);
}

.review-box {
  background: var(--khet-surface);
  border: 1px solid var(--khet-border);
  border-radius: 16px;
  padding: 1.25rem;
}

.review-section {
  margin-bottom: 1.5rem;
}

.review-section:last-child {
  margin-bottom: 0;
}

.section-title {
  font-size: 0.85rem;
  text-transform: uppercase;
  color: var(--khet-text-muted);
  margin-bottom: 0.75rem;
  border-bottom: 1px solid var(--khet-border);
  padding-bottom: 4px;
}

.review-row {
  display: flex;
  justify-content: space-between;
  font-size: 0.9rem;
  margin-bottom: 0.5rem;
}

.wizard-footer {
  position: fixed;
  bottom: 80px;
  left: 1.5rem;
  right: 1.5rem;
  display: flex;
  gap: 1rem;
  z-index: 10;
}

.btn-primary, .btn-success {
  flex: 1;
  padding: 14px;
  border-radius: 12px;
  font-weight: 600;
  border: none;
  cursor: pointer;
  color: white;
}

.btn-primary { background: var(--khet-primary); }
.btn-success { background: #10b981; }
.btn-secondary {
  padding: 14px 24px;
  border-radius: 12px;
  border: 1px solid var(--khet-border);
  background: var(--khet-surface);
  cursor: pointer;
  font-weight: 600;
}

.btn-primary:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>
