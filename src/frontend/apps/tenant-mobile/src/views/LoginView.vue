<template>
  <div class="login-view">
    <div class="login-container">
      <div class="login-header">
        <h2>Sign In</h2>
      </div>
      <div class="login-form">
        <div class="form-group">
          <label for="username">Username</label>
          <input type="text" id="username" class="form-input" placeholder="Username" />
        </div>
        <div class="form-group">
          <label for="password">Password</label>
          <input type="password" id="password" class="form-input" placeholder="Password" />
        </div>
        <div class="form-group">
          <button type="submit" class="btn-primary btn-block">Sign In</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
</script>

<style scoped>
.login-view {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: #f8fafc;
}

.login-container {
  background-color: white;
  padding: 1.5rem;
  border-radius: 0.5rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  width: 100%;
  max-width: 300px;
}

.login-header {
  margin-bottom: 1rem;
  text-align: center;
}

.login-header h2 {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1f2937;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.form-group label {
  font-weight: 500;
  font-size: 0.875rem;
  color: #374151;
}

.form-input {
  padding: 0.5rem;
  border: 1px solid #d1d5db;
  border-radius: 0.25rem;
  font-size: 0.875rem;
}

.form-input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.5);
}

.btn-primary {
  background-color: #3b82f6;
  color: white;
  border: none;
  padding: 0.5rem;
  font-weight: 600;
  border-radius: 0.25rem;
  cursor: pointer;
}

.btn-primary:hover {
  background-color: #2563eb;
}

.btn-block {
  width: 100%;
}
</style>