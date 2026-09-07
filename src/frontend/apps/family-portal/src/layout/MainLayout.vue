<template>
  <header class="portal-header">
    <div class="header-content">
      <div class="welcome" v-if="!loading && !error">
        <h1>In Loving Memory of {{ deceasedName }}</h1>
        <p>We are here to support you. Below is the current progress of the arrangements.</p>
      </div>
      <div v-else-if="loading" class="loading-indicator">
        <div class="spinner"></div>
        <p>Loading case information...</p>
      </div>
      <div v-else-if="error" class="error-message">
        {{ error }}
      </div>
      <div class="connection-status" :class="{ connected: isConnected, disconnected: !isConnected }">
        <span v-if="isConnected">● Online</span>
        <span v-if="!isConnected">○ Offline</span>
      </div>
      
      <NotificationBadge />
      
      <div class="family-badge">
        <span>Family Access Portal</span>
      </div>
      <div class="user-profile">
        <span class="username">Sarah Jenkins</span>
        <img src="/images/user-placeholder.png" alt="User Profile" class="user-avatar" />
      </div>
    </div>
  </header>

  <main v-if="!loading && !error" class="portal-main">
    <router-view />
  </main>

  <footer class="portal-footer" v-if="!loading && !error">
    <p>Need help? Contact your Funeral Director, {{ assignedFuneralDirector.name }}, at {{ assignedFuneralDirector.contactNumber }}</p>
  </footer>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { FamilyPortalService, FuneralCaseTimeline } from '@/services/familyPortalService';
import { signalRService } from '@/services/signalRService';
import { authService } from '@/services/authService';
import { ConnectionStatus } from '@/components/ConnectionStatus.vue';
import { NotificationBadge } from '@/components/NotificationBadge.vue';

const deceasedName = ref('');
const currentStage = ref('');
const currentStageIndex = ref(0);
const loading = ref<boolean>(true);
const error = ref<string | null>(null);

const milestones = ref<Array<{ title: string; description: string; date?: string }>>([]);
const requiredDocs = ref<Array<{ name: string; uploaded: boolean; uploadDate?: string }>>([]);
const financialSummary = ref<{
  totalPackageCost: number;
  insuranceCover: number;
  outstandingBalance: number;
  currency: string;
}>({
  totalPackageCost: 0,
  insuranceCover: 0,
  outstandingBalance: 0,
  currency: 'ZAR',
});

const assignedFuneralDirector = ref<{
  name: string;
  contactNumber: string;
}>({
  name: '',
  contactNumber: '',
});

const fetchCaseTimeline = async () => {
  try {
    loading.value = true;
    error.value = null;
    // In a real app, we'd get the case ID from the route or auth context
    const caseId = 'C-1024'; // Hardcoded for demo
    const timelineData = await FamilyPortalService.getCaseTimeline(caseId);
    
    deceasedName.value = timelineData.deceasedName;
    currentStage.value = timelineData.currentStage;
    currentStageIndex.value = timelineData.currentStageIndex;
    milestones.value = timelineData.milestones;
    requiredDocs.value = timelineData.requiredDocuments;
    financialSummary.value = timelineData.financialSummary;
    assignedFuneralDirector.value = timelineData.assignedFuneralDirector;
  } catch (err) {
    error.value = 'Failed to load case information. Please try again later.';
    console.error('Error fetching case timeline:', err);
    // Fallback to mock data in case of API failure
    deceasedName.value = 'Samuel Tshikota';
    currentStage.value = 'Service Planning';
    currentStageIndex.value = 2;
    milestones.value = [
      { title: 'Death Notification', description: 'Case opened and initial notification received.', date: 'Aug 25' },
      { title: 'Verification', description: 'Identity and policy verification completed.', date: 'Aug 26' },
      { title: 'Service Planning', description: 'Arranging venue, transport and casket selection.', date: 'Current' },
      { title: 'Service Delivery', description: 'The funeral service and burial/cremation.', date: 'Pending' },
      { title: 'Case Closure', description: 'Final administration and document archiving.', date: 'Pending' },
    ];
    requiredDocs.value = [
      { name: 'Death Certificate', uploaded: true },
      { name: 'ID Copy (Deceased)', uploaded: true },
      { name: 'ID Copy (Next of Kin)', uploaded: false },
      { name: 'Marriage Certificate', uploaded: false },
    ];
    financialSummary.value = {
      totalPackageCost: 12000,
      insuranceCover: 8000,
      outstandingBalance: 4000,
      currency: 'ZAR',
    };
    assignedFuneralDirector.value = {
      name: 'Sarah Jenkins',
      contactNumber: '+27 82 123 4567',
    };
  } finally {
    loading.value = false;
  }
};

const uploadDoc = (name: string) => {
  // In a real implementation, this would open a file upload dialog
  alert(`Opening upload dialog for ${name}...`);
};

const makePayment = () => {
  // In a real implementation, this would redirect to a payment gateway
  alert('Redirecting to secure payment gateway...');
};

onMounted(() => {
  // Fetch initial case data
  fetchCaseTimeline();
  
  // Set up SignalR listeners for real-time updates
  signalRService.on('CaseTimelineUpdated', (updatedTimeline: any) => {
    // Update the case timeline with new data
    deceasedName.value = updatedTimeline.deceasedName;
    currentStage.value = updatedTimeline.currentStage;
    currentStageIndex.value = updatedTimeline.currentStageIndex;
    milestones.value = updatedTimeline.milestones;
    requiredDocs.value = updatedTimeline.requiredDocuments;
    financialSummary.value = updatedTimeline.financialSummary;
    assignedFuneralDirector.value = updatedTimeline.assignedFuneralDirector;
    
    notificationService.addNotification({
      title: 'Case Timeline Updated',
      message: 'The case timeline has been updated with new information',
      type: 'info'
    });
  });
  
  // Start SignalR connection
  signalRService.start().catch(err => {
    console.error('Failed to start SignalR connection:', err);
    notificationService.addNotification({
      title: 'Connection Issue',
      message: 'Unable to connect to real-time updates. Some features may not be live.',
      type: 'warning'
    });
  });
});
</script>

<script setup lang="ts">
</script>

<style scoped>
.portal-container {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.portal-header {
  background-color: var(--family-surface);
  padding: 3rem 2rem;
  text-align: center;
  border-bottom: 1px solid var(--family-border);
}

.header-content {
  max-width: 800px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.5rem;
}

.welcome h1 {
  font-size: 2.5rem;
  margin: 0 0 1rem 0;
  color: var(--family-text-main);
  font-weight: 300;
}

.welcome p {
  font-size: 1.1rem;
  color: var(--family-text-muted);
  max-width: 600px;
  margin: 0 auto;
}

.family-badge {
  background-color: var(--family-primary-light, #f3f4f6);
  color: var(--family-text-muted);
  padding: 6px 16px;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 600;
  border: 1px solid var(--family-border);
}

.connection-status {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
}

.status-dot {
  font-size: 1.25rem;
}

.status-connected {
  color: #10b981;
}

.status-disconnected {
  color: #ef4444;
}

.status-text {
  font-size: 0.875rem;
  font-weight: 500;
}

.portal-main {
  flex: 1;
  padding: 3rem 2rem;
  max-width: 1100px;
  margin: 0 auto;
  width: 100%;
}

.portal-footer {
  text-align: center;
  padding: 3rem 2rem;
  color: var(--family-text-muted);
  font-size: 0.9rem;
  border-top: 1px solid var(--family-border);
}
</style>