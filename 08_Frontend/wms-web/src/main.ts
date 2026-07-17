import { createApp } from 'vue';
import { createPinia } from 'pinia';
import ElementPlus from 'element-plus';
import zhCn from 'element-plus/es/locale/lang/zh-cn';
import * as ElementPlusIconsVue from '@element-plus/icons-vue';
import router from './router';
import App from './App.vue';
import './styles/global.scss';

const app = createApp(App);

// Register Element Plus with Chinese locale
app.use(ElementPlus, { locale: zhCn });

// Register all Element Plus icons
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component);
}

// Register Pinia state management
app.use(createPinia());

// Register Vue Router
app.use(router);

app.mount('#app');
