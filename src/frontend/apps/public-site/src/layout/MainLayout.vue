<template>
  <header class="site-header">
    <div class="container header-content">
      <div class="logo">
        <router-link to="/">
          <img src="/images/khet360_logo.png" alt="Khet-360 Funeral Services" />
          <span>Khet-360 Funeral Services</span>
        </router-link>
      </div>
      
      <nav class="main-nav">
        <ul class="nav-list">
          <li><router-link to="/" exact-active-class="active">Home</router-link></li>
          <li><router-link to="/about" active-class="active">About Us</router-link></li>
          <li><router-link to="/services" active-class="active">Services</router-link></li>
          <li><router-link to="/pricing" active-class="active">Pricing</router-link></li>
          <li><router-link to="/resources" active-class="active">Resources</router-link></li>
        <li><router-link to="/analytics" active-class="active">Analytics</router-link></li>
          <li><router-link to="/login" active-class="active">Login</router-link></li>
        </ul>
      </nav>
      
      <div class="header-tools">
        <div class="connection-status" :class="{ connected: isConnected, disconnected: !isConnected }">
          <span v-if="isConnected">● Online</span>
          <span v-if="!isConnected">○ Offline</span>
        </div>
        
        <NotificationBadge />
      </div>
    </div>
  </header>

  <main class="site-main">
    <router-view />
  </main>

  <footer class="site-footer">
    <div class="container footer-content">
      <div class="footer-section">
        <h4>Khet-360 Funeral Services</h4>
        <p>Compassionate care when you need it most.</p>
      </div>
      
      <div class="footer-section">
        <h4>Quick Links</h4>
        <ul>
          <li><router-link to="/">Home</router-link></li>
          <li><router-link to="/services">Services</router-link></li>
          <li><router-link to="/pricing">Pricing</router-link></li>
          <li><router-link to="/contact">Contact</router-link></li>
        </ul>
      </div>
      
      <div class="footer-section">
        <h4>Contact Us</h4>
        <p>Phone: +27 11 123 4567</p>
        <p>Email: info@khet360.co.za</p>
        <p>Address: 123 Funeral Street, Johannesburg, 2000</p>
      </div>
    </div>
    
    <div class="footer-bottom">
      <p>&copy; {{ new Date().getFullYear() }} Khet-360 Funeral Services. All rights reserved.</p>
    </div>
  </footer>
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
.site-header {
  background-color: white;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  position: sticky;
  top: 0;
  z-index: 1000;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1.5rem;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 0;
}

.logo {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.logo img {
  height: 40px;
}

.logo span {
  font-weight: 700;
  font-size: 1.5rem;
  color: #2c3e50;
}

.main-nav {
  flex-grow: 1;
}

.nav-list {
  display: flex;
  list-style: none;
  margin: 0;
  padding: 0;
  gap: 2rem;
}

.nav-list li {
  position: relative;
}

.nav-list a {
  text-decoration: none;
  font-weight: 500;
  color: #34495e;
  padding: 0.5rem 0;
  transition: color 0.3s;
}

.nav-list a:hover {
  color: #3498db;
}

.nav-list a.active {
  color: #3498db;
  font-weight: 600;
}

.nav-list a.active::after {
  content: '';
  position: absolute;
  bottom: -5px;
  left: 0;
  width: 100%;
  height: 2px;
  background-color: #3498db;
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

.status-dot {
  font-size: 1.25rem;
}

.status-connected {
  color: #27ae60;
}

.status-disconnected {
  color: #e74c3c;
}

.status-text {
  font-size: 0.875rem;
  font-weight: 500;
}

.site-main {
  min-height: calc(100vh - 200px);
  padding: 3rem 0;
}

.site-footer {
  background-color: #2c3e50;
  color: #ecf0f1;
  padding: 3rem 0;
}

.footer-content {
  display: flex;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 2rem;
}

.footer-section h4 {
  color: #ecf0f1;
  margin-bottom: 1rem;
  font-size: 1.1rem;
}

.footer-section ul {
  list-style: none;
  padding: 0;
}

.footer-section li {
  margin-bottom: 0.5rem;
}

.footer-section a {
  color: #bdc3c7;
  text-decoration: none;
  transition: color 0.3s;
}

.footer-section a:hover {
  color: #ecf0f1;
}

.footer-section p {
  margin-bottom: 0.5rem;
  line-height: 1.6;
}

.footer-bottom {
  text-align: center;
  padding-top: 2rem;
  border-top: 1px solid #34495e;
  margin-top: 2rem;
  font-size: 0.875rem;
  color: #95a5a6;
}
</style>