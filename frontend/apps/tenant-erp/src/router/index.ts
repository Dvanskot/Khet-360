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
        children: [
          {
            path: 'customer/:id',
            name: 'customer-detail',
            component: () => import('@/views/crm/Customer360View.vue'),
          },
          {
            path: 'leads',
            name: 'leads',
            component: () => import('@/views/crm/LeadsView.vue'),
          },
        ]
      },

      {
        path: 'inventory',
        name: 'inventory',
        component: () => import('@/views/inventory/InventoryDashboard.vue'),
        children: [
          {
            path: 'stock',
            name: 'stock',
            component: () => import('@/views/inventory/stock/StockView.vue'),
          },
          {
            path: 'transfers',
            name: 'transfers',
            component: () => import('@/views/inventory/transfers/TransfersView.vue'),
          },
          {
            path: 'procurement',
            name: 'procurement',
            component: () => import('@/views/inventory/procurement/ProcurementView.vue'),
          },
        ]
      },

      {
        path: 'finance',
        name: 'finance',
        component: () => import('@/views/finance/FinanceDashboard.vue'),
        children: [
          {
            path: 'ledger',
            name: 'ledger',
            component: () => import('@/views/finance/ledger/LedgerView.vue'),
          },
          {
            path: 'tax',
            name: 'tax',
            component: () => import('@/views/finance/tax/TaxCenterView.vue'),
          },
        ]
      },

      {
        path: 'hr',
        name: 'hr',
        component: () => import('@/views/hr/HRDashboard.vue'),
        children: [
          {
            path: 'employees',
            name: 'employees',
            component: () => import('@/views/hr/employees/EmployeesView.vue'),
          },
          {
            path: 'payroll',
            name: 'payroll',
            component: () => import('@/views/hr/payroll/PayrollView.vue'),
          },
        ]
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
