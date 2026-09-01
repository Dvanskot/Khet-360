<template>
  <div class="min-h-screen bg-gray-50 flex">
    <!-- Sidebar -->
    <aside class="w-64 bg-slate-900 text-white flex flex-col">
      <div class="p-6 text-xl font-bold border-b border-slate-800">
        Khet-360
      </div>
      <nav class="flex-1 p-4 space-y-2">
        <router-link to="/work-items" class="block px-4 py-2 rounded-lg hover:bg-slate-800 transition" active-class="bg-slate-800 font-semibold">
          📅 My Work
        </router-link>
        <router-link to="/crm/leads" class="block px-4 py-2 rounded-lg hover:bg-slate-800 transition" active-class="bg-slate-800 font-semibold">
          👥 Lead Pipeline
        </router-link>
      </nav>
      <div class="p-4 border-t border-slate-800">
        <div class="text-sm text-slate-400 mb-2">Logged in as:</div>
        <div class="font-medium text-slate-200">{{ userStore.user?.userName || 'Guest' }}</div>
        <button @click="userStore.logout()" class="mt-4 w-full text-left text-sm text-red-400 hover:text-red-300">
          Logout
        </button>
      </div>
    </aside>

    <!-- Main Content -->
    <main class="flex-1 overflow-auto">
      <router-view />
    </main>
  </div>
</template>

<script setup lang="ts">
import { useUserStore } from '@/stores/user';
const userStore = useUserStore();
</script>

<style>
@import 'tailwindcss/base';
@import 'tailwindcss/components';
@import 'tailwindcss/utilities';
</style>
