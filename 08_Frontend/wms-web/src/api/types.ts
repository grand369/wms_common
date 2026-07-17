export interface PagedParams {
  skipCount?: number;
  maxResultCount?: number;
  sorting?: string;
  filter?: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
}

export interface EntityDto<T = string> {
  id: T;
}

export interface AuditedEntityDto<T = string> extends EntityDto<T> {
  creationTime?: string;
  creatorId?: string;
  lastModificationTime?: string;
  lastModifierId?: string;
}

export interface ListResultDto<T> {
  items: T[];
}
