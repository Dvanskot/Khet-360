import { createApp } from 'vue';
import { createPinia } from 'pinia';
import { createRouter, createWebHistory } from 'vue-router';
import App from './App.vue';
import router from './router';
import { seedMobileDB } from './db/seed';

const app = createApp(App);
const pinia = createPinia();

app.use(pinia);
app.use(router);

// Seed local database before mounting
seedMobileDB().then(() => {
  app.mount('#app');
});
