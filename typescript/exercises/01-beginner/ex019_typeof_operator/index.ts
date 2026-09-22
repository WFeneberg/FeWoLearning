// Exercise 019 — the `typeof` type operator (beginner).
// Goal:   derive a type from a value that already exists.
// Drills: typeof on an object, typeof on a function.
// Passes: Settings matches the shape of defaultSettings, Formatter matches
//         the signature of formatDuration, and withOverrides merges.
//
// There are two `typeof`s in TypeScript and they are unrelated. In an
// expression it is JavaScript's runtime operator returning a string. In a
// type position it is this one: "the type of that value". A configuration
// object, a function, a module namespace — anything with a value can have its
// type read off instead of written twice.
//
// Note the deliberate absence of `as const` here, unlike ex011: the fields
// are meant to widen, because withOverrides has to accept any number.

export const defaultSettings = {
  retries: 3,
  timeoutMs: 5_000,
  verbose: false,
};

/** Given, to read a type off. Do not change. */
export function formatDuration(ms: number): string {
  return `${(ms / 1000).toFixed(1)}s`;
}

/** TODO: the type of defaultSettings. Derive it; do not write the shape out. */
export type Settings = unknown;

/** TODO: the type of formatDuration. Derive it too. */
export type Formatter = unknown;

/** defaultSettings with `overrides` applied on top. */
export function withOverrides(_overrides: Partial<Settings>): Settings {
  throw new Error("TODO: implement withOverrides");
}
