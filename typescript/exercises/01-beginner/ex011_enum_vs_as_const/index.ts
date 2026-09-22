// Exercise 011 — numeric enums, and the idiom that replaced them (beginner).
// Goal:   see what a numeric enum really is at runtime, and build the
//         `as const` object most TypeScript code uses instead.
// Drills: reverse mapping, `as const` objects, deriving a union from values.
// Passes: enumNames strips the reverse-mapping entries, Compass keeps its
//         literal types, and CompassPoint is the union of its values.
//
// `Direction` below is given so you can look at it. Coming from C#, the
// surprise is that a numeric enum compiles to an ordinary object carrying
// BOTH directions of the mapping — name to value AND value back to name — so
// its four members produce eight keys. That is what makes Object.keys() on
// one of them useless without filtering, and it is one reason the `as const`
// object has largely replaced it.

/** Given, to inspect. Do not change. */
export enum Direction {
  Up,
  Down,
  Left,
  Right,
}

/**
 * The member names of a numeric enum, without the reverse-mapping entries.
 * TODO: enumNames(Direction) is ["Up", "Down", "Left", "Right"].
 */
export function enumNames(_enumObject: Record<string, string | number>): string[] {
  throw new Error("TODO: implement enumNames");
}

/**
 * TODO: the same four directions as a plain object whose values are the
 * lowercase strings "up", "down", "left", "right" — with their literal types
 * preserved, not widened to string.
 */
export const Compass = {} as const;

/** TODO: the union of Compass's values. Derive it; do not retype the strings. */
export type CompassPoint = unknown;

/** The opposite point — opposite("up") is "down". */
export function opposite(_point: CompassPoint): CompassPoint {
  throw new Error("TODO: implement opposite");
}
