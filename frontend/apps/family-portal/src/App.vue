<template>
  <div class="family-portal">
    <CaseTimelineView />
  </template>
</template>

<script setup lang="ts">
import CaseTimelineView from './views/CaseTimelineView.vue';
</script>

<style>
:root {
  --family-primary: #6b7280;
  --family-bg: #fdfbfb;
  --family-surface: #ffffff;
  --family-text-main: #374151;
  --family-text-muted: #6b7280;
  --family-border: #e5e7eb;
  --family-accent: #08AFAF;
  --family-radius: 16px;
  --family-font: 'Inter', system-ui, -apple-system, sans-serif;
}

body {
  margin: 0;
  padding: 0;
  font-family: var(--family-font);
  background-color: var(--family-bg);
  color: var(--family-text-main);
}
</style>
