// Exercise 003 — literal types and `as const` (beginner).
// Goal:   keep a value's literal type instead of letting it widen to string.
// Drills: `as const`, deriving a union from a tuple with typeof + [number].
// Passes: LEVELS is a readonly tuple of four exact literals, LogLevel is the
//         union of them, and isAtLeast compares their severity.
//
// The trap: `const LEVELS = ["debug", "info"]` has type string[], not
// ["debug", "info"] — `const` freezes the binding, `as const` freezes the type.

/**
 * TODO: the four log levels in ascending severity — debug, info, warn, error —
 * kept as exact literal types rather than widening to string[].
 */
export const LEVELS = [] as const;

/** TODO: the union of the four levels, derived from LEVELS rather than retyped. */
export type LogLevel = unknown;

/** True when `level` is at least as severe as `min`. */
export function isAtLeast(_level: LogLevel, _min: LogLevel): boolean {
  throw new Error("TODO: implement isAtLeast");
}
