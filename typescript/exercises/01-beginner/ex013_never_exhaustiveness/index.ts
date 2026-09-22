// Exercise 013 — `never`, and turning a missed case into a build error
// (beginner).
// Goal:   write the helper that makes an unhandled union member fail to
//         compile rather than fail in production.
// Drills: never as the empty type, exhaustive switch, assertNever.
// Passes: area handles both shapes, and assertNever refuses any argument
//         that is not already narrowed away to never.
//
// `never` is the type with no values. Nothing is assignable TO it, which is
// why a parameter of type never can only be passed something the checker has
// already narrowed out of existence — and that is the entire trick: once the
// switch below handles every kind, the default branch's `shape` is never, and
// the call compiles. Add a third kind to the union and it stops compiling, at
// the exact line that would otherwise have silently returned undefined.
//
// Note the stub's parameter is `unknown`, not `never` — narrowing it is your
// job, and it is what the type fact grades.

export type Shape =
  | { kind: "circle"; radius: number }
  | { kind: "square"; side: number };

/** Given, to show what a later case looks like. Do not change. */
export type ShapeV2 = Shape | { kind: "triangle"; base: number; height: number };

/**
 * TODO: accept only a value the checker has narrowed away to nothing, and
 * throw an Error whose message starts with "Unexpected value:" followed by
 * the value as JSON.
 */
export function assertNever(_value: unknown): never {
  throw new Error("TODO: implement assertNever");
}

/** The area — PI * r^2 for a circle, side^2 for a square. */
export function area(_shape: Shape): number {
  throw new Error("TODO: implement area");
}
