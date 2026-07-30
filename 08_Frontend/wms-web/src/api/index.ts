import axios from 'axios';
import type { AxiosInstance, AxiosRequestConfig, InternalAxiosRequestConfig, AxiosResponse } from 'axios';
import { ElMessage } from 'element-plus';
import { getToken, removeToken } from '@/utils/auth';
import { getFriendlyErrorMessage, parseAxiosError } from '@/utils/errorHandler';

const baseURL: string = import.meta.env.VITE_API_BASE_URL || '/api';
const service: AxiosInstance = axios.create({
  baseURL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor — add JWT token
service.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor — handle errors and ABP error format
service.interceptors.response.use(
  (response: AxiosResponse) => {
    const res = response.data;
    // ABP error response format check
    if (res.error) {
      const err = res.error
      // Special handling for non-critical validation warnings
      if (err.code === 401) {
        removeToken();
        window.location.href = '/login';
      }
      const friendlyMsg = getFriendlyErrorMessage({
        code: err.code,
        message: err.message,
        details: err.details,
      })
      ElMessage.error(friendlyMsg)
      return Promise.reject(new Error(friendlyMsg))
    }
    return res;
  },
  (error) => {
    const status = error.response?.status;
    const parsedError = parseAxiosError(error);
    const friendlyMsg = getFriendlyErrorMessage(parsedError)
    
    // Handle ABP BusinessException
    if (parsedError.code || parsedError.details) {
      ElMessage.error(friendlyMsg)
      if (parsedError.code === '401' || status === 401) {
        removeToken()
        window.location.href = '/login'
      }
      return Promise.reject(error)
    }
    
    // Generic HTTP error handling
    if (status === 401) {
      removeToken()
      window.location.href = '/login'
      ElMessage.error('登录已过期，请重新登录')
    } else if (status === 403) {
      ElMessage.error('没有权限执行此操作')
    } else if (status === 404) {
      ElMessage.error('请求的资源不存在')
    } else if (status === 500) {
      ElMessage.error('服务器内部错误，请稍后重试')
    } else {
      ElMessage.error(friendlyMsg)
    }
    return Promise.reject(error)
  }
);

export default service;

// Generic CRUD API methods
export function get<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
  return service.get(url, config) as Promise<T>;
}

export function post<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
  return service.post(url, data, config) as Promise<T>;
}

export function put<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
  return service.put(url, data, config) as Promise<T>;
}

export function patch<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
  return service.patch(url, data, config) as Promise<T>;
}

export function del<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
  return service.delete(url, config) as Promise<T>;
}
