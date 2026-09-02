<template>
  <div class="public-site">
    <header class="site-header">
      <img src="/images/khet360_logo.png" alt="Khet-360 Logo" class="logo" />
      <nav class="nav-links">
        <a href="#features">Features</a>
        <a href="#pricing">Pricing</a>
        <KButton variant="primary" @click="scrollToOnboarding">Get Started</KButton>
      </nav>
    </header>

    <main>
      <!-- Hero Section -->
      <section class="hero">
        <div class="hero-content">
          <h1>The Connected Operating System for Funeral Services</h1>
          <p>Streamline your operations, ensure SARS compliance, and protect your margins with Khet-360.</p>
          <div class="hero-actions">
            <KButton variant="primary" size="lg" @click="scrollToOnboarding">Start Your Free Trial</KButton>
            <KButton variant="secondary" size="lg" @click="scrollToPricing">View Pricing</KButton>
          </div>
        </div>
      </section>

      <!-- Value Propositions -->
      <section id="features" class="features">
        <div class="section-header">
          <h2>Built for the South African Market</h2>
          <p>We solve the specific challenges of the local funeral industry.</p>
        </div>
        <div class="feature-grid">
          <KCard elevation="sm">
            <template #header><strong>🇿🇦 SARS Compliance</strong></template>
            <p>Automated EMP201 and EMP501 reporting. No more manual tax calculations or compliance anxiety.</p>
          </KCard>
          <KCard elevation="sm">
            <template #header><strong>📦 Inventory Control</strong></template>
            <p>Direct bridge between Insurance Claims and Inventory. Track every casket and urn in real-time.</p>
          </KCard>
          <KCard elevation="sm">
            <template #header><strong>🚀 Operational Agility</strong></template>
            <p>Connect your office to your field staff. Instant updates on case milestones and fleet status.</p>
          </KCard>
        </div>
      </section>

      <!-- Pricing Section -->
      <section id="pricing" class="pricing">
        <div class="section-header">
          <h2>Simple, Transparent Pricing</h2>
          <p>Choose the plan that fits your business scale.</p>
        </div>
        <PricingTable />
      </section>

      <!-- Onboarding Section -->
      <section id="onboarding" class="onboarding">
        <SubscriptionWizard />
      </section>
    </main>

    <footer class="site-footer">
      <p>&copy; 2026 Khet-360. All rights reserved.</p>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { KButton, KCard } from '@khet360/ui-shared';
import PricingTable from './components/PricingTable.vue';
import SubscriptionWizard from './components/SubscriptionWizard.vue';

const scrollToPricing = () => {
  document.getElementById('pricing')?.scrollIntoView({ behavior: 'smooth' });
};

const scrollToOnboarding = () => {
  document.getElementById('onboarding')?.scrollIntoView({ behavior: 'smooth' });
};
</script>

<style scoped>
.public-site {
  font-family: var(--khet-font-main);
  background-color: var(--khet-bg);
  color: var(--khet-text-main);
  min-height: 100vh;
}

.site-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem 4rem;
  background-color: var(--khet-surface);
  border-bottom: 1px solid var(--khet-border);
  position: sticky;
  top: 0;
  z-index: 100;
}

.logo {
  height: 40px;
}

.nav-links {
  display: flex;
  align-items: center;
  gap: 2rem;
}

.nav-links a {
  text-decoration: none;
  color: var(--khet-text-muted);
  font-weight: 500;
  transition: color 0.2s;
}

.nav-links a:hover {
  color: var(--khet-primary);
}

.hero {
  padding: 8rem 4rem;
  text-align: center;
  background: linear-gradient(135deg, var(--khet-surface) 0%, var(--khet-primary-light) 100%);
}

.hero-content h1 {
  font-size: 3.5rem;
  font-weight: 800;
  margin-bottom: 1.5rem;
  color: var(--khet-text-main);
  max-width: 900px;
  margin-left: auto;
  margin-right: auto;
  line-height: 1.1;
}

.hero-content p {
  font-size: 1.25rem;
  color: var(--khet-text-muted);
  margin-bottom: 3rem;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
}

.hero-actions {
  display: flex;
  justify-content: center;
  gap: 1rem;
}

.features {
  padding: 6rem 4rem;
}

.section-header {
  text-align: center;
  margin-bottom: 4rem;
}

.section-header h2 {
  font-size: 2.5rem;
  margin-bottom: 1rem;
}

.feature-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
}

.pricing {
  padding: 6rem 4rem;
  background-color: var(--khet-surface-alt);
}

.onboarding {
  padding: 6rem 4rem;
  display: flex;
  justify-content: center;
}

.site-footer {
  padding: 3rem 4rem;
  text-align: center;
  border-top: 1px solid var(--khet-border);
  color: var(--khet-text-muted);
  font-size: 0.875rem;
}
</style>
