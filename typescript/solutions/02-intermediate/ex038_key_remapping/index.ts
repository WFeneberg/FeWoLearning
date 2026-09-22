// Reference solution — exercise 038.
export interface Api {
  getUser(): string;
  getPost(): string;
  id: number;
  label: string;
}

// `never` in the `as` clause contributes no key, so the conditional filters.
export type MethodsOf<T> = {
  [K in keyof T as T[K] extends (...args: never[]) => unknown ? K : never]: T[K];
};

export type DataOf<T> = {
  [K in keyof T as T[K] extends (...args: never[]) => unknown ? never : K]: T[K];
};

export type Prefixed<T> = {
  [K in keyof T as `raw_${string & K}`]: T[K];
};

export function dataOnly(api: Api): DataOf<Api> {
  return { id: api.id, label: api.label };
}
