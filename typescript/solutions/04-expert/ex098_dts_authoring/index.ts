// Reference solution — exercise 098.
import format, { DEFAULTS, slugify, truncate } from "./legacy";

export function shortSlug(title: string, limit: number): string {
  return truncate(slugify(title), limit);
}

export function underscored(title: string): string {
  return slugify(title, "_");
}

export function formatted(title: string): string {
  return format(title);
}

export function defaultLimit(): number {
  return DEFAULTS.limit;
}
