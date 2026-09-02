<template>
  <div class="page">
    <header class="page-header">
      <h1 class="title">Mortuary</h1>
      <router-link to="/ops" class="back-link">← Hub</router-link>
    </header>

    <div class="action-bar">
      <button @click="showAddModal = true" class="btn-primary">
        + Admit Case
      </button>
    </div>

    <div class="custody-list">
      <div v-if="records.length === 0" class="empty-state">
        <span class="emoji">❄️</span>
        <p>No current custody records.</p>
      </div>

      <div
        v-for="record in records"
        :key="record.id"
        class="custody-card"
      >
        <div class="card-header">
          <span class="case-id">{{ record.caseId }}</span>
          <span class="status-pill" :class="record.status.toLowerCase()">
            {{ record.status }}
          </span>
        </div>

        <div class="card-body">
          <div class="info-row">
            <span class="label">Location:</span>
            <span class="value">{{ record.location }}</span>
          </div>
          <div class="info-row">
            <span class="label">Slot:</span>
            <span class="value">{{ record.slotId || 'Not Assigned' }}</span>
          </div>
        </div>

        <div class="card-actions">
          <button
            v-if="record.status !== 'Released'"
            @click="releaseCase(record)"
            class="btn-action"
          >
            Confirm Release
          </button>
          <span v-else class="released-badge">Released from Facility</span>
        </div>
      </div>
    </div>

    <!-- Admit Modal -->
    <div v-if="showAddModal" class="modal-overlay">
      <div class="modal">
        <h2>Admit Case</h2>
        <form @submit.prevent="admitCase">
          <div class="form-group">
            <label>Case ID</label>
            <input v-model="newCase.caseId" placeholder="e.g. CASE-123" required />
          </div>
          <div class="form-group">
            <label>Location</label>
            <input v-model="newCase.location" placeholder="e.g. Main Cold Room" required />
          </div>
          <div class="form-group">
            <label>Slot ID (Optional)</label>
            <input v-model="newCase.slotId" placeholder="e.g. A-12" />
          </div>
          <div class="modal-actions">
            <button type="button" @click="showAddModal = false" class="btn-secondary">Cancel</button>
            <button type="submit" class="btn-primary">Admit</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { db } from '@/db/schema';
import { syncEngine } from '@/sync/sync-engine';
import { LocalCustody } from '@/db/schema';

const records = ref<LocalCustody[]>([]);
const showAddModal = ref(false);
const newCase = ref({
  caseId: '',
  location: '',
  slotId: '',
});

async function loadRecords() {
  records.value = await db.custody.where('status').notEqual('Released').toArray();
}

async function admitCase() {
  const id = crypto.randomUUID();
  await syncEngine.executeCommand({
    entityType: 'Custody',
    entityId: id,
    action: 'CREATE',
    payload: {
      id,
      ...newCase.value,
      status: 'Admitted',
      updatedAt: new Date().toISOString(),
    },
  });

  showAddModal.value = false;
  newCase.value = { caseId: '', location: '', slotId: '' };
  await loadRecords();
}

async function releaseCase(record: LocalCustody) {
  await syncEngine.executeCommand({
    entityType: 'Custody',
    entityId: record.id,
    action: 'UPDATE',
    payload: { status: 'Released' },
  });
  await loadRecords();
}

onMounted(async () => {
  await loadRecords();
  setInterval(loadRecords, 5000);
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
  margin-bottom: 1.5rem;
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

.action-bar {
  margin-bottom: 2rem;
}

.btn-primary {
  background: var(--khet-primary);
  color: white;
  border: none;
  padding: 12px 20px;
  border-radius: 12px;
  font-weight: 600;
  width: 100%;
  cursor: pointer;
}

.custody-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.custody-card {
  background: var(--khet-surface);
  border-radius: 16px;
  padding: 1.25rem;
  border: 1px solid var(--khet-border);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.case-id {
  font-weight: 700;
  font-size: 1.1rem;
}

.status-pill {
  font-size: 0.7rem;
  padding: 4px 8px;
  border-radius: 6px;
  font-weight: 700;
  text-transform: uppercase;
  color: white;
}

.status-pill.admitted { background: #3b82f6; }
.status-pill.moved { background: #f59e0b; }
.status-pill.released { background: #10b981; }

.card-body {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-bottom: 1.25rem;
}

.info-row {
  display: flex;
  font-size: 0.9rem;
}

.label {
  color: var(--khet-text-muted);
  width: 80px;
  font-weight: 500;
}

.value {
  font-weight: 600;
}

.card-actions {
  display: flex;
  justify-content: center;
}

.btn-action {
  background: var(--khet-primary);
  color: white;
  border: none;
  padding: 10px 16px;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
}

.released-badge {
  font-size: 0.8rem;
  color: #10b981;
  font-weight: 600;
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

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
}

.modal {
  background: var(--khet-surface);
  padding: 1.5rem;
  border-radius: 20px;
  width: 100%;
  max-width: 400px;
  border: 1px solid var(--khet-border);
}

.modal h2 {
  margin: 0 0 1.5rem 0;
  font-size: 1.4rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.form-group label {
  font-size: 0.85rem;
  font-weight: 600;
  color: var(--khet-text-muted);
}

.form-group input {
  padding: 12px;
  border-radius: 8px;
  border: 1px solid var(--khet-border);
  background: var(--khet-bg);
  color: var(--khet-text-main);
}

.modal-actions {
  display: flex;
  gap: 1rem;
  margin-top: 1.5rem;
}

.btn-secondary {
  flex: 1;
  background: transparent;
  border: 1px solid var(--khet-border);
  padding: 12px;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
}
</style>
