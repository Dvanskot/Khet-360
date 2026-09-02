<template>
  <div class="pricing-container">
    <div class="pricing-grid">
      <KCard
        v-for="plan in plans"
        :key="plan.id"
        :elevation="plan.category === 1 ? 'lg' : 'sm'"
        :class="{ 'plan-card--highlighted': plan.category === 1 }"
      >
        <template #header>
          <div class="plan-header">
            <span class="plan-category">{{ getCategoryName(plan.category) }}</span>
            <h3 class="plan-name">{{ plan.name }}</h3>
            <div class="plan-price">
              <span class="price-amount">{{ plan.monthlyPrice }}</span>
              <span class="price-currency">{{ plan.currency }}</span>
              <span class="price-period">/mo</span>
            </div>
            <p class="plan-description">{{ plan.description }}</p>
          </div>
        </template>

        <div class="plan-features">
          <div v-for="ent in plan.entitlements" :key="ent.code" class="feature-item">
            <span class="feature-icon">✓</span>
            <span class="feature-text">{{ ent.description }} ({{ ent.limitValue }})</span>
          </div>
        </div>

        <template #footer>
          <KButton
            :variant="plan.category === 1 ? 'primary' : 'secondary'"
            @click="$emit('select-plan', plan)"
          >
            Choose {{ plan.name }}
          </KButton>
        </template>
      </KCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { KButton, KCard } from '@khet360/ui-shared';
import { SubscriptionApi, SubscriptionPlan } from '@khet360/api-client';

const emit = defineEmits(['select-plan']);
const plans = ref<SubscriptionPlan[]>([]);
const isLoading = ref(true);
const error = ref<string | null>(null);

const getCategoryName = (category: number) => {
  const categories = { 0: 'Basic', 1: 'Professional', 2: 'Enterprise' };
  return categories[category as keyof typeof categories] || 'Custom';
};

onMounted(async () => {
  try {
    plans.value = await SubscriptionApi.getPublicPlans();
  } catch (e) {
    error.value = "Failed to load pricing plans. Please refresh the page.";
  } finally {
    isLoading.value = false;
  }
});
</script>

<style scoped>
.pricing-container {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
}

.pricing-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
  justify-items: center;
}

.plan-card--highlighted {
  border: 2px solid var(--khet-primary);
  transform: scale(1.05);
  z-index: 10;
}

.plan-header {
  text-align: center;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
}

.plan-category {
  font-size: 0.75rem;
  font-weight: 700;
  text-transform: uppercase;
  color: var(--khet-primary);
  letter-spacing: 0.1em;
}

.plan-name {
  font-size: 1.5rem;
  margin: 0;
  color: var(--khet-text-main);
}

.plan-price {
  display: flex;
  align-items: baseline;
  gap: 0.25rem;
  margin: 1rem 0;
}

.price-amount {
  font-size: 2.5rem;
  font-weight: 800;
  color: var(--khet-text-main);
}

.price-currency, .price-period {
  color: var(--khet-text-muted);
  font-size: 1rem;
}

.plan-description {
  font-size: 0.9rem;
  color: var(--khet-text-muted);
  text-align: center;
  margin-bottom: 1.5rem;
}

.plan-features {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-bottom: 2rem;
}

.feature-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
  color: var(--khet-text-main);
}

.feature-icon {
  color: var(--khet-success);
  font-weight: bold;
}
</style>
