<template>
  <div class="main-layout">
    <NavBar />
    <div class="content-area">
      <TopBar @open-palette="showPalette = true" />
      <main class="page-content">
        <router-view />
      </main>
    </div>

    <CommandPalette v-if="showPalette" @close="showPalette = false" />
  </template>

  <script setup lang="ts">
  import { ref, onMounted, onUnmounted } from 'vue';
  import NavBar from './NavBar.vue';
  import TopBar from './TopBar.vue';
  import CommandPalette from './CommandPalette.vue';

  const showPalette = ref(false);

  const handleKeyDown = (e: KeyboardEvent) => {
    if (e.ctrlKey && e.key === 'k') {
      e.preventDefault();
      showPalette.value = !showPalette.value;
    }
    if (e.key === 'Escape' && showPalette.value) {
      showPalette.value = false;
    }
  };

  onMounted(() => {
    window.addEventListener('keydown', handleKeyDown);
  });

  onUnmounted(() => {
    window.removeEventListener('keydown', handleKeyDown);
  });
  </script>

  <style scoped>
  .main-layout {
    display: flex;
    height: 100vh;
    overflow: hidden;
  }

  .content-area {
    flex: 1;
    display: flex;
    flex-direction: column;
    height: 100vh;
    overflow: hidden;
    margin-left: 260px;
  }

  .page-content {
    flex: 1;
    padding: 2rem;
    overflow-y: auto;
    background-color: var(--khet-bg);
  }
  </style>
</template>
