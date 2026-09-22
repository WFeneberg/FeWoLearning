// Reference solution — exercise 002.
export interface Config {
  host: string;
}

// The second declaration merges into the first rather than replacing it.
export interface Config {
  port: number;
}

export type Id = string | number;

export function formatConfig(config: Config): string {
  return `${config.host}:${config.port}`;
}
