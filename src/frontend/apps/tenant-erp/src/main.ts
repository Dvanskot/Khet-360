import { createApp } from 'vue';
import { createPinia } from 'pinia';
import { createRouter, createWebHistory } from 'vue-router';
import App from './App.vue';
import router from './router';
import { authService } from '@/services/authService';

// Check for existing token on app load
const token = localStorage.getItem('access_token');
if (token) {
  // Token exists, we could validate it here or let the backend handle validation
  console.log('Found existing token in localStorage');
}

const app = createApp(App);
const pinia = createPinia();

app.use(pinia);
app.use(router);
app.mount('#app');