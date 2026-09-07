<template>
  <div class="notification-badge" @click="toggleDropdown">
    <span class="bell-icon">🔔</span>
    <span class="badge" v-if="unreadCount > 0">{{ unreadCount }}</span>
    <div v-if="showDropdown" class="notification-dropdown">
      <div class="dropdown-header">
        <h3>Notifications</h3>
        <button class="clear-btn" @click="clearAll">Clear All</button>
      </div>
      <div class="dropdown-content">
        <div v-if="notifications.length === 0" class="empty-state">
          No notifications
        </div>
        <div v-else>
          <div v-for="notification in notifications" :key="notification.id" class="notification-item" :class="{ 'unread': !notification.read }">
            <div class="notification-content">
              <strong>{{ notification.title }}</strong>
              <p>{{ notification.message }}</p>
              <small>{{ formatTimestamp(notification.timestamp) }}</small>
            </div>
            <div class="notification-actions">
              <button v-if="!notification.read" @click="markAsRead(notification.id)" class="mark-read-btn">
                Mark as Read
              </button>
              <button @click="removeNotification(notification.id)" class="remove-btn">
                Remove
              </button>
            </div>
          </div>
        </div>
      </div>
      <div class="dropdown-footer">
        <button @click="showDropdown = false">Close</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue';
import { notificationService } from '@/services/notificationService';
import { signalRService } from '@/services/signalRService';

const showDropdown = ref(false);
const notifications = ref<Array<any>>([]);
const unreadCount = ref(0);

const updateNotifications = () => {
  notifications.value = notificationService.getNotifications();
  unreadCount.value = notificationService.getUnreadCount();
};

const toggleDropdown = () => {
  showDropdown.value = !showDropdown.value;
};

const markAsRead = (id: string) => {
  notificationService.markAsRead(id);
  updateNotifications();
};

const removeNotification = (id: string) => {
  notificationService.removeNotification(id);
  updateNotifications();
};

const clearAll = () => {
  notificationService.clear();
  updateNotifications();
  showDropdown.value = false;
};

const formatTimestamp = (timestamp: Date): string => {
  return new Date(timestamp).toLocaleTimeString([], { hour: '2-digit', minute:'2-digit' });
};

// Initialize
onMounted(() => {
  updateNotifications();
  
  // Listen for notification service updates
  const notificationListener = notificationService.addListener(updateNotifications);
  
  // Also listen for SignalR notifications
  signalRService.on('NotificationReceived', (notification: any) => {
    notificationService.addNotification(notification);
    updateNotifications();
  });
  
  // Cleanup on unmount
  onBeforeUnmount(() => {
    notificationListener();
    signalRService.off('NotificationReceived', (notification: any) => {
      notificationService.addNotification(notification);
      updateNotifications();
    });
  });
});
</script>

<script setup lang="ts">
</script>

<style scoped>
.notification-badge {
  position: relative;
  display: inline-flex;
  align-items: center;
  cursor: pointer;
  margin-left: 1rem;
}

.bell-icon {
  font-size: 1.5rem;
}

.badge {
  position: absolute;
  top: -8px;
  right: -8px;
  background-color: #ef4444;
  color: white;
  border-radius: 50%;
  padding: 2px 6px;
  font-size: 0.75rem;
  font-weight: 600;
}

.notification-dropdown {
  position: absolute;
  top: 120%;
  right: 0;
  width: 320px;
  max-height: 400px;
  background-color: white;
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
  z-index: 1000;
  overflow: hidden;
}

.dropdown-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
}

.dropdown-header h3 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
}

.clear-btn {
  background: none;
  border: none;
  color: #6b7280;
  font-size: 0.875rem;
  cursor: pointer;
}

.clear-btn:hover {
  color: #374151;
}

.dropdown-content {
  overflow-y: auto;
  max-height: 300px;
}

.notification-item {
  padding: 1rem;
  border-bottom: 1px solid #f3f4f6;
}

.notification-item.unread {
  background-color: #f0f9ff;
  border-left: 3px solid #3b82f6;
}

.notification-content {
  flex: 1;
}

.notification-actions {
  display: flex;
  gap: 0.5rem;
  margin-top: 0.5rem;
}

.mark-read-btn,
.remove-btn {
  padding: 0.25rem 0.5rem;
  font-size: 0.75rem;
  border-radius: 0.25rem;
  border: 1px solid #d1d5db;
  background-color: white;
  cursor: pointer;
}

.mark-read-btn:hover {
  background-color: #f3f4f6;
}

.remove-btn:hover {
  background-color: #fee2e2;
}

.dropdown-footer {
  padding: 1rem;
  text-align: right;
  border-top: 1px solid #e5e7eb;
}

.dropdown-footer button {
  background-color: #3b82f6;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 0.25rem;
  font-size: 0.875rem;
  cursor: pointer;
}

.dropdown-footer button:hover {
  background-color: #2563eb;
}

.empty-state {
  text-align: center;
  padding: 2rem;
  color: #6b7280;
  font-size: 0.875rem;
}
</style>