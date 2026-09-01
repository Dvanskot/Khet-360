import { createRouter, createWebHistory } from 'vue-router';
import WorkItemsView from '@/views/WorkItemsView.vue';
import LeadsView from '@/views/LeadsView.vue';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: '/work-items',
    },
    {
      path: '/work-items',
      name: 'WorkItems',
      component: WorkItemsView,
    },
    {
      path: '/crm/leads',
      name: 'Leads',
      component: LeadsView,
    },
  ],
});

export default router;
