// Reference solution — exercise 035.
export interface Config {
  name: string;
  port: number;
  tags: string[];
}

export function isConfig(value: unknown): value is Config {
  if (typeof value !== "object" || value === null || Array.isArray(value)) {
    return false;
  }
  const candidate = value as Record<string, unknown>;
  return (
    typeof candidate["name"] === "string" &&
    typeof candidate["port"] === "number" &&
    Array.isArray(candidate["tags"]) &&
    // The elements, not just the container.
    candidate["tags"].every((tag) => typeof tag === "string")
  );
}

export function parseConfig(text: string): Config | undefined {
  let parsed: unknown;
  try {
    parsed = JSON.parse(text) as unknown;
  } catch {
    return undefined;
  }
  return isConfig(parsed) ? parsed : undefined;
}
