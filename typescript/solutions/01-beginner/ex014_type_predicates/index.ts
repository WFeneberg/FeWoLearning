// Reference solution — exercise 014.
export interface User {
  id: string;
  email: string;
}

export function isUser(value: unknown): value is User {
  if (typeof value !== "object" || value === null) {
    return false;
  }
  const candidate = value as Record<string, unknown>;
  return typeof candidate["id"] === "string" && typeof candidate["email"] === "string";
}

export function isStringArray(value: unknown): value is string[] {
  return Array.isArray(value) && value.every((item) => typeof item === "string");
}
