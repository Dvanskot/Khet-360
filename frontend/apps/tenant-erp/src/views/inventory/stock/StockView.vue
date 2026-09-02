<template>
  <div class="view-container">
    <div class="view-header">
      <h1>Stock Management</h1>
      <p>Real-time inventory tracking across all branches.</p>
    </div>
    <div class="placeholder-content">
      <KCard elevation="sm" class="empty-card">
        <div class="empty-state">
          <span class="icon">📦</span>
          <h3>Stock View is under construction</h3>
          <p>This will provide detailed SKU tracking, adjustments and audits.</p>
          <KButton variant="primary">Perform Stock Count</KButton>
        </div>
      </KCard>
    </div>
  </template>

  <script setup lang="ts">
  import { KButton, KCard } from '@khet360/ui-shared';
  </script>

  <style scoped>
  .view-container {
    display: flex;
    flex-direction: column;
    gap: 2rem;
  }

  .view-header {
    margin-bottom: 1rem;
  }

  .view-header h1 {
    font-size: 2rem;
    margin: 0 0 0.5rem 0;
  }

  .view-header p {
    color: var(--khet-text-muted);
    font-size: 1.1rem;
  }

  .placeholder-content {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 60vh;
  }

  .empty-card {
    max-width: 500px;
    text-align: center;
    padding: 3rem;
  }

  .empty-state {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 1rem;
  }

  .icon {
    font-size: 4rem;
    margin-bottom: 1rem;
  }

  .empty-state h3 {
    font-size: 1.5rem;
    margin: 0;
  }

  .empty-state p {
    color: var(--khet-text-muted);
    margin-bottom: 2rem;
  }
  </style>
</template>
