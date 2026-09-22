// Exercise 002 — `type` alias vs `interface` (beginner).
// Goal:   find the two places where the choice is actually observable.
// Drills: interface declaration merging, aliasing a union.
// Passes: Config carries both properties although it is declared twice, and
//         Id is a union — which an interface cannot express at all.
//
// The rule of thumb "they are interchangeable" holds right up to these two
// cases: only an interface merges, and only an alias can name a union, a
// primitive or a tuple.

/**
 * TODO: add a SECOND `interface Config` declaration below this one that
 * contributes `port: number`. Do not edit the first declaration, and do not
 * merge the two by hand — the point is that TypeScript merges them for you.
 */
export interface Config {
  host: string;
}

/** TODO: an identifier that is either a string or a number. */
export type Id = unknown;

/** Returns `host:port`, e.g. `localhost:8080`. */
export function formatConfig(_config: Config): string {
  throw new Error("TODO: implement formatConfig");
}
