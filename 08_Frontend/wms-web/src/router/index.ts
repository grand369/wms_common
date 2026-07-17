import { createRouter, createWebHistory } from 'vue-router';
import type { RouteRecordRaw } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { usePermissionStore } from '@/stores/permission';
import { asyncRoutes, setupDynamicRoutes } from './dynamicRoutes';

export const constantRoutes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'DefaultLayout',
    component: () => import('@/layouts/DefaultLayout.vue'),
    meta: { title: 'Layout', icon: 'Layout' },
    children: [
      {
        path: '',
        name: 'Dashboard',
        component: () => import('@/views/dashboard/Home.vue'),
        meta: { title: '首页概览', icon: 'Odometer' },
      },
      {
        path: 'system/:pathMatch(.*)*',
        name: 'System',
        component: () => import('@/views/system/Index.vue'),
        meta: { title: '系统管理', icon: 'Setting', hidden: true },
      },
    ],
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/login/Index.vue'),
    meta: { title: '登录', icon: 'User', hidden: true },
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: constantRoutes,
  scrollBehavior() {
    return { top: 0, left: 0 };
  },
});

const whiteList = ['/login'];

router.beforeEach(async (to, _from, next) => {
  const authStore = useAuthStore();
  const token = authStore.token;

  // Allow white-listed routes (login) without auth
  if (whiteList.includes(to.path)) {
    if (token && to.path === '/login') {
      return next({ path: '/' });
    }
    return next();
  }

  // Not authenticated -> redirect to login with original target
  if (!token) {
    return next(`/login?redirect=${encodeURIComponent(to.fullPath)}`);
  }

  // Dynamic permission routes not generated yet -> fetch and register
  const permissionStore = usePermissionStore();
  if (!permissionStore.dynamicRoutesLoaded) {
    try {
      await setupDynamicRoutes();
      // Re-enter the same route after dynamic routes have been added
      next({ ...to, replace: true });
    } catch (error) {
      await authStore.logout();
      next(`/login?redirect=${encodeURIComponent(to.fullPath)}`);
    }
    return;
  }

  next();
});

router.afterEach((to) => {
  const title = (to.meta.title as string) || 'WMS 仓储管理平台';
  document.title = title ? `${title} - WMS 仓储管理平台` : 'WMS 仓储管理平台';
});

export default router;

export { asyncRoutes };
