// Exercise 091 — Split, at the type level (expert).
// Goal:   take a string type apart on a delimiter and give back a tuple
//         of string types.
// Drills: recursive template-literal matching, spreading a recursive
//         call into a tuple, the base case that keeps the remainder.
// Passes: every splitting case a runtime split handles, including the
//         empty pieces.
//
// The pattern is ex075's, applied to a delimiter instead of a dot:
//
//   S extends `${infer Head}${D}${infer Rest}` ? [Head, ...Split<Rest, D>] : [S]
//
// Three things decide whether it is correct.
//
// The recursive call is SPREAD into the tuple, not nested: `[Head,
// ...Split<Rest, D>]` builds one flat tuple, while `[Head, Split<Rest,
// D>]` builds a tree.
//
// The base case is `[S]`, not `[]`. When no delimiter is left, the whole
// remainder is the last piece — and for `Split<"", ",">` that means
// `[""]`, matching what runtime split does with an empty string.
//
// And inference on a template literal is greedy FROM THE LEFT (ex075),
// which is what makes each step peel exactly one piece off the front.
// It is also why a multi-character delimiter works with no extra care:
// the pattern matches the whole of D.
//
// Join is the inverse, and the pair is worth having together: a correct
// Split composed with a correct Join is the identity on any string that
// does not end in the delimiter.

/** TODO: the pieces of S, split on every occurrence of D. */
export type Split<S extends string, D extends string> = unknown;

/** TODO: the pieces of T joined by D. */
export type Join<T extends readonly string[], D extends string> = unknown;

/** TODO: the runtime half of Split, reporting the same tuple type. */
export function split<S extends string, D extends string>(
  _text: S,
  _delimiter: D,
): Split<S, D> {
  throw new Error("TODO: implement split");
}
