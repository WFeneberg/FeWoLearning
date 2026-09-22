// Exercise 005 — optional, readonly, and "present but undefined" (beginner).
// Goal:   tell `theme?: T` apart from `theme: T | undefined`.
// Drills: readonly properties, optional properties, exactOptionalPropertyTypes.
// Passes: Settings has a readonly id, OmittableKeys names only the properties
//         that may be left out entirely, and resolveTheme defaults to "light".
//
// These are three different promises, and C# has a single `T?` for all of them:
//   readonly id        — present, and may not be reassigned
//   theme?: "light"    — may be missing from the object altogether
//   nickname: string | undefined — must be present; its value may be undefined
// This track sets exactOptionalPropertyTypes, so `theme: undefined` is NOT a
// legal way to spell "no theme" — leaving the key out is.

export interface Settings {
  /** TODO: this must not be reassignable after construction. */
  id: string;
  theme?: "light" | "dark";
  nickname: string | undefined;
}

/**
 * TODO: the keys of Settings that may be omitted from an object entirely.
 * `nickname` is not one of them — it must be present even when its value is
 * undefined. Derive this from Settings rather than writing the key out.
 */
export type OmittableKeys = unknown;

/** The configured theme, defaulting to "light" when none is set. */
export function resolveTheme(_settings: Settings): "light" | "dark" {
  throw new Error("TODO: implement resolveTheme");
}
