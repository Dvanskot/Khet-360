import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('@/layout/MainLayout.vue'),
    children: [
      {
        path: '',
        name: 'my-work',
        component: () => import('@/views/MyWorkView.vue'),
      },
      {
        path: 'team-queue',
        name: 'team-queue',
        component: () => import('@/views/TeamQueueView.vue'),
      },
      {
        path: 'exceptions',
        name: 'exceptions',
        component: () => import('@/views/ExceptionsView.vue'),
      },
      {
        path: 'crm',
        name: 'crm',
        component: () => import('@/views/crm/CRMDashboard.vue'),
      },
      {
        path: 'operations',
        name: 'operations',
        component: () => import('@/views/operations/OperationsDashboard.vue'),
      },
      {
        path: 'finance',
        name: 'finance',
        component: () => import('@/views/finance/FinanceDashboard.vue'),
      },
      {
        path: 'hr',
        name: 'hr',
        component: () => import('@/views/hr/HRDashboard.vue'),
      },
    ],
  },
  {
    path: '/login',
    name: 'login',
    component: () => import('@/views/LoginView.vue'),
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/',
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
