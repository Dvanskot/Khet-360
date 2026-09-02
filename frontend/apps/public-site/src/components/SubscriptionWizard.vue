<template>
  <div class="wizard-container">
    <KCard elevation="md">
      <template #header>
        <div class="wizard-header">
          <h3>Start Your Khet-360 Journey</h3>
          <p>Enter your details to begin your digital transformation.</p>
        </div>
      </template>

      <div class="wizard-body">
        <div v-if="step === 'plan'" class="step-content">
          <div class="step-title">Select Your Plan</div>
          <div class="plan-selector">
            <div
              v-for="plan in plans"
              :key="plan.id"
              :class="['plan-option', { active: selectedPlanId === plan.id }]"
              @click="selectedPlanId = plan.id"
            >
              <div class="plan-info">
                <strong>{{ plan.name }}</strong>
                <span>{{ plan.monthlyPrice }} {{ plan.currency }}/mo</span>
              </div>
              <div class="plan-radio"></div>
            </div>
          </div>
          <div class="wizard-actions">
            <KButton variant="primary" :disabled="!selectedPlanId" @click="nextStep">Continue</KButton>
          </div>
        </div>

        <div v-if="step === 'details'" class="step-content">
          <div class="step-title">Company Details</div>
          <div class="form-grid">
            <KInput label="Company Name" v-model="form.companyName" placeholder="e.g. Alpha Funeral Services" />
            <KInput label="Business Slug" v-model="form.slug" placeholder="e.g. alpha-funerals" />
            <KInput label="Contact Email" type="email" v-model="form.email" placeholder="admin@company.co.za" />
          </div>
          <div class="wizard-actions">
            <KButton variant="secondary" @click="step = 'plan'">Back</KButton>
            <KButton variant="primary" @click="submitTrial">Start 14-Day Free Trial</KButton>
          </div>
        </div>

        <div v-if="step === 'success'" class="step-content success-state">
          <div class="success-icon">🎉</div>
          <h3>Your Workspace is Ready!</h3>
          <p>We are provisioning your isolated tenant database. You will receive an email shortly with your login credentials.</p>
          <KButton variant="primary" @click="reset">Finish</KButton>
        </div>
      </div>
    </KCard>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { KButton, KInput, KCard } from '@khet360/ui-shared';
import { SubscriptionApi, SubscriptionPlan } from '@khet360/api-client';

const step = ref<'plan' | 'details' | 'success'>('plan');
const plans = ref<SubscriptionPlan[]>([]);
const selectedPlanId = ref<string | null>(null);
const form = ref({
  companyName: '',
  slug: '',
  email: ''
});

onMounted(async () => {
  try {
    plans.value = await SubscriptionApi.getPublicPlans();
  } catch (e) {
    console.error("Failed to load plans");
  }
});

const nextStep = () => {
  step.value = 'details';
};

const submitTrial = async () => {
  try {
    await SubscriptionApi.startTrial({
      companyName: form.value.companyName,
      slug: form.value.slug,
      subscriptionPlanId: selectedPlanId.value!,
      email: form.value.email
    });
    step.value = 'success';
  } catch (e) {
    alert("Onboarding failed. Please check your details.");
  }
};

const reset = () => {
  step.value = 'plan';
  form.value = { companyName: '', slug: '', email: '' };
};
</script>

<style scoped>
.wizard-container {
  width: 100%;
  max-width: 600px;
  margin: 0 auto;
}

.wizard-header {
  text-align: center;
}

.wizard-header h3 {
  margin: 0;
  font-size: 1.5rem;
}

.wizard-header p {
  color: var(--khet-text-muted);
  font-size: 0.9rem;
}

.step-title {
  font-size: 1.25rem;
  font-weight: 700;
  margin-bottom: 1.5rem;
  text-align: center;
}

.plan-selector {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 2rem;
}

.plan-option {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border: 1px solid var(--khet-border);
  border-radius: var(--khet-radius-md);
  cursor: pointer;
  transition: all 0.2s ease;
}

.plan-option:hover {
  border-color: var(--khet-primary);
}

.plan-option.active {
  border-color: var(--khet-primary);
  background-color: var(--khet-primary-light);
}

.plan-info {
  display: flex;
  flex-direction: column;
}

.plan-info strong {
  font-size: 1rem;
}

.plan-info span {
  font-size: 0.85rem;
  color: var(--khet-text-muted);
}

.plan-radio {
  width: 20px;
  height: 20px;
  border: 2px solid var(--khet-border);
  border-radius: 50%;
  position: relative;
}

.plan-option.active .plan-radio {
  border-color: var(--khet-primary);
}

.plan-option.active .plan-radio::after {
  content: '';
  position: absolute;
  top: 4px;
  left: 4px;
  width: 8px;
  height: 8px;
  background-color: var(--khet-primary);
  border-radius: 50%;
}

.form-grid {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.wizard-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
}

.success-state {
  text-align: center;
  padding: 2rem 0;
}

.success-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
}
</style>
