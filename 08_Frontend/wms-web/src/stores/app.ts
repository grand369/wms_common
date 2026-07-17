import { defineStore } from 'pinia';

export type ThemeMode = 'light' | 'dark';

export interface AppState {
  sidebarCollapsed: boolean;
  currentModule: string;
  theme: ThemeMode;
  themeColor: string;
  language: string;
  tagsView: boolean;
}

export const useAppStore = defineStore('app', {
  state: (): AppState => ({
    sidebarCollapsed: false,
    currentModule: '',
    theme: 'light',
    themeColor: '#2563EB',
    language: 'zh-CN',
    tagsView: true,
  }),

  getters: {
    isDark: (state) => state.theme === 'dark',
    isCollapsed: (state) => state.sidebarCollapsed,
  },

  actions: {
    toggleSidebar() {
      this.sidebarCollapsed = !this.sidebarCollapsed;
    },

    toggleCollapse() {
      this.sidebarCollapsed = !this.sidebarCollapsed;
    },

    setSidebarCollapsed(collapsed: boolean) {
      this.sidebarCollapsed = collapsed;
    },

    setCurrentModule(module: string) {
      this.currentModule = module;
    },

    setTheme(theme: ThemeMode) {
      this.theme = theme;
      document.documentElement.setAttribute('data-theme', theme);
    },

    setThemeColor(color: string) {
      this.themeColor = color;
    },

    setLanguage(language: string) {
      this.language = language;
    },

    toggleTagsView() {
      this.tagsView = !this.tagsView;
    },
  },
});
