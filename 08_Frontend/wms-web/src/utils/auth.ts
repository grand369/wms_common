const TOKEN_KEY = 'wms_token';
const USER_NAME_KEY = 'wms_user_name';

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

export function setToken(token: string): void {
  localStorage.setItem(TOKEN_KEY, token);
}

export function removeToken(): void {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_NAME_KEY);
}

export function setUserName(name: string): void {
  localStorage.setItem(USER_NAME_KEY, name);
}

export function getUserName(): string | null {
  return localStorage.getItem(USER_NAME_KEY);
}

export function isAuthenticated(): boolean {
  return !!getToken();
}
