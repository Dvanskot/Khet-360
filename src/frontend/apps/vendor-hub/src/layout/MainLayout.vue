<template>
  <header class="main-header">
    <div class="header-content">
      <div class="logo">
        <img src="/images/khet360_logo.png" alt="Khet-360 Logo" />
        <span>Khet-360 Vendor Hub</span>
      </div>
      
      <div class="header-tools">
        <div class="connection-status" :class="{ connected: isConnected, disconnected: !isConnected }">
          <span v-if="isConnected">● Online</span>
          <span v-if="!isConnected">○ Offline</span>
        </div>
        
        <NotificationBadge />
        
        <div class="user-profile">
          <span class="username">Admin User</span>
          <img src="/images/user-placeholder.png" alt="User Profile" class="user-avatar" />
        </div>
      </div>
    </div>
  </header>

  <nav class="main-nav">
    <div class="nav-content">
      <router-link to="/dashboard" class="nav-link" exact-active-class="active">
        Dashboard
      </router-link>
      <router-link to="/products" class="nav-link" active-class="active">
        Products
      </router-link>
      <router-link to="/inventory" class="nav-link" active-class="active">
        Inventory
      </router-link>
      <router-link to="/orders" class="nav-link" active-class="active">
        Orders
      </router-link>
      <router-link to="/invoices" class="nav-link" active-class="active">
        Invoices
      </router-link>
    </div>
  </nav>

  <router-view />
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { signalRService } from '@/services/signalRService';
import { NotificationBadge } from '@/components/NotificationBadge.vue';

const isConnected = ref(false);

onMounted(() => {
  // Update connection status
  const updateStatus = () => {
    isConnected.value = signalRService.isConnectedStatus();
  };
  
  // Listen for connection changes
  window.addEventListener('signalr-connection-change', (e: any) => {
    isConnected.value = e.detail.connected;
  });
  
  // Initial check
  updateStatus();
  
  // Start SignalR connection
  signalRService.start().catch(err => {
    console.error('Failed to start SignalR connection:', err);
  });
  
  // Periodic check
  setInterval(updateStatus, 5000);
});

onBeforeUnmount(() => {
  window.removeEventListener('signalr-connection-change', (e: any) => {});
  signalRService.stop();
});
</script>

<script setup lang="ts">
</script>

<style scoped>
.main-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 2rem;
  background-color: white;
  border-bottom: 1px solid #e5e7eb;
}

.header-content {
  display: flex;
  align-items: center;
  gap: 2rem;
}

.logo {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.logo img {
  height: 32px;
}

.logo span {
  font-weight: 600;
  font-size: 1.25rem;
  color: #1f2937;
}

.header-tools {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.connection-status {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
}

.connected {
  color: #10b981;
}

.disconnected {
  color: #ef4444;
}

.user-profile {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.username {
  font-weight: 500;
  color: #374151;
}

.user-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  object-fit: cover;
}

.main-nav {
  background-color: #f8fafc;
  border-bottom: 1px solid #e5e7eb;
}

.nav-content {
  display: flex;
  gap: 2rem;
  padding: 1rem 2rem;
}

.nav-link {
  text-decoration: none;
  font-weight: 500;
  color: #6b7280;
  padding: 0.5rem 0;
  border-bottom: 2px solid transparent;
}

.nav-link:hover {
  color: #374151;
}

.nav-link.active {
  color: #3b82f6;
  border-bottom-color: #3b82f6;
}

.nav-link.active:hover {
  color: #2563eb;
}
</style>