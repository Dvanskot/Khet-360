<template>
  <div class="page">
    <header class="page-header">
      <h1 class="title">Operations</h1>
      <router-link to="/" class="back-link">← Back to Work</router-link>
    </header>

    <div class="ops-grid">
      <div
        v-for="tool in tools"
        :key="tool.name"
        class="tool-card"
        @click="navigateToTool(tool.path)"
      >
        <div class="tool-icon">{{ tool.icon }}</div>
        <div class="tool-content">
          <h3>{{ tool.name }}</h3>
          <p>{{ tool.description }}</p>
        </div>
        <span class="arrow">→</span>
      </div>
    </div>

    <router-view v-if="currentRoute" />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';

const router = useRouter();
const route = useRoute();

const tools = [
  {
    name: 'Repatriation',
    icon: '✈️',
    description: 'Manage cross-border movements and custody.',
    path: '/ops/repatriation',
  },
  {
    name: 'Mortuary',
    icon: '❄️',
    description: 'Admissions, slot management and releases.',
    path: '/ops/mortuary',
  },
];

const currentRoute = computed(() => route.name && route.name.startsWith('repatriation') || route.name === 'mortuary');

function navigateToTool(path: string) {
  router.push(path);
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

.back-link {
  font-size: 0.85rem;
  color: var(--khet-primary);
  text-decoration: none;
  font-weight: 600;
}

.ops-grid {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.tool-card {
  background: var(--khet-surface);
  border-radius: 16px;
  padding: 1.25rem;
  border: 1px solid var(--khet-border);
  display: flex;
  align-items: center;
  gap: 1.25rem;
  cursor: pointer;
  transition: all 0.2s;
}

.tool-card:active {
  transform: scale(0.98);
  background: var(--khet-bg);
}

.tool-icon {
  font-size: 2rem;
  background: rgba(0,0,0,0.05);
  width: 56px;
  height: 56px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 12px;
}

.tool-content {
  flex: 1;
}

.tool-content h3 {
  font-size: 1.1rem;
  margin: 0 0 0.25rem 0;
  font-weight: 600;
}

.tool-content p {
  font-size: 0.85rem;
  color: var(--khet-text-muted);
  margin: 0;
}

.arrow {
  color: var(--khet-text-muted);
  font-size: 1.2rem;
  font-weight: bold;
}
</style>
