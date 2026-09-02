<template>
  <div class="page">
    <header class="page-header">
      <h1 class="title">Profile</h1>
    </header>

    <div class="profile-card">
      <div class="avatar">👤</div>
      <div class="user-info">
        <h2>Lancelot Tshikota</h2>
        <p>Funeral Ambassador</p>
        <span class="badge">Branch: Johannesburg North</span>
      </div>
    </div>

    <div class="settings-group">
      <h3>Account</h3>
      <div class="setting-item">
        <span>Notifications</span>
        <input type="checkbox" checked />
      </div>
      <div class="setting-item">
        <span>Offline Mode</span>
        <input type="checkbox" checked />
      </div>
      <div class="setting-item">
        <span>Biometric Login</span>
        <input type="checkbox" />
      </div>
    </div>

    <div class="settings-group">
      <h3>App Info</h3>
      <div class="setting-item">
        <span>Version</span>
        <span class="value">v1.0.4-beta</span>
      </div>
      <div class="setting-item">
        <span>Last Sync</span>
        <span class="value">2 mins ago</span>
      </div>
    </div>

    <button @click="logout" class="btn-logout">Logout</button>
  </div>
</template>

<script setup lang="ts">
function logout() {
  alert('Logging out...');
}
</script>

<style scoped>
.page {
  padding: 1.5rem;
}

.page-header {
  margin-bottom: 2rem;
}

.title {
  font-size: 1.8rem;
  font-weight: 700;
  margin: 0;
}

.profile-card {
  background: var(--khet-surface);
  border-radius: 20px;
  padding: 2rem 1.5rem;
  text-align: center;
  border: 1px solid var(--khet-border);
  margin-bottom: 2rem;
}

.avatar {
  font-size: 4rem;
  margin-bottom: 1rem;
}

.user-info h2 {
  font-size: 1.4rem;
  margin: 0;
  font-weight: 700;
}

.user-info p {
  font-size: 1rem;
  color: var(--khet-text-muted);
  margin: 0.25rem 0 1rem 0;
}

.badge {
  font-size: 0.75rem;
  background: rgba(0,0,0,0.05);
  padding: 4px 12px;
  border-radius: 12px;
  font-weight: 600;
  color: var(--khet-text-muted);
}

.settings-group {
  margin-bottom: 2rem;
}

.settings-group h3 {
  font-size: 0.9rem;
  text-transform: uppercase;
  color: var(--khet-text-muted);
  letter-spacing: 0.05rem;
  margin-bottom: 1rem;
}

.setting-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 0;
  border-bottom: 1px solid var(--khet-border);
  font-size: 0.95rem;
}

.setting-item .value {
  font-weight: 600;
  color: var(--khet-text-muted);
}

.btn-logout {
  width: 100%;
  background: transparent;
  border: 1px solid #ef4444;
  color: #ef4444;
  padding: 12px;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  margin-top: 2rem;
}
</style>
