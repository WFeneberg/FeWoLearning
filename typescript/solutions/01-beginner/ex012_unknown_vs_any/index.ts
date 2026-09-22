// Reference solution — exercise 012.
export function parseJson(text: string) {
  // The annotation is the whole point: JSON.parse is declared to return any,
  // and without this cast that any escapes into every caller.
  return JSON.parse(text) as unknown;
}

export function lengthOf(value: unknown): number {
  if (typeof value === "string" || Array.isArray(value)) {
    return value.length;
  }
  return 0;
}

export function asRecord(value: unknown): Record<string, unknown> | undefined {
  if (typeof value !== "object" || value === null || Array.isArray(value)) {
    return undefined;
  }
  return value as Record<string, unknown>;
}
