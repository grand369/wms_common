import axios from 'axios';
import type { AxiosInstance, AxiosRequestConfig, InternalAxiosRequestConfig, AxiosResponse } from 'axios';
import { ElMessage } from 'element-plus';
import { getToken, removeToken } from '@/utils/auth';

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
      ElMessage.error(res.error.message || 'Request failed');
      if (res.error.code === 401) {
        removeToken();
        window.location.href = '/login';
      }
      return Promise.reject(new Error(res.error.message || 'Request failed'));
    }
    return res;
  },
  (error) => {
    const status = error.response?.status;
    if (status === 401) {
      removeToken();
      window.location.href = '/login';
    } else if (status === 403) {
      ElMessage.error('No permission to access this resource');
    } else if (status === 500) {
      ElMessage.error('Server internal error');
    } else {
      ElMessage.error(error.message || 'Network error');
    }
    return Promise.reject(error);
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
