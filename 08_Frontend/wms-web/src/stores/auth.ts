import { defineStore } from 'pinia';
import { getToken, setToken, removeToken, setUserName, getUserName } from '@/utils/auth';
import { login, getCurrentUser, getPermissions } from '@/api/auth';
import type { LoginRequest, UserProfile } from '@/api/auth';
import { usePermissionStore } from './permission';

export interface AuthState {
  token: string | null;
  user: UserProfile | null;
  permissions: string[];
  userName: string | null;
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    token: getToken(),
    user: null,
    permissions: [],
    userName: getUserName(),
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    username: (state) => state.userName || state.user?.name || state.user?.userName,
  },

  actions: {
    async login(payload: LoginRequest & { rememberMe?: boolean }) {
      const response = await login(payload);
      this.token = response.accessToken;
      setToken(response.accessToken);

      const user = await getCurrentUser();
      this.user = user;
      this.userName = user.userName;
      setUserName(user.userName);

      const permissions = await getPermissions();
      this.permissions = permissions;

      return response;
    },

    setToken(token: string) {
      this.token = token;
      setToken(token);
    },

    setUser(user: UserProfile) {
      this.user = user;
      this.userName = user.userName;
      setUserName(user.userName);
    },

    setPermissions(permissions: string[]) {
      this.permissions = permissions;
    },

    hasPermission(permission: string): boolean {
      // Wms.All wildcard or admin role — pass all checks
      if (this.permissions.includes('Wms.All') || this.permissions.includes('admin')) return true;

      // Exact match
      if (this.permissions.includes(permission)) return true;

      // Module prefix match: e.g. permission "Wms.Warehouse" matches
      // any granted permission starting with "Wms.Warehouse." (like "Wms.Warehouse.Read")
      for (const p of this.permissions) {
        if (p.startsWith(permission + '.') || permission.startsWith(p + '.')) return true;
      }

      return false;
    },

    logout() {
      const permissionStore = usePermissionStore();
      this.token = null;
      this.user = null;
      this.permissions = [];
      this.userName = null;
      removeToken();
      permissionStore.reset();
    },
  },
});
