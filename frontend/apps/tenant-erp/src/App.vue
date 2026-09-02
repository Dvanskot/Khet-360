<template>
  <router-view />
</template>

<script setup lang="ts">
</script>

<style>
:root {
  --khet-primary: #08AFAF;
  --khet-primary-light: #e6f6f6;
  --khet-bg: #f8f9fa;
  --khet-surface: #ffffff;
  --khet-surface-alt: #f1f3f5;
  --khet-text-main: #212529;
  --khet-text-muted: #6c757d;
  --khet-border: #dee2e6;
  --khet-radius-md: 8px;
  --khet-font-main: 'Inter', system-ui, -apple-system, sans-serif;
}

body {
  margin: 0;
  padding: 0;
  font-family: var(--khet-font-main);
  background-color: var(--khet-bg);
  color: var(--khet-text-main);
}
</style>
