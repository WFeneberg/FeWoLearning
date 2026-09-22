// Reference solution — exercise 069.
export function route<const T extends readonly string[]>(parts: T): T {
  return parts;
}

export function defineConfig<const T>(config: T): T {
  return config;
}

export function pathOf(parts: readonly string[]): string {
  return `/${parts.join("/")}`;
}
