<template>
  <div class="mobile-app">
    <div class="app-container">
      <router-view />
    </div>
    <nav class="bottom-nav">
      <router-link to="/" class="nav-item" active-class="active">
        <span class="nav-icon">📅</span>
        <span class="nav-label">Work</span>
      </router-link>
      <router-link to="/ops" class="nav-item" active-class="active">
        <span class="nav-icon">⚰️</span>
        <span class="nav-label">Ops</span>
      </router-link>
      <router-link to="/crm" class="nav-item" active-class="active">
        <span class="nav-icon">👤</span>
        <span class="nav-label">CRM</span>
      </router-link>
      <router-link to="/production" class="nav-item" active-class="active">
        <span class="nav-icon">🔨</span>
        <span class="nav-label">Shop</span>
      </router-link>
      <router-link to="/profile" class="nav-item" active-class="active">
        <span class="nav-icon">⚙️</span>
        <span class="nav-label">More</span>
      </router-link>
    </nav>
  </template>

  <script setup lang="ts">
  </script>

  <style scoped>
  .mobile-app {
    height: 100vh;
    display: flex;
    flex-direction: column;
    background-color: var(--khet-bg);
    font-family: var(--khet-font-main);
    color: var(--khet-text-main);
  }

  .app-container {
    flex: 1;
    overflow-y: auto;
    padding-bottom: 70px;
  }

  .bottom-nav {
    position: fixed;
    bottom: 0;
    left: 0;
    right: 0;
    height: 65px;
    background-color: var(--khet-surface);
    border-top: 1px solid var(--khet-border);
    display: flex;
    justify-content: space-around;
    align-items: center;
    z-index: 100;
    padding-bottom: env(safe-area-inset-bottom);
  }

  .nav-item {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    text-decoration: none;
    color: var(--khet-text-muted);
    font-size: 0.7rem;
    font-weight: 500;
    transition: all 0.2s;
    flex: 1;
  }

  .nav-icon {
    font-size: 1.4rem;
    margin-bottom: 2px;
  }

  .nav-item.active {
    color: var(--khet-primary);
  }

  .nav-item.active .nav-icon {
    transform: scale(1.2);
  }
  </style>
</template>
