<template>
  <div class="page">
    <header class="page-header">
      <h1 class="title">Production Shop</h1>
      <router-link to="/ops" class="back-link">← Hub</router-link>
    </header>

    <div class="production-list">
      <div v-if="orders.length === 0" class="empty-state">
        <span class="emoji">🔨</span>
        <p>No active production orders.</p>
      </div>

      <div
        v-for="order in orders"
        :key="order.id"
        class="order-card"
      >
        <div class="order-header">
          <span class="memorial-id">{{ order.memorialId }}</span>
          <span class="status-badge" :class="order.status.toLowerCase()">
            {{ order.status }}
          </span>
        </div>

        <div class="stage-tracker">
          <div
            v-for="(stage, index) in STAGES"
            :key="stage"
            class="stage-node"
            :class="{
              completed: isStageCompleted(order, index),
              current: order.stage === stage
            }"
          >
            <div class="node-indicator">
              <span v-if="isStageCompleted(order, index)">✓</span>
            </div>
            <span class="stage-label">{{ stage }}</span>
          </div>
        </div>

        <div class="order-actions">
          <button
            v-if="order.status === 'Active'"
            @click="advanceStage(order)"
            class="btn-advance"
          >
            Advance to {{ getNextStage(order.stage) }}
          </button>
          <button
            v-if="order.status === 'Active'"
            @click="updateOrderStatus(order, 'OnHold')"
            class="btn-hold"
          >
            Put on Hold
          </button>
          <button
            v-if="order.status === 'OnHold'"
            @click="updateOrderStatus(order, 'Active')"
            class="btn-resume"
          >
            Resume Production
          </button>
          <span v-if="order.stage === 'Ready'" class="ready-badge">Ready for Delivery</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { db } from '@/db/schema';
import { syncEngine } from '@/sync/sync-engine';
import { LocalProductionOrder } from '@/db/schema';

const STAGES = [
  'SlabSelection', 'Cutting', 'Polishing',
  'Engraving', 'Finishing', 'QualityCheck', 'Ready'
];

const orders = ref<LocalProductionOrder[]>([]);

async function loadOrders() {
  orders.value = await db.production.where('status').equals('Active').or('status').equals('OnHold').toArray();
}

function isStageCompleted(order: LocalProductionOrder, index: number) {
  const currentIdx = STAGES.indexOf(order.stage);
  return index < currentIdx;
}

function getNextStage(currentStage: string) {
  const idx = STAGES.indexOf(currentStage);
  return STAGES[idx + 1] || 'Ready';
}

async function advanceStage(order: LocalProductionOrder) {
  const nextStage = getNextStage(order.stage);
  await syncEngine.executeCommand({
    entityType: 'ProductionOrder',
    entityId: order.id,
    action: 'UPDATE',
    payload: { stage: nextStage },
  });
  await loadOrders();
}

async function updateOrderStatus(order: LocalProductionOrder, newStatus: LocalProductionOrder['status']) {
  await syncEngine.executeCommand({
    entityType: 'ProductionOrder',
    entityId: order.id,
    action: 'UPDATE',
    payload: { status: newStatus },
  });
  await loadOrders();
}

onMounted(async () => {
  await loadOrders();
  setInterval(loadOrders, 5000);
});
</script>

<style scoped>
.page {
  padding: 1.5rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.title {
  font-size: 1.8rem;
  font-weight: 700;
  margin: 0;
}

.back-link {
  font-size: 0.85rem;
  color: var(--khet-primary);
  text-decoration: none;
  font-weight: 600;
}

.production-list {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.order-card {
  background: var(--khet-surface);
  border-radius: 20px;
  padding: 1.25rem;
  border: 1px solid var(--khet-border);
}

.order-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.memorial-id {
  font-weight: 700;
  font-size: 1.1rem;
}

.status-badge {
  font-size: 0.7rem;
  padding: 4px 8px;
  border-radius: 6px;
  font-weight: 700;
  text-transform: uppercase;
  color: white;
}

.status-badge.active { background: #3b82f6; }
.status-badge.onhold { background: #f59e0b; }

.stage-tracker {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-bottom: 1.5rem;
}

.stage-node {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.85rem;
  color: var(--khet-text-muted);
  transition: all 0.3s;
}

.node-indicator {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  border: 2px solid var(--khet-border);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.7rem;
  font-weight: bold;
}

.stage-node.completed {
  color: #10b981;
}

.stage-node.completed .node-indicator {
  background: #10b981;
  border-color: #10b981;
  color: white;
}

.stage-node.current {
  color: var(--khet-primary);
  font-weight: 600;
}

.stage-node.current .node-indicator {
  border-color: var(--khet-primary);
  box-shadow: 0 0 0 2px rgba(var(--khet-primary-rgb), 0.2);
}

.order-actions {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.btn-advance {
  background: var(--khet-primary);
  color: white;
  border: none;
  padding: 12px;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
}

.btn-hold {
  background: transparent;
  color: #ef4444;
  border: 1px solid #ef4444;
  padding: 8px;
  border-radius: 12px;
  font-weight: 500;
  cursor: pointer;
}

.btn-resume {
  background: #10b981;
  color: white;
  border: none;
  padding: 12px;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
}

.ready-badge {
  text-align: center;
  color: #10b981;
  font-weight: 700;
  font-size: 1rem;
}

.empty-state {
  text-align: center;
  padding: 3rem 0;
  color: var(--khet-text-muted);
}

.empty-state .emoji {
  font-size: 3rem;
  display: block;
  margin-bottom: 1rem;
}
</style>
