// Reference solution — exercise 074.
export interface Settings {
  id: string;
  display: {
    theme: string;
    layout: { columns: number; dense: boolean };
  };
  tags: string[];
}

export type Paths<T> = T extends readonly unknown[]
  ? never
  : T extends object
    ? {
        // The branch itself AND everything beneath it.
        [K in keyof T & string]: T[K] extends readonly unknown[]
          ? K
          : T[K] extends object
            ? K | `${K}.${Paths<T[K]>}`
            : K;
      }[keyof T & string]
    : never;

export function pathsOf(value: object): string[] {
  const result: string[] = [];
  for (const [key, child] of Object.entries(value)) {
    result.push(key);
    if (typeof child === "object" && child !== null && !Array.isArray(child)) {
      for (const nested of pathsOf(child as object)) {
        result.push(`${key}.${nested}`);
      }
    }
  }
  return result;
}
