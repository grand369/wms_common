import { defineStore } from 'pinia';
import type { RouteRecordRaw } from 'vue-router';

export interface PermissionState {
  dynamicRoutesLoaded: boolean;
  permissions: string[];
  grantedRoutes: RouteRecordRaw[];
}

export const usePermissionStore = defineStore('permission', {
  state: (): PermissionState => ({
    dynamicRoutesLoaded: false,
    permissions: [],
    grantedRoutes: [],
  }),

  actions: {
    setGrantedRoutes(routes: RouteRecordRaw[], permissions: string[]) {
      this.grantedRoutes = routes;
      this.permissions = permissions;
      this.dynamicRoutesLoaded = true;
    },

    reset() {
      this.dynamicRoutesLoaded = false;
      this.permissions = [];
      this.grantedRoutes = [];
    },
  },
});
