<template>
  <div class="p-6">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800">Work Queue</h1>
      <div class="flex space-x-2">
        <button
          @click="tab = 'mine'"
          :class="['px-4 py-2 rounded-lg transition', tab === 'mine' ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-700 hover:bg-gray-300']"
        >
          My Work
        </button>
        <button
          @click="tab = 'team'"
          :class="['px-4 py-2 rounded-lg transition', tab === 'team' ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-700 hover:bg-gray-300']"
        >
          Team Queue
        </button>
      </div>
    </div>

    <div class="bg-white shadow-md rounded-xl overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Source</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Action</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Priority</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Due Date</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">SLA</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="item in workItems" :key="item.id" class="hover:bg-gray-50 transition">
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
              <span class="px-2 py-1 text-xs rounded bg-gray-100 text-gray-600 mr-2">{{ item.sourceEntityType }}</span>
              {{ item.sourceEntityId.substring(0, 8) }}...
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
              {{ item.nextAction }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm">
              <span :class="priorityClass(item.priority)" class="px-2 py-1 rounded-full text-xs font-semibold">
                {{ priorityLabel(item.priority) }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
              {{ formatDate(item.dueDate) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm">
              <span :class="slaClass(item.slaStatus)" class="flex items-center">
                <span class="h-2 w-2 rounded-full mr-2"></span>
                {{ slaLabel(item.slaStatus) }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <button @click="claimItem(item)" v-if="tab === 'team'" class="text-blue-600 hover:text-blue-900 mr-3">Claim</button>
              <button @click="completeItem(item)" class="text-green-600 hover:text-green-900">Complete</button>
            </td>
          </tr>
          <tr v-if="workItems.length === 0">
            <td colspan="6" class="px-6 py-10 text-center text-gray-500 italic">
              No pending work items found.
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import apiClient from '@/api/client';
import { useUserStore } from '@/stores/user';
import type { WorkItem, WorkItemPriority, SlaStatus } from '@/types/workitem';

const userStore = useUserStore();
const tab = ref<'mine' | 'team'>('mine');
const workItems = ref<WorkItem[]>([]);

async function fetchWork() {
  const branchId = userStore.user?.branchId || '';
  const endpoint = tab.value === 'mine'
    ? `/work-items/my-work?userId=${userStore.user?.userId}&branchId=${branchId}`
    : `/work-items/team-queue?branchId=${branchId}`;

  try {
    const response = await apiClient.get(endpoint);
    workItems.value = response.data.items;
  } catch (error) {
    console.error('Failed to fetch work items:', error);
  }
}

async function claimItem(item: WorkItem) {
  try {
    await apiClient.post(`/work-items/${item.id}/assign`, { userId: userStore.user?.userId });
    await fetchWork();
  } catch (error) {
    alert('Failed to claim item');
  }
}

async function completeItem(item: WorkItem) {
  const outcome = prompt('Enter completion outcome:');
  if (!outcome) return;

  try {
    await apiClient.post(`/work-items/${item.id}/complete`, { outcome });
    await fetchWork();
  } catch (error) {
    alert('Failed to complete item');
  }
}

function priorityLabel(p: WorkItemPriority) {
  return ['Low', 'Medium', 'High', 'Critical'][p];
}

function priorityClass(p: WorkItemPriority) {
  return [
    'bg-gray-100 text-gray-600',
    'bg-blue-100 text-blue-600',
    'bg-orange-100 text-orange-600',
    'bg-red-100 text-red-600',
  ][p];
}

function slaLabel(s: SlaStatus) {
  return ['On Track', 'Warning', 'Breached'][s];
}

function slaClass(s: SlaStatus) {
  return [
    'text-green-600',
    'text-yellow-600',
    'text-red-600',
  ][s];
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString();
}

onMounted(fetchWork);
watch(tab, fetchWork);
</script>
