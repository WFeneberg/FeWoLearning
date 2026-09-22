// Exercise 075 — the value at a path, and reading it (advanced).
// Goal:   resolve a dotted path to the type it lands on, then write the
//         getter that matches.
// Drills: splitting a template literal with `infer`, recursing on the
//         remainder, the constraint that ties the two halves together.
// Passes: ValueAt answers per path, getByPath reports that same type, and
//         a path that does not exist is rejected.
//
// ex074 produced the paths; this resolves them. The pattern is a
// template literal taken apart from the left:
//
//   P extends `${infer Head}.${infer Rest}`
//
// Head is the first segment, Rest is everything after the first dot — and
// `infer` on a template literal is GREEDY FROM THE LEFT, so for
// "a.b.c" Head is "a" and Rest is "b.c", not "a.b" and "c". Recursing on
// Rest walks the path one segment per step.
//
// Two arms end the recursion: a P with no dot left, which is a plain key
// lookup, and a Head that is not a key of T, which is `never`.
//
// The payoff is the runtime function. Constraining P to the path union
// means a typo is a compile error at the call site rather than an
// undefined at three in the morning, and the return type follows the
// string that was passed.

export interface Order {
  id: string;
  customer: { name: string; email: string; address: { city: string; zip: number } };
  total: number;
}

/** TODO: every dotted path into T. Same shape as ex074; write it again
 *  rather than importing, since this row's constraint depends on it. */
export type Paths<T> = unknown;

/** TODO: the type found at dotted path P inside T, or never. */
export type ValueAt<T, P extends string> = unknown;

/** TODO: read the value at `path`. Constrain `path` so only a real one
 *  compiles, and report the type it lands on. */
export function getByPath(_source: Order, _path: string): unknown {
  throw new Error("TODO: implement getByPath");
}
