// Reference solution — exercise 072.
export interface Profile {
  id: string;
  createdAt: Date;
  owner: { name: string; email: string };
  tags: string[];
  rows: { label: string; count: number }[];
  render: (value: string) => string;
}

export type DeepPartial<T> = T extends (...args: never[]) => unknown
  ? T
  : T extends Date
    ? T
    : T extends readonly (infer E)[]
      ? // The property becomes optional above; the elements do not.
        DeepPartial<E>[]
      : T extends object
        ? { [K in keyof T]?: DeepPartial<T[K]> }
        : T;

function isPlainObject(value: unknown): value is Record<string, unknown> {
  return (
    typeof value === "object" &&
    value !== null &&
    !Array.isArray(value) &&
    !(value instanceof Date)
  );
}

export function mergeDeep(base: Profile, patch: DeepPartial<Profile>): Profile {
  const result: Record<string, unknown> = { ...base };
  for (const [key, value] of Object.entries(patch as Record<string, unknown>)) {
    if (value === undefined) {
      continue;
    }
    const existing = result[key];
    result[key] =
      isPlainObject(existing) && isPlainObject(value)
        ? { ...existing, ...value }
        : value;
  }
  return result as unknown as Profile;
}
