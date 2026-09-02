<template>
  <div class="page">
    <header class="page-header">
      <h1 class="title">Installation</h1>
      <router-link to="/production" class="back-link">← Shop</router-link>
    </header>

    <div class="install-hub">
      <div v-if="activeInstallation" class="installation-card">
        <div class="card-header">
          <h2 class="memorial-id">{{ activeInstallation.memorialId }}</h2>
          <span class="status-pill">{{ activeInstallation.status }}</span>
        </div>

        <div class="checklist">
          <h3 class="section-title">Site Readiness Checklist</h3>
          <div v-for="(item, idx) in checklist" :key="idx" class="check-item">
            <input
              type="checkbox"
              v-model="installationProgress[idx]"
              @change="updateProgress"
              :id="'check-' + idx"
            />
            <label :for="'check-' + idx">{{ item }}</label>
          </div>
        </div>

        <div class="photo-section">
          <h3 class="section-title">Completion Photo</h3>
          <div class="upload-box" :class="{ uploaded: photoUploaded }">
            <div v-if="!photoUploaded" class="upload-placeholder">
              <span class="emoji">📸</span>
              <p>Capture final installation photo</p>
              <input type="file" @change="simulatePhotoUpload" accept="image/*" class="file-input" />
            </div>
            <div v-else class="photo-preview">
              <span class="emoji">✅</span>
              <p>Photo Uploaded Successfully</p>
            </div>
          </div>
        </div>

        <div class="signoff-section">
          <h3 class="section-title">Customer Sign-off</h3>
          <div class="signature-pad">
            <div class="signature-canvas">
              <span class="placeholder-text">Digital Signature Area</span>
            </div>
            <button @click="captureSignature" class="btn-sign">Capture Signature</button>
          </div>
        </div>

        <div class="final-actions">
          <button
            @click="finalizeInstallation"
            :disabled="!canFinalize"
            class="btn-complete"
          >
            Mark as Installed
          </button>
        </div>
      </div>

      <div v-else class="empty-state">
        <span class="emoji">📍</span>
        <p>No installations assigned for today.</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

const activeInstallation = ref({
  memorialId: 'MEM-501',
  status: 'Scheduled',
  siteAddress: 'Plot 42, West Park Cemetery',
});

const checklist = [
  'Foundation base leveled and cured',
  'Slab dimensions verified against site',
  'Memorial aligned with adjacent graves',
  'Curbing and borders installed',
  'Site cleared of debris',
];

const installationProgress = ref(new Array(checklist.length).fill(false));
const photoUploaded = ref(false);
const signatureCaptured = ref(false);

const canFinalize = computed(() => {
  const allChecked = installationProgress.value.every(val => val === true);
  return allChecked && photoUploaded.value && signatureCaptured.value;
});

function updateProgress() {
  // Logic to save progress to local DB
}

function simulatePhotoUpload() {
  photoUploaded.value = true;
}

function captureSignature() {
  signatureCaptured.value = true;
  alert('Customer signature captured successfully.');
}

async function finalizeInstallation() {
  try {
    alert('Installation completed and synced to ERP.');
    router.push('/production');
  } catch (e) {
    alert('Failed to finalize installation.');
  }
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
  font-size: 1.6rem;
  font-weight: 700;
  margin: 0;
}

.back-link {
  font-size: 0.85rem;
  color: var(--khet-primary);
  text-decoration: none;
  font-weight: 600;
}

.installation-card {
  background: var(--khet-surface);
  border-radius: 20px;
  padding: 1.5rem;
  border: 1px solid var(--khet-border);
  box-shadow: 0 4px 12px rgba(0,0,0,0.05);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.memorial-id {
  font-size: 1.2rem;
  font-weight: 700;
  margin: 0;
}

.status-pill {
  font-size: 0.75rem;
  padding: 4px 10px;
  border-radius: 12px;
  background: rgba(0,0,0,0.05);
  font-weight: 600;
}

.section-title {
  font-size: 0.9rem;
  text-transform: uppercase;
  color: var(--khet-text-muted);
  margin: 0 0 1rem 0;
  border-bottom: 1px solid var(--khet-border);
  padding-bottom: 4px;
}

.checklist {
  margin-bottom: 2rem;
}

.check-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 0.75rem;
  font-size: 0.9rem;
  cursor: pointer;
}

.check-item input {
  width: 20px;
  height: 20px;
  cursor: pointer;
}

.photo-section {
  margin-bottom: 2rem;
}

.upload-box {
  border: 2px dashed var(--khet-border);
  border-radius: 16px;
  padding: 2rem 1rem;
  text-align: center;
  position: relative;
  transition: all 0.3s;
}

.upload-box.uploaded {
  border-color: #10b981;
  background: rgba(16, 185, 129, 0.05);
}

.upload-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
}

.upload-placeholder .emoji {
  font-size: 2rem;
}

.upload-placeholder p {
  font-size: 0.85rem;
  color: var(--khet-text-muted);
}

.file-input {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  opacity: 0;
  cursor: pointer;
}

.photo-preview {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
}

.photo-preview .emoji {
  font-size: 2rem;
}

.signoff-section {
  margin-bottom: 2rem;
}

.signature-pad {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.signature-canvas {
  height: 120px;
  background: #fff;
  border: 1px solid var(--khet-border);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-style: italic;
  color: var(--khet-text-muted);
  font-size: 0.85rem;
}

.btn-sign {
  background: var(--khet-surface);
  border: 1px solid var(--khet-primary);
  color: var(--khet-primary);
  padding: 10px;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
}

.final-actions {
  margin-top: 2rem;
}

.btn-complete {
  width: 100%;
  background: #10b981;
  color: white;
  border: none;
  padding: 14px;
  border-radius: 12px;
  font-weight: 700;
  font-size: 1rem;
  cursor: pointer;
}

.btn-complete:disabled {
  opacity: 0.5;
  cursor: not-allowed;
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
