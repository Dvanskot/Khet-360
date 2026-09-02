<template>
  <div class="case-card" @click="$emit('select')">
    <div class="card-header">
      <span class="case-number">{{ caseData.caseNumber }}</span>
      <span class="priority-dot" :class="caseData.priority"></span>
    </div>
    <div class="card-body">
      <h4 class="deceased-name">{{ caseData.deceasedName }}</h4>
      <p class="next-action">
        <span class="action-label">Next:</span> {{ caseData.nextAction }}
      </p>
    </div>
    <div class="card-footer">
      <div class="readiness-bar">
        <div class="progress" :style="{ width: caseData.readinessScore + '%' }"></div>
      </div>
      <span class="readiness-text">{{ caseData.readinessScore }}% Ready</span>
    </div>
  </template>

  <script setup lang="ts">
  import { FuneralCase } from './types';

  defineProps<{
    caseData: FuneralCase;
  }>();

  defineEmits(['select']);
  </script>

  <style scoped>
  .case-card {
    background-color: white;
    border: 1px solid var(--khet-border);
    border-radius: var(--khet-radius-md);
    padding: 1rem;
    cursor: pointer;
    transition: all 0.2s ease;
    box-shadow: 0 2px 4px rgba(0,0,0,0.05);
  }

  .case-card:hover {
    border-color: var(--khet-primary);
    transform: translateY(-2px);
    box-shadow: 0 4px 8px rgba(0,0,0,0.1);
  }

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 0.75rem;
  }

  .case-number {
    font-size: 0.75rem;
    font-weight: 700;
    color: var(--khet-text-muted);
    text-transform: uppercase;
  }

  .priority-dot {
    width: 8px;
    height: 8px;
    border-radius: 50%;
  }

  .priority-dot.Critical { background-color: #c0392b; }
  .priority-dot.High { background-color: #e67e22; }
  .priority-dot.Medium { background-color: #f1c40f; }
  .priority-dot.Low { background-color: #2ecc71; }

  .deceased-name {
    font-size: 1rem;
    margin: 0 0 0.5rem 0;
    color: var(--khet-text-main);
  }

  .next-action {
    font-size: 0.85rem;
    color: var(--khet-text-muted);
    display: flex;
    gap: 0.25rem;
  }

  .action-label {
    font-weight: 600;
    color: var(--khet-text-main);
  }

  .card-footer {
    margin-top: 1rem;
    padding-top: 0.75rem;
    border-top: 1px solid var(--khet-border);
    display: flex;
    align-items: center;
    gap: 0.75rem;
  }

  .readiness-bar {
    flex: 1;
    height: 6px;
    background-color: var(--khet-surface-alt);
    border-radius: 3px;
    overflow: hidden;
  }

  .progress {
    height: 100%;
    background-color: var(--khet-primary);
    transition: width 0.3s ease;
  }

  .readiness-text {
    font-size: 0.7rem;
    font-weight: 600;
    color: var(--khet-text-muted);
    min-width: 40px;
    text-align: right;
  }
  </style>
</template>
