// Reference solution — exercise 019.
export const defaultSettings = {
  retries: 3,
  timeoutMs: 5_000,
  verbose: false,
};

export function formatDuration(ms: number): string {
  return `${(ms / 1000).toFixed(1)}s`;
}

export type Settings = typeof defaultSettings;

export type Formatter = typeof formatDuration;

export function withOverrides(overrides: Partial<Settings>): Settings {
  return { ...defaultSettings, ...overrides };
}
