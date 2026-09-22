// Reference solution — exercise 003.
export const LEVELS = ["debug", "info", "warn", "error"] as const;

export type LogLevel = (typeof LEVELS)[number];

export function isAtLeast(level: LogLevel, min: LogLevel): boolean {
  return LEVELS.indexOf(level) >= LEVELS.indexOf(min);
}
