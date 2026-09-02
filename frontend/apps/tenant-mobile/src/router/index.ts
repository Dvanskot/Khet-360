import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'my-work',
    component: () => import('@/views/MyWorkView.vue'),
  },
  {
    path: '/ops',
    name: 'ops-hub',
    component: () => import('@/views/ops/OpsHubView.vue'),
    children: [
      {
        path: 'repatriation',
        name: 'repatriation',
        component: () => import('@/views/ops/RepatriationView.vue'),
      },
      {
        path: 'mortuary',
        name: 'mortuary',
        component: () => import('@/views/ops/MortuaryView.vue'),
      },
    ]
  },
  {
    path: '/crm',
    name: 'crm',
    component: () => import('@/views/crm/MobileCRMView.vue'),
    children: [
      {
        path: 'enroll',
        name: 'enrollment',
        component: () => import('@/views/crm/PolicyEnrollmentView.vue'),
      },
      {
        path: 'claims',
        name: 'claims',
        component: () => import('@/views/crm/ClaimSubmissionView.vue'),
      },
    ]
  },
  {
    path: '/production',
    name: 'production',
    component: () => import('@/views/production/ProductionView.vue'),
  },
  {
    path: '/install',
    name: 'installation',
    component: () => import('@/views/production/InstallationView.vue'),
  },
  {
    path: '/profile',
    name: 'profile',
    component: () => import('@/views/ProfileView.vue'),
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
