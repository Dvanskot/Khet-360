<template>
  <div class="p-6">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800">Lead Pipeline</h1>
      <button @click="showCreateModal = true" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700">
        + New Lead
      </button>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 gap-6">
      <div v-for="stage in stages" :key="stage.value" class="bg-gray-100 p-4 rounded-xl min-h-[70vh]">
        <h2 class="text-lg font-semibold mb-4 text-gray-700 flex justify-between">
          {{ stage.label }}
          <span class="text-sm bg-gray-300 px-2 py-1 rounded-full">{{ getLeadsByStage(stage.value).length }}</span>
        </h2>
        <div class="space-y-4">
          <div v-for="lead in getLeadsByStage(stage.value)" :key="lead.id"
               class="bg-white p-4 rounded-lg shadow-sm border border-gray-200 cursor-pointer hover:border-blue-400 transition"
               @click="selectLead(lead)">
            <div class="font-medium text-gray-900">{{ lead.firstName }} {{ lead.lastName }}</div>
            <div class="text-xs text-gray-500 mb-2">{{ lead.companyName || 'Individual' }}</div>
            <div class="flex justify-between items-center">
              <span class="text-[10px] text-gray-400">{{ formatDate(lead.createdAt) }}</span>
              <button @click.stop="convertLead(lead)" class="text-xs text-blue-600 font-semibold hover:underline">Convert</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Lead Detail Modal -->
    <div v-if="selectedLead" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl max-w-2xl w-full p-6 max-h-[90vh] overflow-y-auto">
        <div class="flex justify-between items-center mb-6">
          <h2 class="text-xl font-bold">Lead Details</h2>
          <button @click="selectedLead = null" class="text-gray-500 hover:text-gray-700">&times;</button>
        </div>
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-xs text-gray-500 uppercase">Name</label>
            <p class="font-medium">{{ selectedLead.firstName }} {{ selectedLead.lastName }}</p>
          </div>
          <div>
            <label class="block text-xs text-gray-500 uppercase">Email</label>
            <p class="font-medium">{{ selectedLead.email }}</p>
          </div>
          <div>
            <label class="block text-xs text-gray-500 uppercase">Phone</label>
            <p class="font-medium">{{ selectedLead.phone }}</p>
          </div>
          <div>
            <label class="block text-xs text-gray-500 uppercase">Source</label>
            <p class="font-medium">{{ selectedLead.source }}</p>
          </div>
          <div class="col-span-2">
            <label class="block text-xs text-gray-500 uppercase">Notes</label>
            <p class="text-gray-600">{{ selectedLead.notes }}</p>
          </div>
        </div>
        <div class="mt-8 flex justify-end space-x-3">
          <button @click="selectedLead = null" class="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded-lg">Close</button>
          <button @click="convertLead(selectedLead)" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">Convert to Customer</button>
        </div>
      </div>
    </div>

    <!-- Create Lead Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-xl max-w-md w-full p-6">
        <h2 class="text-xl font-bold mb-6">Create New Lead</h2>
        <form @submit.prevent="submitLead" class="space-y-4">
          <div class="grid grid-cols-2 gap-4">
            <input v-model="newLead.firstName" placeholder="First Name" class="border p-2 rounded-lg" required />
            <input v-model="newLead.lastName" placeholder="Last Name" class="border p-2 rounded-lg" required />
          </div>
          <input v-model="newLead.email" type="email" placeholder="Email" class="border p-2 rounded-lg w-full" />
          <input v-model="newLead.phone" placeholder="Phone" class="border p-2 rounded-lg w-full" />
          <input v-model="newLead.companyName" placeholder="Company (Optional)" class="border p-2 rounded-lg w-full" />
          <input v-model="newLead.source" placeholder="Source (e.g. Web, Referral)" class="border p-2 rounded-lg w-full" />
          <textarea v-model="newLead.notes" placeholder="Notes" class="border p-2 rounded-lg w-full"></textarea>
          <div class="flex justify-end space-x-3 pt-4">
            <button type="button" @click="showCreateModal = false" class="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded-lg">Cancel</button>
            <button type="submit" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">Create Lead</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import apiClient from '@/api/client';
import { useUserStore } from '@/stores/user';
import type { LeadDto } from '@/types/lead';

const userStore = useUserStore();
const leads = ref<LeadDto[]>([]);
const selectedLead = ref<LeadDto | null>(null);
const showCreateModal = ref(false);
const newLead = ref({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  companyName: '',
  source: '',
  notes: '',
});

const stages = [
  { label: 'New', value: 'New' },
  { label: 'Contacted', value: 'Contacted' },
  { label: 'Qualified', value: 'Qualified' },
  { label: 'Converted', value: 'Converted' },
];

async function fetchLeads() {
  const branchId = userStore.user?.branchId || '';
  try {
    const response = await apiClient.get(`/leads?branchId=${branchId}&pageSize=100`);
    leads.value = response.data.items;
  } catch (error) {
    console.error('Failed to fetch leads:', error);
  }
}

function getLeadsByStage(stage: string) {
  return leads.value.filter(l => l.status === stage);
}

function selectLead(lead: LeadDto) {
  selectedLead.value = lead;
}

async function submitLead() {
  const branchId = userStore.user?.branchId || '';
  try {
    await apiClient.post(`/leads?branchId=${branchId}`, newLead.value);
    showCreateModal.value = false;
    await fetchWork(); // Simplified call for now
    fetchLeads();
  } catch (error) {
    alert('Failed to create lead');
  }
}

async function convertLead(lead: LeadDto) {
  const confirmation = confirm('Convert this lead to a customer and create an opportunity?');
  if (!confirmation) return;

  try {
    await apiClient.post(`/leads/${lead.id}/convert`, {
      createCustomer: true,
      createOpportunity: true,
      customerType: 'Individual',
      opportunityName: `Opp from ${lead.firstName} ${lead.lastName}`,
    });
    selectedLead.value = null;
    fetchLeads();
  } catch (error) {
    alert('Failed to convert lead');
  }
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString();
}

onMounted(fetchLeads);
</script>
