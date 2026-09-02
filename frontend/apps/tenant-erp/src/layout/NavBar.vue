<template>
  <nav class="nav-bar">
    <div class="nav-logo">
      <img src="/images/khet360_logo.png" alt="Khet-360 Logo" />
      <span class="app-name">Tenant ERP</span>
    </div>

    <div class="nav-groups">
      <div v-for="group in navGroups" :key="group.label" class="nav-group">
        <div class="group-label">{{ group.label }}</div>
        <div class="group-items">
          <router-link
            v-for="item in group.items"
            :key="item.name"
            :to="item.to"
            class="nav-item"
            active-class="active"
          >
            <span class="nav-icon">{{ item.icon }}</span>
            <span class="nav-text">{{ item.text }}</span>
          </router-link>
        </div>
      </div>
    </div>

    <div class="nav-footer">
      <div class="user-badge">
        <div class="avatar">JD</div>
        <div class="user-info">
          <div class="user-name">John Doe</div>
          <div class="user-role">Administrator</div>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
const navGroups = [
  {
    label: 'Operation Center',
    items: [
      { name: 'my-work', text: 'My Work', to: '/', icon: '📅' },
      { name: 'team-queue', text: 'Team Queue', to: '/team-queue', icon: '👥' },
      { name: 'exceptions', text: 'Exceptions', to: '/exceptions', icon: '⚠️' },
    ]
  },
  {
    label: 'Business Areas',
    items: [
      { name: 'crm', text: 'CRM & Customers', to: '/crm', icon: '🤝' },
      { name: 'operations', text: 'Funeral Ops', to: '/operations', icon: '⚰️' },
      { name: 'inventory', text: 'Inventory', to: '/inventory', icon: '📦' },
      { name: 'fleet', text: 'Fleet', to: '/fleet', icon: '🚚' },
      { name: 'finance', text: 'Finance', to: '/finance', icon: '💰' },
      { name: 'hr', text: 'HR & Payroll', to: '/hr', icon: '👥' },
    ]
  },


  {
    label: 'Platform',
    items: [
      { name: 'settings', text: 'Settings', to: '/settings', icon: '⚙️' },
      { name: 'support', text: 'Support', to: '/support', icon: '🎧' },
    ]
  }
];
</script>

<style scoped>
.nav-bar {
  width: 260px;
  height: 100vh;
  background-color: var(--khet-surface);
  border-right: 1px solid var(--khet-border);
  display: flex;
  flex-direction: column;
  position: fixed;
  left: 0;
  top: 0;
  z-index: 100;
}

.nav-logo {
  padding: 2rem 1.5rem;
  display: flex;
  align-items: center;
  gap: 1rem;
  border-bottom: 1px solid var(--khet-border);
}

.nav-logo img {
  height: 32px;
}

.app-name {
  font-weight: 800;
  font-size: 1.1rem;
  color: var(--khet-text-main);
}

.nav-groups {
  flex: 1;
  overflow-y: auto;
  padding: 1.5rem 0;
}

.nav-group {
  margin-bottom: 2rem;
}

.group-label {
  padding: 0 1.5rem 0.5rem;
  font-size: 0.7rem;
  font-weight: 700;
  text-transform: uppercase;
  color: var(--khet-text-muted);
  letter-spacing: 0.05em;
}

.group-items {
  display: flex;
  flex-direction: column;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 1.5rem;
  text-decoration: none;
  color: var(--khet-text-muted);
  font-size: 0.9rem;
  font-weight: 500;
  transition: all 0.2s;
}

.nav-item:hover {
  background-color: var(--khet-surface-alt);
  color: var(--khet-text-main);
}

.nav-item.active {
  background-color: var(--khet-primary-light);
  color: var(--khet-primary);
  border-right: 4px solid var(--khet-primary);
}

.nav-icon {
  font-size: 1.1rem;
  width: 20px;
  text-align: center;
}

.nav-footer {
  padding: 1.5rem;
  border-top: 1px solid var(--khet-border);
  background-color: var(--khet-surface-alt);
}

.user-badge {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.avatar {
  width: 32px;
  height: 32px;
  background-color: var(--khet-primary);
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.8rem;
  font-weight: 700;
}

.user-info {
  display: flex;
  flex-direction: column;
}

.user-name {
  font-size: 0.85rem;
  font-weight: 600;
  color: var(--khet-text-main);
}

.user-role {
  font-size: 0.75rem;
  color: var(--khet-text-muted);
}
</style>
