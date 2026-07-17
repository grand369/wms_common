/**
 * Check if the current user has a specific permission.
 * In v1.0, permissions are managed by ABP Permission Management.
 */
export function hasPermission(permissionName: string): boolean {
  const permissions: string[] = JSON.parse(
    localStorage.getItem('wms_permissions') || '[]'
  );
  return permissions.includes(permissionName);
}

/**
 * Check if the current user has any of the specified permissions.
 */
export function hasAnyPermission(...permissionNames: string[]): boolean {
  return permissionNames.some((name) => hasPermission(name));
}

/**
 * Check if the current user has all of the specified permissions.
 */
export function hasAllPermissions(...permissionNames: string[]): boolean {
  return permissionNames.every((name) => hasPermission(name));
}

/**
 * Store permissions in localStorage after login.
 */
export function setPermissions(permissions: string[]): void {
  localStorage.setItem('wms_permissions', JSON.stringify(permissions));
}

/**
 * Clear permissions on logout.
 */
export function clearPermissions(): void {
  localStorage.removeItem('wms_permissions');
}
