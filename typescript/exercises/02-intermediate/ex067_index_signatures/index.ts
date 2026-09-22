// Exercise 067 — index signatures, and how they differ from Record
// (intermediate).
// Goal:   describe an object whose keys are not known in advance, and know
//         what you gave up.
// Drills: `[key: string]: V`, keyof on one, reads under
//         noUncheckedIndexedAccess, and the exactness a literal Record
//         keeps.
// Passes: the key type is the surprising one, reads admit undefined, and
//         the two open/closed forms disagree where they should.
//
// `{ [key: string]: V }` says any string key maps to a V. Two consequences
// that catch people:
//
// `keyof` on it is `string | number`, NOT `string`. Numeric keys are
// coerced to strings at runtime, so a string index signature accepts
// `bag[0]` and keyof has to say so.
//
// And every read is a guess. Under this track's noUncheckedIndexedAccess
// (ex007) `bag[key]` is `V | undefined`, because the signature promises
// what a key maps to IF it is there, and nothing about whether it is.
//
// Record is the other shape, and the difference is only visible with a
// LITERAL key union: `Record<"a" | "b", V>` is closed — exactly those two
// keys, both required, an unknown one rejected — while
// `Record<string, V>` is an index signature again (ex043). Reach for the
// index signature when the keys are genuinely open, and for the literal
// Record when they are not, because only the closed one catches a typo.
//
// readFrom carries no return annotation: the graded type is the one your
// body produces.

export interface Bag {
  [key: string]: number;
}

/** TODO: the key type of Bag. Derive it; the answer is not just string. */
export type BagKeys = unknown;

/** TODO: read `key` out of `bag`, admitting that it might not be there. */
export function readFrom(_bag: Bag, _key: string) {
  throw new Error("TODO: implement readFrom");
}

/** TODO: count how often each word appears. */
export function countWords(_text: string): Bag {
  throw new Error("TODO: implement countWords");
}
