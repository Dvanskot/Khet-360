import { defineStore } from 'pinia';
import { ref } from 'vue';

export interface UserContext {
  userId: string;
  userName: string;
  tenantId: string;
  tenantName: string;
  branchId: string;
  branchName: string;
  roles: string[];
}

export const useUserStore = defineStore('user', () => {
  const user = ref<UserContext | null>(null);
  const token = ref<string | null>(localStorage.getItem('khet360_token'));

  function setToken(newToken: string) {
    token.value = newToken;
    localStorage.setItem('khet360_token', newToken);
  }

  function setUser(userData: UserContext) {
    user.value = userData;
  }

  function logout() {
    user.value = null;
    token.value = null;
    localStorage.removeItem('khet360_token');
  }

  return {
    user,
    token,
    setToken,
    setUser,
    logout,
  };
});
