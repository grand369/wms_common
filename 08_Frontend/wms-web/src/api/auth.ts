import { get, post } from '@/api/index';

export interface LoginRequest {
  userNameOrEmailAddress: string;
  password: string;
  rememberMe?: boolean;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

export interface UserProfile {
  id: string;
  userName: string;
  email: string;
  name?: string;
  surname?: string;
  phoneNumber?: string;
  tenantId?: string;
  roles?: string[];
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export function login(data: LoginRequest): Promise<LoginResponse> {
  return post<LoginResponse>('/api/v1/auth/login', data);
}

export function getCurrentUser(): Promise<UserProfile> {
  return get<UserProfile>('/api/v1/auth/current-user');
}

export function getPermissions(): Promise<string[]> {
  return get<string[]>('/api/v1/auth/permissions');
}

export function refreshToken(data: RefreshTokenRequest): Promise<LoginResponse> {
  return post<LoginResponse>('/api/v1/auth/refresh-token', data);
}
