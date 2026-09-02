<template>
  <div class="page">
    <header class="page-header">
      <h1 class="title">CRM & Insurance</h1>
      <div class="header-actions">
        <button @click="view = 'leads'" :class="{ active: view === 'leads' }" class="tab-btn">Leads</button>
        <button @click="view = 'policies'" :class="{ active: view === 'policies' }" class="tab-btn">Policies</button>
        <button @click="view = 'claims'" :class="{ active: view === 'claims' }" class="tab-btn">Claims</button>
      </div>
    </header>

    <div v-if="view === 'leads'" class="crm-section">
      <div class="action-bar">
        <button @click="showLeadModal = true" class="btn-primary">+ Capture Lead</button>
      </div>
      <!-- ... rest of the lead list ... -->

      <div class="lead-list">
        <div v-for="lead in leads" :key="lead.id" class="lead-card">
          <div class="lead-info">
            <h3>{{ lead.name }}</h3>
            <p>{{ lead.phone }}</p>
            <span class="lead-status">{{ lead.status }}</span>
          </div>
          <button @click="enrollLead(lead)" class="btn-enroll">Enroll</button>
        </div>
        <div v-if="leads.length === 0" class="empty-state">
          <span class="emoji">🎯</span>
          <p>No leads captured yet.</p>
        </div>
      </div>
    </div>

    <div v-if="view === 'policies'" class="crm-section">
      <div class="policy-list">
        <div v-for="policy in policies" :key="policy.id" class="policy-card">
          <div class="policy-header">
            <h3>{{ policy.holderName }}</h3>
            <span class="policy-code">{{ policy.code }}</span>
          </div>
          <div class="member-list">
            <div v-for="member in policy.members" :key="member.id" class="member-item">
              <span>{{ member.name }}</span>
              <span class="member-role">{{ member.role }}</span>
            </div>
          </div>
          <div class="policy-actions" style="display: flex; gap: 0.5rem; margin-top: 1rem;">
            <button @click="managePolicy(policy)" class="btn-manage" style="flex: 1;">Members</button>
            <button @click="startClaim(policy)" class="btn-claim" style="flex: 1; background: var(--khet-primary); color: white; border: none; border-radius: 12px; font-weight: 600; cursor: pointer;">Submit Claim</button>
          </div>
        </div>
        <div v-if="policies.length === 0" class="empty-state">
          <span class="emoji">📜</span>
          <p>No enrolled policies found.</p>
        </div>
      </div>
    </div>

    <div v-if="view === 'claims'" class="crm-section">
      <div class="action-bar">
        <button @click="router.push('/crm/claims')" class="btn-primary">+ New Claim Request</button>
      </div>
      <div class="claim-list">
        <div v-for="claim in recentClaims" :key="claim.id" class="lead-card">
          <div class="lead-info">
            <h3>{{ claim.deceasedName }}</h3>
            <p>Claim ID: {{ claim.id }}</p>
            <span class="lead-status" :class="claim.status.toLowerCase()">{{ claim.status }}</span>
          </div>
          <span class="arrow">View →</span>
        </div>
        <div v-if="recentClaims.length === 0" class="empty-state">
          <span class="emoji">📂</span>
          <p>No recent claims found.</p>
        </div>
      </div>
    </div>

    <!-- Lead Modal -->
    <div v-if="showLeadModal" class="modal-overlay">
      <div class="modal">
        <h2>Capture Lead</h2>
        <form @submit.prevent="saveLead">
          <div class="form-group">
            <label>Full Name</label>
            <input v-model="newLead.name" required />
          </div>
          <div class="form-group">
            <label>Phone Number</label>
            <input v-model="newLead.phone" required />
          </div>
          <div class="form-group">
            <label>Notes</label>
            <textarea v-model="newLead.notes"></textarea>
          </div>
          <div class="modal-actions">
            <button type="button" @click="showLeadModal = false" class="btn-secondary">Cancel</button>
            <button type="submit" class="btn-primary">Save Lead</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';

type View = 'leads' | 'policies' | 'claims';
const view = ref<View>('leads');
const router = useRouter();

const leads = ref([
  { id: '1', name: 'John Doe', phone: '082 123 4567', status: 'New', notes: 'Interested in Family Plan' },
  { id: '2', name: 'Jane Smith', phone: '071 987 6543', status: 'Contacted', notes: 'Needs quote for repatriation' },
]);

const policies = ref([
  {
    id: 'p1',
    holderName: 'Robert Mabusa',
    code: 'FAM-GOLD-001',
    members: [
      { id: 'm1', name: 'Robert Mabusa', role: 'Main Member' },
      { id: 'm2', name: 'Sarah Mabusa', role: 'Spouse' },
      { id: 'm3', name: 'Little Robert', role: 'Child' },
    ],
  },
]);

const recentClaims = ref([
  { id: 'CLM-2026-001', deceasedName: 'Sipho Khumalo', status: 'Pending' },
  { id: 'CLM-2026-002', deceasedName: 'Grace Mokoena', status: 'Approved' },
]);

const showLeadModal = ref(false);
const newLead = ref({ name: '', phone: '', notes: '' });

async function saveLead() {
  try {
    // Simulating API call
    const lead = { ...newLead.value, id: Date.now().toString(), status: 'New' };
    leads.value.push(lead);
    showLeadModal.value = false;
    newLead.value = { name: '', phone: '', notes: '' };
  } catch (e) {
    alert('Failed to save lead. Please check connection.');
  }
}

function enrollLead(lead: any) {
  alert(`Redirecting to enrollment flow for ${lead.name}...`);
}

function startClaim(policy: any) {
  router.push('/crm/claims');
}

function managePolicy(policy: any) {
  alert(`Opening member management for ${policy.code}...`);
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
  margin-bottom: 2rem;
}

.title {
  font-size: 1.8rem;
  font-weight: 700;
  margin: 0;
}

.header-actions {
  display: flex;
  gap: 0.5rem;
  background: var(--khet-surface);
  padding: 4px;
  border-radius: 12px;
  border: 1px solid var(--khet-border);
}

.tab-btn {
  background: transparent;
  border: none;
  padding: 6px 12px;
  font-size: 0.8rem;
  font-weight: 600;
  border-radius: 8px;
  cursor: pointer;
  color: var(--khet-text-muted);
}

.tab-btn.active {
  background: white;
  color: var(--khet-primary);
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
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

.lead-list, .policy-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.lead-card, .policy-card {
  background: var(--khet-surface);
  border-radius: 16px;
  padding: 1.25rem;
  border: 1px solid var(--khet-border);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.lead-info h3 {
  font-size: 1.1rem;
  margin: 0 0 0.25rem 0;
  font-weight: 600;
}

.lead-info p {
  font-size: 0.9rem;
  color: var(--khet-text-muted);
  margin: 0 0 0.5rem 0;
}

.lead-status {
  font-size: 0.7rem;
  font-weight: 700;
  text-transform: uppercase;
  color: var(--khet-primary);
  background: rgba(var(--khet-primary-rgb), 0.1);
  padding: 2px 6px;
  border-radius: 4px;
}

.btn-enroll {
  background: var(--khet-primary);
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
}

.policy-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1rem;
}

.policy-header h3 {
  font-size: 1.1rem;
  margin: 0;
  font-weight: 600;
}

.policy-code {
  font-size: 0.75rem;
  font-weight: 700;
  color: var(--khet-text-muted);
  background: rgba(0,0,0,0.05);
  padding: 2px 6px;
  border-radius: 4px;
}

.member-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-bottom: 1.25rem;
}

.member-item {
  display: flex;
  justify-content: space-between;
  font-size: 0.85rem;
  padding: 4px 0;
  border-bottom: 1px solid var(--khet-border);
}

.member-role {
  font-size: 0.75rem;
  color: var(--khet-text-muted);
}

.btn-manage {
  width: 100%;
  background: transparent;
  border: 1px solid var(--khet-primary);
  color: var(--khet-primary);
  padding: 10px;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
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

.form-group input, .form-group textarea {
  padding: 12px;
  border-radius: 8px;
  border: 1px solid var(--khet-border);
  background: var(--khet-bg);
  color: var(--khet-text-main);
}

.form-group textarea {
  height: 80px;
  resize: none;
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
