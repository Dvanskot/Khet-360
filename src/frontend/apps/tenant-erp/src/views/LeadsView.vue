<template>
  <div class="page">
    <header class="page-header">
      <h1 class="title">Leads Management</h1>
      <div class="header-actions">
        <ConnectionStatus />
        <KButton @click="refreshLeads" variant="outline" size="sm">
          Refresh
        </KButton>
        <KButton @click="showCreateLeadDialog = true" variant="primary">
          Add Lead
        </KButton>
      </div>
    </header>

    <div v-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-if="loading" class="loading-indicator">
      <div class="spinner"></div>
      <p>Loading leads...</p>
    </div>

    <div class="filters-section">
      <div class="filter-group">
        <label for="search" class="form-label">Search Leads</label>
        <KInput
          v-model="searchTerm"
          placeholder="Search by name, organization, or service interest..."
          class="w-full"
        />
      </div>
      
      <div class="filter-group">
        <label for="status" class="form-label">Status</label>
        <KSelect
          v-model="selectedStatus"
          :options="statusOptions"
          placeholder="All Statuses"
          class="w-full"
        />
      </div>
      
      <div class="filter-group">
        <label for="priority" class="form-label">Priority</label>
        <KSelect
          v-model="selectedPriority"
          :options="priorityOptions"
          placeholder="All Priorities"
          class="w-full"
        />
      </div>
    </div>

    <div v-if="filteredLeads.length === 0 && !loading" class="empty-state">
      <span class="emoji">📋</span>
      <p>No leads found matching your criteria.</p>
      <KButton @click="showCreateLeadDialog = true" variant="secondary">
        Add First Lead
      </KButton>
    </div>

    <div class="leads-table">
      <table class="leads-table">
        <thead>
          <tr>
            <th>Contact Name</th>
            <th>Organization</th>
            <th>Service Interest</th>
            <th>Priority</th>
            <th>Status</th>
            <th>Date Created</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="lead in filteredLeads" :key="lead.id" class="table-row">
            <td>{{ lead.contactName }}</td>
            <td>{{ lead.organization || '-' }}</td>
            <td>{{ lead.serviceInterest }}</td>
            <td>
              <span 
                class="priority-badge" 
                :class="[lead.priority.toLowerCase()]"
              >
                {{ lead.priority }}
              </span>
            </td>
            <td>
              <span 
                class="status-badge" 
                :class="[lead.status.toLowerCase().replace(' ', '-')]"
              >
                {{ lead.status }}
              </span>
            </td>
            <td>{{ formatDate(lead.createdAt) }}</td>
            <td class="actions-cell">
              <KButton 
                variant="secondary" 
                size="sm" 
                @click="editLead(lead)"
                class="mr-2"
              >
                Edit
              </KButton>
              <KButton 
                variant="success" 
                size="sm" 
                @click="convertLead(lead.id)"
              >
                Convert to Case
              </KButton>
              <KButton 
                variant="danger" 
                size="sm" 
                @confirm="deleteLead(lead.id)"
                @confirmed="confirmDelete(lead.id)"
                class="mr-2"
              >
                Delete
              </KButton>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else class="no-results">
      <p>No leads found matching your criteria.</p>
    </div>
  </div>

  <!-- Create/Edit Lead Dialog -->
  <KDialog v-model:show="showCreateLeadDialog" title="Create New Lead" width="500px">
    <div class="dialog-content">
      <div class="form-group">
        <label for="contactName" class="form-label">Contact Name *</label>
        <KInput
          v-model="form.contactName"
          id="contactName"
          placeholder="Enter contact name"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="organization" class="form-label">Organization</label>
        <KInput
          v-model="form.organization"
          id="organization"
          placeholder="Enter organization name"
        />
      </div>
      
      <div class="form-group">
        <label for="serviceInterest" class="form-label">Service Interest *</label>
        <KSelect
          v-model="form.serviceInterest"
          id="serviceInterest"
          :options="serviceOptions"
          placeholder="Select service interest"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="priority" class="form-label">Priority</label>
        <KSelect
          v-model="form.priority"
          id="priority"
          :options="priorityOptions"
          placeholder="Select priority"
          required
        />
      </div>
      
      <div class="form-group">
        <label for="status" class="form-label">Status</label>
        <KSelect
          v-model="form.status"
          id="status"
          :options="statusOptions"
          placeholder="Select status"
          required
        />
      </div>
    </div>
    
    <template #footer>
      <KButton variant="secondary" @click="showCreateLeadDialog = false">
        Cancel
      </KButton>
      <KButton 
        variant="primary" 
        @click="saveLead"
        :loading="saving"
      >
        {{ form.id ? 'Update' : 'Create' }} Lead
      </KButton>
    </template>
  </KDialog>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, computed } from 'vue';
import { KButton, KInput, KSelect, KDialog } from '@khet360/ui-shared';
import { LeadService, Lead } from '@/services/leadService';
import { signalRService } from '@/services/signalRService';
import { authService } from '@/services/authService';
import { notificationService } from '@/services/notificationService';
import { ConnectionStatus } from '@/components/ConnectionStatus.vue';

const leads = ref<Lead[]>([]);
const filteredLeads = ref<Lead[]>([]);
const loading = ref<boolean>(true);
const error = ref<string | null>(null);

const searchTerm = ref('');
const selectedStatus = ref<string | null>(null);
const selectedPriority = ref<string | null>(null);
const showCreateLeadDialog = ref(false);
const saving = ref(false);
const confirmDelete = ref(false);

const form = ref({
  id: '',
  contactName: '',
  organization: '',
  serviceInterest: '',
  priority: 'Medium',
  status: 'New'
});

const statusOptions = ['New', 'Contacted', 'Qualified', 'Proposal Sent', 'Negotiation', 'Closed Won', 'Closed Lost'];
const priorityOptions = ['Low', 'Medium', 'High', 'Critical'];
const serviceOptions = [
  'Traditional Funeral',
  'Cremation Service',
  'Memorial Service',
  'Pre-Planning Consultation',
  'Grief Support',
  'Transportation Services',
  'Floral Arrangements',
  'Catering Services'
];

// Computed property for filtered leads
computed(() => {
  return leads.value.filter(lead => {
    const matchesSearch = lead.contactName.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
                         (lead.organization && lead.organization.toLowerCase().includes(searchTerm.value.toLowerCase())) ||
                         lead.serviceInterest.toLowerCase().includes(searchTerm.value.toLowerCase());
    
    const matchesStatus = !selectedStatus.value || lead.status === selectedStatus.value;
    const matchesPriority = !selectedPriority.value || lead.priority === selectedPriority.value;
    
    return matchesSearch && matchesStatus && matchesPriority;
  });
});

// Real-time update handlers for leads
const handleLeadCreated = (lead: Lead) => {
  leads.value = [...leads.value, lead];
  notificationService.addNotification({
    title: 'New Lead',
    message: `New lead "${lead.contactName}" from ${lead.organization || 'unknown'} has been created`,
    type: 'success'
  });
};

const handleLeadUpdated = (lead: Lead) => {
  const index = leads.value.findIndex(item => item.id === lead.id);
  if (index !== -1) {
    leads.value[index] = { ...leads.value[index], ...lead };
    notificationService.addNotification({
      title: 'Lead Updated',
      message: `Lead "${lead.contactName}" has been updated`,
      type: 'info'
    });
  }
};

const handleLeadDeleted = (leadId: number) => {
  leads.value = leads.value.filter(item => item.id !== leadId);
  notificationService.addNotification({
    title: 'Lead Removed',
    message: `Lead #${leadId} has been removed`,
    type: 'warning'
  });
};

const handleLeadConverted = (data: any) => {
  // Remove the converted lead
  leads.value = leads.value.filter(item => item.id !== data.leadId);
  
  notificationService.addNotification({
    title: 'Lead Converted',
    message: `Lead has been converted to a case`,
    type: 'success'
  });
};

const fetchLeads = async () => {
  try {
    loading.value = true;
    error.value = null;
    leads.value = await LeadService.getLeads();
  } catch (err) {
    error.value = 'Failed to load leads. Please try again later.';
    console.error('Error fetching leads:', err);
    // Fallback to mock data
    leads.value = [
      { id: 1, contactName: 'John Smith', organization: 'Smith Family Funeral Home', serviceInterest: 'Traditional Funeral', priority: 'High', status: 'New', createdAt: 'Today' },
      { id: 2, contactName: 'Mary Johnson', organization: '', serviceInterest: 'Cremation Service', priority: 'Medium', status: 'Contacted', createdAt: 'Yesterday' },
      { id: 3, contactName: 'Robert Williams', organization: 'Williams Memorial Services', serviceInterest: 'Memorial Service', priority: 'Medium', status: 'Qualified', createdAt: '2 days ago' },
      { id: 4, contactName: 'Lisa Davis', organization: 'Davis Family Chapels', serviceInterest: 'Traditional Funeral', priority: 'High', status: 'Proposal Sent', createdAt: '3 days ago' },
    ];
  } finally {
    loading.value = false;
  }
};

const refreshLeads = async () => {
  await fetchLeads();
};

const saveLead = async () => {
  try {
    saving.value = true;
    if (form.value.id) {
      // Update existing lead
      await LeadService.updateLead(form.value.id, form.value);
      // Notify via SignalR that we updated a lead
      signalRService.send('LeadUpdated', form.value);
    } else {
      // Create new lead
      const newLead = await LeadService.createLead(form.value);
      // Notify via SignalR that we created a lead
      signalRService.send('LeadCreated', newLead);
    }
    
    // Close dialog and refresh
    showCreateLeadDialog.value = false;
    await fetchLeads();
  } catch (err) {
    error.value = 'Failed to save lead. Please try again later.';
    console.error('Error saving lead:', err);
  } finally {
    saving.value = false;
  }
};

const editLead = (lead: Lead) => {
  form.value = { ...lead };
  showCreateLeadDialog.value = true;
};

const deleteLead = (leadId: number) => {
  confirmDelete.value = true;
};

const confirmDelete = async (leadId: number) => {
  try {
    await LeadService.deleteLead(leadId);
    // Notify via SignalR that we deleted a lead
    signalRService.send('LeadDeleted', leadId);
    
    await fetchLeads();
  } catch (err) {
    error.value = 'Failed to delete lead. Please try again later.';
    console.error('Error deleting lead:', err);
  }
};

const convertLead = async (leadId: number) => {
  try {
    // In a real implementation, this would open a conversion form
    // For now, we'll simulate the conversion
    const caseData = {
      caseType: 'Funeral',
      serviceDate: new Date().toISOString().split('T')[0],
      deceasedName: 'TBD',
      nextOfKin: form.value.contactName
    };
    
    const result = await LeadService.convertLead(leadId, caseData);
    // Notify via SignalR that we converted a lead
    signalRService.send('LeadConverted', {
      leadId,
      caseId: result.id,
      ...result
    });
    
    await fetchLeads();
  } catch (err) {
    error.value = 'Failed to convert lead. Please try again later.';
    console.error('Error converting lead:', err);
  }
};

const formatDate = (dateStr: string) => {
  if (!dateStr) return 'No date';
  return new Date(dateStr).toLocaleDateString([], { month: 'short', day: 'numeric' });
};

onMounted(() => {
  // Fetch initial data
  fetchLeads();
  
  // Set up SignalR listeners for real-time updates
  const leadCleanup = LeadService.subscribeToLeadUpdates(
    handleLeadCreated,
    handleLeadUpdated,
    handleLeadDeleted,
    handleLeadConverted
  );
  
  // Start SignalR connection if not already started
  signalRService.start().catch(err => {
    console.error('Failed to start SignalR connection:', err);
    notificationService.addNotification({
      title: 'Connection Issue',
      message: 'Unable to connect to real-time updates. Some features may not be live.',
      type: 'warning'
    });
  });
  
  // Cleanup on unmount
  onBeforeUnmount(() => {
    leadCleanup();
    // Note: We don't stop the SignalR connection here as other components might still need it
  });
});
</script>

<script setup lang="ts">
</script>

<style scoped>
.page {
  padding: 1.5rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.page-header h1 {
  font-size: 1.8rem;
  font-weight: 700;
  margin: 0;
}

.page-header .header-actions {
  display: flex;
  gap: 1rem;
}

.error-message {
  padding: 1rem;
  background-color: #f8d7da;
  color: #721c24;
  border-radius: var(--khet-radius-md);
  border: 1px solid #f5c6cb;
}

.loading-indicator {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  color: var(--khet-text-muted);
}

.spinner {
  width: 24px;
  height: 24px;
  border: 2px solid var(--khet-border);
  border-top-color: var(--khet-primary);
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin-bottom: 0.5rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.filters-section {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-label {
  font-weight: 600;
  font-size: 0.875rem;
  margin-bottom: 0.25rem;
}

.empty-state {
  text-align: center;
  padding: 3rem;
  color: var(--khet-text-muted);
}

.empty-state .emoji {
  font-size: 3rem;
  display: block;
  margin-bottom: 1rem;
}

.leads-table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
}

.leads-table th,
.leads-table td {
  padding: 1rem;
  text-align: left;
  border-bottom: 1px solid #f3f4f6;
}

.leads-table th {
  background-color: #f8fafc;
  font-weight: 600;
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.table-row {
  transition: background-color 0.2s;
}

.table-row:hover {
  background-color: #f8fafc;
}

.priority-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
}

.priority-low {
  background-color: #e2e8f0;
  color: #64748b;
}

.priority-medium {
  background-color: #fef3c7;
  color: #d97706;
}

.priority-high {
  background-color: #fed7d7;
  color: #dc2626;
}

.priority-critical {
  background-color: #fecaca;
  color: #991b1b;
}

.status-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
}

.status-new {
  background-color: #dbeafe;
  color: #2563eb;
}

.status-contacted {
  background-color: #dcfce7;
  color: #16a34a;
}

.status-qualified {
  background-color: #fffbeb;
  color: #92400e;
}

.status-proposal-sent {
  background-color: #ffedd5;
  color: #c2410c;
}

.status-negotiation {
  background-color: #e9d5ff;
  color: #5b21b6;
}

.status-closed-won {
  background-color: #dcfce7;
  color: #16a34a;
}

.status-closed-lost {
  background-color: #fecaca;
  color: #991b1b;
}

.actions-cell {
  display: flex;
  gap: 0.5rem;
}

/* Dialog styles */
.dialog-content {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}
</style>