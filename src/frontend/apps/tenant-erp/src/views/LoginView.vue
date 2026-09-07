<template>
  <div class="login-view">
    <div class="login-container">
      <div class="login-header">
        <h2>Sign In to Khet-360</h2>
      </div>
      <div class="login-form">
        <div class="form-group">
          <label for="username">Username</label>
          <input type="text" id="username" class="form-input" placeholder="Enter your username" />
        </div>
        <div class="form-group">
          <label for="password">Password</label>
          <input type="password" id="password" class="form-input" placeholder="Enter your password" />
        </div>
        <div class="form-group">
          <button type="submit" class="btn-primary btn-block">Sign In</button>
        </div>
      </div>
      <div class="login-footer">
        <p>Don't have an account? <a href="/register">Register here</a></p>
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
  padding: 2rem;
  border-radius: 0.75rem;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -2px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 400px;
}

.login-header {
  margin-bottom: 1.5rem;
  text-align: center;
}

.login-header h2 {
  font-size: 1.5rem;
  font-weight: 600;
  color: #1f2937;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group label {
  font-weight: 500;
  font-size: 0.875rem;
  color: #374151;
}

.form-input {
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 0.875rem;
}

.form-input:focus {
  outline: none;
  border-color: #3b82f6;
  ring-color: #3b82f6;
  ring-width: 1px;
}

.btn-primary {
  background-color: #3b82f6;
  color: white;
  border: none;
  padding: 0.75rem;
  font-weight: 600;
  border-radius: 0.375rem;
  cursor: pointer;
}

.btn-primary:hover {
  background-color: #2563eb;
}

.btn-block {
  width: 100%;
}

.login-footer {
  margin-top: 1.5rem;
  text-align: center;
  font-size: 0.875rem;
  color: #6b7280;
}

.login-footer a {
  color: #3b82f6;
  text-decoration: underline;
}

.login-footer a:hover {
  text-decoration: none;
}
</style>