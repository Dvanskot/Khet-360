<template>
  <header class="top-bar">
    <div class="left-section">
      <div class="search-trigger" @click="toggleCommandPalette">
        <span class="search-icon">🔍</span>
        <span class="search-text">Search or run command...</span>
        <span class="search-shortcut">Ctrl + K</span>
      </div
    </div>

    <div class="right-section">
      <div class="context-selector">
        <span class="context-label">Branch:</span>
        <div class="context-value">
          <span class="value-text">Cape Town Central</span>
          <span class="chevron">▾</span>
        </div>
      </div>

      <div class="action-icons">
        <div class="icon-btn" title="Notifications">
          <span class="icon">🔔</span>
          <span class="badge">3</span>
        </div>
        <div class="icon-btn" title="User Settings">
          <span class="icon">⚙️</span>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { defineEmits } from 'vue';

const emit = defineEmits(['open-palette']);

const toggleCommandPalette = () => {
  emit('open-palette');
};
</script>

<style scoped>
.top-bar {
  height: 64px;
  background-color: var(--khet-surface);
  border-bottom: 1px solid var(--khet-border);
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 2rem;
  position: sticky;
  top: 0;
  z-index: 90;
}

.left-section {
  flex: 1;
}

.search-trigger {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.5rem 1rem;
  background-color: var(--khet-surface-alt);
  border: 1px solid var(--khet-border);
  border-radius: var(--khet-radius-md);
  width: fit-content;
  max-width: 400px;
  cursor: pointer;
  transition: all 0.2s;
}

.search-trigger:hover {
  border-color: var(--khet-primary);
  background-color: white;
}

.search-icon {
  font-size: 1rem;
  color: var(--khet-text-muted);
}

.search-text {
  font-size: 0.9rem;
  color: var(--khet-text-muted);
}

.search-shortcut {
  font-size: 0.7rem;
  background-color: var(--khet-border);
  padding: 2px 6px;
  border-radius: 4px;
  color: var(--khet-text-muted);
  margin-left: auto;
  padding-left: 1rem;
}

.right-section {
  display: flex;
  align-items: center;
  gap: 2rem;
}

.context-selector {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
  padding: 0.5rem;
  border-radius: var(--khet-radius-md);
  transition: background 0.2s;
}

.context-selector:hover {
  background-color: var(--khet-surface-alt);
}

.context-label {
  font-size: 0.85rem;
  color: var(--khet-text-muted);
}

.context-value {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  font-size: 0.85rem;
  font-weight: 600;
}

.chevron {
  font-size: 0.7rem;
  color: var(--khet-text-muted);
}

.action-icons {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.icon-btn {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  cursor: pointer;
  position: relative;
  transition: background 0.2s;
}

.icon-btn:hover {
  background-color: var(--khet-surface-alt);
}

.icon {
  font-size: 1.2rem;
}

.badge {
  position: absolute;
  top: 0;
  right: 0;
  background-color: red;
  color: white;
  font-size: 0.65rem;
  font-weight: 700;
  width: 16px;
  height: 16px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 2px solid white;
}
</style>
