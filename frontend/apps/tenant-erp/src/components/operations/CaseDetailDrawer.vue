<template>
  <div class="drawer-overlay" @click.self="$emit('close')">
    <div class="drawer-content">
      <div class="drawer-header">
        <div class="header-top">
          <div class="case-info">
            <span class="case-id">{{ caseData.caseNumber }}</span>
            <h2>{{ caseData.deceasedName }}</h2>
          </div>
          <KButton variant="secondary" size="sm" @click="$emit('close')">Close</KButton>
        </div>
        <div class="status-bar">
          <span class="status-pill">{{ caseData.status }}</span>
          <span class="priority-pill" :class="caseData.priority">{{ caseData.priority }}</span>
        </div>
      </div>

      <div class="drawer-body">
        <section class="detail-section">
          <h3><span class="section-icon">📋</span> Case Details</h3>
          <div class="info-grid">
            <div class="info-item">
              <span class="label">Created Date</span>
              <span class="value">{{ caseData.createdDate }}</span>
            </div>
            <div class="info-item">
              <span class="label">Last Updated</span>
              <span class="value">{{ caseData.lastUpdatedDate }}</span>
            </div>
            <div class="info-item">
              <span class="label">Branch</span>
              <span class="value">Cape Town Central</span>
            </div>
            <div class="info-item">
              <span class="label">Next Action</span>
              <span class="value highlight">{{ caseData.nextAction }}</span>
            </div>
          </div>
        </section>

        <section class="detail-section">
          <h3><span class="section-icon">⚠️</span> Readiness Indicator</h3>
          <div class="readiness-container">
            <div class="readiness-header">
              <span class="readiness-score">{{ caseData.readinessScore }}% Complete</span>
              <span class="readiness-status">Action Required</span>
            </div>
            <div class="readiness-bar">
              <div class="progress" :style="{ width: caseData.readinessScore + '%' }"></div>
            </div>
            <div class="missing-docs">
              <div v-for="doc in caseData.missingDocs" :key="doc" class="doc-item">
                <span class="doc-icon">❌</span>
                <span>{{ doc }}</span>
              </div>
            </div>
          </div>
        </section>

        <section class="detail-section">
          <h3><span class="section-icon">🕒</span> Activity History</h3>
          <div class="timeline">
            <div v-for="i in 3" :key="i" class="timeline-item">
              <div class="timeline-marker"></div>
              <div class="timeline-content">
                <div class="timeline-header">
                  <span class="timeline-action">Case state changed to Arranging</span>
                  <span class="timeline-time">2 hours ago</span>
                </div>
                <p class="timeline-desc">Updated by Sarah Jenkins</p>
              </div>
            </div>
          </div>
        </section>
      </div>

      <div class="drawer-footer">
        <KButton variant="secondary" @click="alert('Opening Notes...')">Add Note</KButton>
        <KButton variant="primary" @click="$emit('start-arrangement')">Start Arrangement</KButton>
      </div>
    </template>

    <script setup lang="ts">
    import { FuneralCase } from './types';
    import { KButton } from '@khet360/ui-shared';

    defineProps<{
      caseData: FuneralCase;
    }>();

    defineEmits(['close', 'start-arrangement']);
    </script>

    <style scoped>
    .drawer-overlay {
      position: fixed;
      top: 0;
      right: 0;
      width: 100vw;
      height: 100vh;
      background-color: rgba(0, 0, 0, 0.3);
      z-index: 1000;
      display: flex;
      justify-content: flex-end;
      backdrop-filter: blur(2px);
    }

    .drawer-content {
      width: 500px;
      height: 100vh;
      background-color: white;
      box-shadow: -10px 0 30px rgba(0,0,0,0.1);
      display: flex;
      flex-direction: column;
      animation: slideIn 0.3s ease-out;
    }

    @keyframes slideIn {
      from { transform: translateX(100%); }
      to { transform: translateX(0); }
    }

    .drawer-header {
      padding: 2rem;
      border-bottom: 1px solid var(--khet-border);
      background-color: var(--khet-surface-alt);
    }

    .header-top {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 1.5rem;
    }

    .case-id {
      font-size: 0.8rem;
      font-weight: 700;
      color: var(--khet-text-muted);
      text-transform: uppercase;
    }

    .drawer-header h2 {
      font-size: 1.5rem;
      margin: 0;
      color: var(--khet-text-main);
    }

    .status-bar {
      display: flex;
      gap: 0.5rem;
    }

    .status-pill {
      font-size: 0.75rem;
      font-weight: 600;
      padding: 2px 10px;
      border-radius: 12px;
      background-color: var(--khet-primary-light);
      color: var(--khet-primary);
      border: 1px solid var(--khet-primary);
    }

    .priority-pill {
      font-size: 0.75rem;
      font-weight: 600;
      padding: 2px 10px;
      border-radius: 12px;
      color: white;
    }

    .priority-pill.Critical { background-color: #c0392b; }
    .priority-pill.High { background-color: #e67e22; }
    .priority-pill.Medium { background-color: #f1c40f; color: black; }
    .priority-pill.Low { background-color: #2ecc71; }

    .drawer-body {
      flex: 1;
      overflow-y: auto;
      padding: 2rem;
      display: flex;
      flex-direction: column;
      gap: 2.5rem;
    }

    .detail-section h3 {
      font-size: 1rem;
      margin-bottom: 1rem;
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .section-icon {
      font-size: 1.2rem;
    }

    .info-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 1.5rem;
    }

    .info-item {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .label {
      font-size: 0.75rem;
      color: var(--khet-text-muted);
    }

    .value {
      font-size: 0.9rem;
      font-weight: 500;
    }

    .value.highlight {
      color: var(--khet-primary);
      font-weight: 700;
    }

    .readiness-container {
      background-color: var(--khet-surface-alt);
      padding: 1.5rem;
      border-radius: var(--khet-radius-md);
    }

    .readiness-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 0.75rem;
    }

    .readiness-score {
      font-weight: 700;
      font-size: 0.9rem;
    }

    .readiness-status {
      font-size: 0.75rem;
      color: #c0392b;
      font-weight: 600;
    }

    .readiness-bar {
      height: 8px;
      background-color: #dee2e6;
      border-radius: 4px;
      overflow: hidden;
      margin-bottom: 1rem;
    }

    .progress {
      height: 100%;
      background-color: var(--khet-primary);
    }

    .missing-docs {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .doc-item {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      font-size: 0.85rem;
      color: var(--khet-text-muted);
    }

    .doc-icon {
      font-size: 0.8rem;
    }

    .timeline {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
      position: relative;
    }

    .timeline::before {
      content: '';
      position: absolute;
      left: 7px;
      top: 0;
      bottom: 0;
      width: 2px;
      background-color: var(--khet-border);
    }

    .timeline-item {
      display: flex;
      gap: 1rem;
      position: relative;
    }

    .timeline-marker {
      width: 16px;
      height: 16px;
      border-radius: 50%;
      background-color: white;
      border: 2px solid var(--khet-primary);
      margin-top: 4px;
      z-index: 1;
    }

    .timeline-content {
      flex: 1;
      padding-bottom: 1rem;
    }

    .timeline-header {
      display: flex;
      justify-content: space-between;
      align-items: baseline;
      margin-bottom: 0.25rem;
    }

    .timeline-action {
      font-size: 0.9rem;
      font-weight: 600;
    }

    .timeline-time {
      font-size: 0.75rem;
      color: var(--khet-text-muted);
    }

    .timeline-desc {
      font-size: 0.85rem;
      color: var(--khet-text-muted);
      margin: 0;
    }

    .drawer-footer {
      padding: 1.5rem 2rem;
      border-top: 1px solid var(--khet-border);
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
      background-color: white;
    }
    </style>
    </script>
