<template>
  <div class="login-wrapper">
    <!-- Left: Branding Panel -->
    <div class="login-brand">
      <div class="brand-content">
        <div class="brand-logo">
          <el-icon :size="48"><Box /></el-icon>
          <h1 class="brand-name">WMS 仓储管理平台</h1>
        </div>
        <p class="brand-subtitle">智能制造 · 仓储数字化 · 高效协同</p>
        <div class="brand-features">
          <div class="feature-item">
            <el-icon><Check /></el-icon>
            <span>多仓库统一管理</span>
          </div>
          <div class="feature-item">
            <el-icon><Check /></el-icon>
            <span>实时库存可视化</span>
          </div>
          <div class="feature-item">
            <el-icon><Check /></el-icon>
            <span>智能出入库流程</span>
          </div>
          <div class="feature-item">
            <el-icon><Check /></el-icon>
            <span>全链路追溯与报表</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Right: Login Form -->
    <div class="login-form-panel">
      <div class="login-form-card">
        <h2 class="login-title">欢迎登录</h2>
        <p class="login-desc">请输入您的账号信息登录系统</p>

        <el-form
          ref="formRef"
          :model="loginForm"
          :rules="rules"
          class="login-form"
          @keyup.enter="handleLogin"
        >
          <el-form-item prop="userNameOrEmailAddress">
            <el-input
              v-model="loginForm.userNameOrEmailAddress"
              placeholder="请输入用户名或邮箱"
              :prefix-icon="User"
              size="large"
              clearable
            />
          </el-form-item>

          <el-form-item prop="password">
            <el-input
              v-model="loginForm.password"
              type="password"
              placeholder="请输入密码"
              :prefix-icon="Lock"
              size="large"
              show-password
            />
          </el-form-item>

          <div class="login-options">
            <el-checkbox v-model="loginForm.rememberMe">记住账号</el-checkbox>
          </div>

          <el-form-item>
            <el-button
              type="primary"
              size="large"
              class="login-btn"
              :loading="loading"
              @click="handleLogin"
            >
              {{ loading ? '登录中...' : '登 录' }}
            </el-button>
          </el-form-item>
        </el-form>

        <div class="login-footer">
          <span>&copy; {{ currentYear }} WMS 仓储管理平台. All rights reserved.</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { ElMessage } from 'element-plus';
import { User, Lock, Box, Check } from '@element-plus/icons-vue';
import { useAuthStore } from '@/stores/auth';
import type { FormInstance, FormRules } from 'element-plus';

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();

const formRef = ref<FormInstance>();
const loading = ref(false);

const currentYear = computed(() => new Date().getFullYear());

const loginForm = reactive({
  userNameOrEmailAddress: '',
  password: '',
  rememberMe: false,
});

const rules: FormRules = {
  userNameOrEmailAddress: [
    { required: true, message: '请输入用户名或邮箱', trigger: 'blur' },
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 4, message: '密码长度不能少于 4 位', trigger: 'blur' },
  ],
};

async function handleLogin() {
  if (!formRef.value) return;

  try {
    await formRef.value.validate();
  } catch {
    return;
  }

  loading.value = true;
  try {
    await authStore.login({
      userNameOrEmailAddress: loginForm.userNameOrEmailAddress.trim(),
      password: loginForm.password,
      rememberMe: loginForm.rememberMe,
    });

    ElMessage.success('登录成功，欢迎使用 WMS 仓储管理平台');

    // Redirect to target path or home
    const redirect = (route.query.redirect as string) || '/';
    router.push(redirect);
  } catch (error: any) {
    const message =
      error?.response?.data?.error?.message ||
      error?.message ||
      '登录失败，请检查用户名和密码';
    ElMessage.error(message);
  } finally {
    loading.value = false;
  }
}
</script>

<style lang="scss" scoped>
@use '@/styles/variables.scss' as *;

.login-wrapper {
  display: flex;
  height: 100vh;
  width: 100vw;
  overflow: hidden;
}

// ── Left Branding Panel ──────────────────────────────────────
.login-brand {
  flex: 0 0 480px;
  background: linear-gradient(135deg, #1e3a5f 0%, #2563eb 60%, #3b82f6 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  overflow: hidden;

  &::before {
    content: '';
    position: absolute;
    top: -50%;
    right: -40%;
    width: 120%;
    height: 200%;
    background: radial-gradient(
      circle,
      rgba(255, 255, 255, 0.05) 0%,
      transparent 70%
    );
  }

  &::after {
    content: '';
    position: absolute;
    bottom: 0;
    left: 0;
    right: 0;
    height: 200px;
    background: linear-gradient(
      to top,
      rgba(0, 0, 0, 0.15),
      transparent
    );
    z-index: 1;
  }
}

.brand-content {
  position: relative;
  z-index: 2;
  color: #fff;
  text-align: center;
  padding: $wms-spacing-xxl;
}

.brand-logo {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: $wms-spacing-md;
  margin-bottom: $wms-spacing-md;

  .el-icon {
    color: rgba(255, 255, 255, 0.9);
  }
}

.brand-name {
  font-size: 28px;
  font-weight: 700;
  letter-spacing: 2px;
  color: #fff;
}

.brand-subtitle {
  font-size: $wms-font-size-body;
  color: rgba(255, 255, 255, 0.75);
  margin-bottom: $wms-spacing-xl;
  letter-spacing: 4px;
}

.brand-features {
  display: flex;
  flex-direction: column;
  gap: $wms-spacing-sm;
  text-align: left;
  max-width: 260px;
  margin: 0 auto;
}

.feature-item {
  display: flex;
  align-items: center;
  gap: $wms-spacing-sm;
  font-size: $wms-font-size-body;
  color: rgba(255, 255, 255, 0.85);

  .el-icon {
    color: #60d39c;
    font-size: 18px;
  }
}

// ── Right Login Form Panel ───────────────────────────────────
.login-form-panel {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  background: $wms-bg-base;
  padding: $wms-spacing-lg;
}

.login-form-card {
  width: 400px;
  background: $wms-bg-content;
  border-radius: $wms-radius-lg;
  padding: $wms-spacing-xxl $wms-spacing-xl;
  box-shadow: $wms-shadow-md;
}

.login-title {
  font-size: $wms-font-size-h1;
  font-weight: 700;
  color: $wms-text-primary;
  margin-bottom: $wms-spacing-xs;
}

.login-desc {
  font-size: $wms-font-size-body;
  color: $wms-text-secondary;
  margin-bottom: $wms-spacing-xl;
}

.login-form {
  :deep(.el-input__wrapper) {
    box-shadow: 0 0 0 1px $border-light inset;
    border-radius: $wms-radius-md;
    transition: border-color $wms-transition-hover, box-shadow $wms-transition-hover;

    &:hover {
      box-shadow: 0 0 0 1px $wms-color-primary-light-3 inset;
    }
  }

  :deep(.el-input.is-focus .el-input__wrapper) {
    box-shadow: 0 0 0 1px $wms-color-primary inset, 0 0 0 3px rgba(37, 99, 235, 0.1);
  }
}

.login-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: calc($wms-spacing-md + 2px);
  font-size: $wms-font-size-body;

  :deep(.el-checkbox__label) {
    color: $wms-text-regular;
    font-size: $wms-font-size-body;
  }
}

.login-btn {
  width: 100%;
  font-size: $wms-font-size-h2;
  letter-spacing: 4px;
  height: 44px;
}

.login-footer {
  text-align: center;
  margin-top: $wms-spacing-lg;
  font-size: $wms-font-size-small;
  color: $wms-text-secondary;

  span {
    display: block;
  }
}

// ── Responsive ────────────────────────────────────────────────
@media (max-width: 768px) {
  .login-brand {
    display: none;
  }

  .login-form-card {
    width: 100%;
    max-width: 400px;
    padding: $wms-spacing-xl $wms-spacing-lg;
  }
}
</style>
