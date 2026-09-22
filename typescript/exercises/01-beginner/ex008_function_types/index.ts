// Exercise 008 — function types, and what `void` really means (beginner).
// Goal:   see why a void-returning parameter accepts a function that returns
//         something, and why `undefined` does not.
// Drills: call signatures, void vs undefined as a return type.
// Passes: both call signatures are declared, and the assignability pair
//         resolves to [true, false].
//
// `void` as a return type does not mean "returns undefined". It means "the
// caller will not look at the result", so ANY function is assignable as long
// as its parameters fit. That is what makes `lines.forEach(l => list.push(l))`
// legal even though push returns a number. Declaring `=> undefined` instead
// says the opposite: the result is inspected, and it had better be undefined.

/** TODO: takes a line of text, returns nothing the caller will look at. */
export type Callback = unknown;

/** TODO: takes a line of text, and genuinely returns undefined. */
export type UndefinedCallback = unknown;

/** Calls `onLine` once per newline-separated line of `text`. */
export function forEachLine(_text: string, _onLine: Callback): void {
  throw new Error("TODO: implement forEachLine");
}
