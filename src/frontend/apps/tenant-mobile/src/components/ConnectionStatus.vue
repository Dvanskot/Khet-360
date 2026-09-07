<template>
  <div class="connection-status">
    <span v-if="isConnected" class="status-dot status-connected">●</span>
    <span v-else class="status-dot status-disconnected">○</span>
    <span class="status-text">
      {{ isConnected ? 'Connected' : 'Disconnected' }}
    </span>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue';
import { signalRService } from '@/services/signalRService';

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
  
  // Periodic check
  setInterval(updateStatus, 5000);
});

onBeforeUnmount(() => {
  window.removeEventListener('signalr-connection-change', (e: any) => {});
});
</script>

<script setup lang="ts">
</script>

<style scoped>
.connection-status {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
}

.status-dot {
  font-size: 1.25rem;
}

.status-connected {
  color: #10b981;
}

.status-disconnected {
  color: #ef4444;
}

.status-text {
  font-size: 0.875rem;
  font-weight: 500;
}
</style>