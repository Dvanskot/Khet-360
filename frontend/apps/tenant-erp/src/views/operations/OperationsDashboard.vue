<template>
  <div class="operations-view">
    <div class="view-header">
      <div class="header-left">
        <h1>Funeral Command Centre</h1>
        <p>Real-time operational overview of all active funeral cases.</p>
      </div>
      <div class="header-actions">
        <KButton variant="secondary" @click="refreshBoard">🔄 Refresh</KButton>
        <KButton variant="primary" @click="createNewCase">➕ New Funeral Case</KButton>
      </div>
    </div>

    <div class="kanban-board">
      <div v-for="stage in workflowStages" :key="stage.id" class="kanban-column">
        <div class="column-header">
          <div class="stage-title">
            <span class="stage-dot" :style="{ backgroundColor: stage.color }"></span>
            {{ stage.label }}
          </div>
          <span class="case-count">{{ getCasesForStage(stage.id).length }}</span>
        </div>
        <div class="column-content">
          <CaseCard
            v-for="caseItem in getCasesForStage(stage.id)"
            :key="caseItem.id"
            :case-data="caseItem"
            @select="selectCase(caseItem)"
          />
          <div v-if="getCasesForStage(stage.id).length === 0" class="empty-column">
            No cases in this stage
          </div>
        </div>
      </div>
    </div>

    <CaseDetailDrawer
      v-if="selectedCase"
      :case-data="selectedCase"
      @close="selectedCase = null"
      @start-arrangement="startArrangement"
    />

    <ArrangementWizard
      v-if="showWizard"
      @close="showWizard = false"
    />
  </template>

  <script setup lang="ts">
  import { ref, computed } from 'vue';
  import { KButton } from '@khet360/ui-shared';
  import { FuneralCase, WorkflowStage, CaseStatus } from './components/operations/types';
  import CaseCard from './components/operations/CaseCard.vue';
  import CaseDetailDrawer from './components/operations/CaseDetailDrawer.vue';
  import ArrangementWizard from './wizard/ArrangementWizard.vue';


  const workflowStages: WorkflowStage[] = [
    { id: 'Draft', label: 'Draft', color: '#95a5a6' },
    { id: 'Open', label: 'Open', color: '#3498db' },
    { id: 'Arranging', label: 'Arranging', color: '#f1c40f' },
    { id: 'Confirmed', label: 'Confirmed', color: '#9b59b6' },
    { id: 'InService', label: 'In Service', color: '#e67e22' },
    { id: 'Completed', label: 'Completed', color: '#2ecc71' },
    { id: 'Closed', label: 'Closed', color: '#7f8c8d' },
  ];

  const cases = ref<FuneralCase[]>([
    { id: '1', caseNumber: 'C-1024', deceasedName: 'Samuel Tshikota', status: 'Open', priority: 'High', nextAction: 'Verify Death Cert', branchId: 'B1', createdDate: '2026-08-25', lastUpdatedDate: '2026-08-30', readinessScore: 45, missingDocs: ['Death Certificate', 'ID Copy'] },
    { id: '2', caseNumber: 'C-1021', deceasedName: 'Mary Mokwena', status: 'Arranging', priority: 'Medium', nextAction: 'Confirm Venue', branchId: 'B1', createdDate: '2026-08-20', lastUpdatedDate: '2026-08-31', readinessScore: 75, missingDocs: ['Catering Menu'] },
    { id: '3', caseNumber: 'C-1018', deceasedName: 'John Dlamini', status: 'Open', priority: 'Critical', nextAction: 'Process Payout', branchId: 'B2', createdDate: '2026-08-28', lastUpdatedDate: '2026-08-31', readinessScore: 30, missingDocs: ['Policy Docs', 'Beneficiary ID'] },
    { id: '4', caseNumber: 'C-1005', deceasedName: 'Alice Sibiya', status: 'Confirmed', priority: 'Low', nextAction: 'Dispatch Fleet', branchId: 'B1', createdDate: '2026-08-15', lastUpdatedDate: '2026-08-25', readinessScore: 95, missingDocs: [] },
    { id: '5', caseNumber: 'C-1001', deceasedName: 'Robert Zulu', status: 'Draft', priority: 'Medium', nextAction: 'Complete Initial Form', branchId: 'B3', createdDate: '2026-09-01', lastUpdatedDate: '2026-09-01', readinessScore: 10, missingDocs: ['All Initial Docs'] },
  ]);

  const selectedCase = ref<FuneralCase | null>(null);
  const showWizard = ref(false);


  const getCasesForStage = (status: CaseStatus) => {
    return cases.value.filter(c => c.status === status);
  };

  const selectCase = (caseItem: FuneralCase) => {
    selectedCase.value = caseItem;
  };

  const refreshBoard = () => {
    alert('Refreshing cases from server...');
  };

  const createNewCase = () => {
    alert('Opening Create Case Wizard...');
  };

  const startArrangement = () => {
    showWizard.value = true;
    selectedCase.value = null;
  };
  </script>

  <style scoped>
  .operations-view {
    display: flex;
    flex-direction: column;
    gap: 2rem;
    height: calc(100vh - 128px);
  }

  .view-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1rem;
  }

  .header-left h1 {
    font-size: 2rem;
    margin: 0 0 0.5rem 0;
    color: var(--khet-text-main);
  }

  .header-left p {
    color: var(--khet-text-muted);
    font-size: 1.1rem;
  }

  .header-actions {
    display: flex;
    gap: 1rem;
  }

  .kanban-board {
    display: flex;
    gap: 1.5rem;
    overflow-x: auto;
    padding-bottom: 1rem;
    flex: 1;
  }

  .kanban-column {
    width: 320px;
    min-width: 320px;
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  .column-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0 0.5rem;
    margin-bottom: 0.5rem;
  }

  .stage-title {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-weight: 700;
    font-size: 0.9rem;
    text-transform: uppercase;
    color: var(--khet-text-muted);
  }

  .stage-dot {
    width: 8px;
    height: 8px;
    border-radius: 50%;
  }

  .case-count {
    font-size: 0.8rem;
    background-color: var(--khet-border);
    padding: 2px 8px;
    border-radius: 12px;
    color: var(--khet-text-muted);
  }

  .column-content {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    min-height: 200px;
  }

  .empty-column {
    text-align: center;
    padding: 3rem 1rem;
    border: 2px dashed var(--khet-border);
    border-radius: var(--khet-radius-md);
    color: var(--khet-text-muted);
    font-size: 0.85rem;
    font-style: italic;
  }
  </style>
</template>
