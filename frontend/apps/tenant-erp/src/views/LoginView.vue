<template>
  <div class="login-container">
    <KCard elevation="lg" class="login-card">
      <template #header>
        <div class="login-header">
          <img src="/images/khet360_logo.png" alt="Khet-360 Logo" class="logo" />
          <h2>Welcome Back</h2>
          <p>Sign in to your business operation center</p>
        </div>
      </template>
      <div class="login-body">
        <form @submit.prevent="handleLogin" class="login-form">
          <KInput label="Email Address" v-model="email" type="email" placeholder="admin@company.co.za" required />
          <KInput label="Password" v-model="password" type="password" placeholder="••••••••" required />
          <div class="form-options">
            <label class="checkbox-label">
              <input type="checkbox" v-model="rememberMe" />
              <span>Remember me</span>
            </label>
            <a href="#" class="forgot-password">Forgot password?</a>
          </div>
          <KButton variant="primary" type="submit" size="lg" class="login-btn">Sign In</KButton>
        </form>
      </div>
    </KCard>
  </template>

  <script setup lang="ts">
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';
  import { KButton, KInput, KCard } from '@khet360/ui-shared';

  const router = useRouter();
  const email = ref('');
  const password = ref('');
  const rememberMe = ref(false);

  const handleLogin = () => {
    // Basic mock login
    router.push('/');
  };
  </script>

  <style scoped>
  .login-container {
    height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;
    background-color: var(--khet-bg);
  }

  .login-card {
    width: 400px;
    padding: 1rem;
  }

  .login-header {
    text-align: center;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 1rem;
  }

  .logo {
    height: 60px;
    margin-bottom: 1rem;
  }

  .login-header h2 {
    font-size: 1.5rem;
    margin: 0;
  }

  .login-header p {
    color: var(--khet-text-muted);
    font-size: 0.9rem;
  }

  .login-body {
    padding: 1rem 0;
  }

  .login-form {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
  }

  .form-options {
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-size: 0.85rem;
    color: var(--khet-text-muted);
  }

  .checkbox-label {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    cursor: pointer;
  }

  .forgot-password {
    color: var(--khet-primary);
    text-decoration: none;
    font-weight: 500;
  }

  .login-btn {
    margin-top: 1rem;
    width: 100%;
  }
  </style>
</template>
