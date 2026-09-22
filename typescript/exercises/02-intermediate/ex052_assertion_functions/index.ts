// Exercise 052 — assertion functions (intermediate).
// Goal:   narrow a value by throwing rather than by branching.
// Drills: the `asserts x is T` return type, asserting non-null, asserting
//         a tuple shape.
// Passes: after each call the caller's value is narrowed, which a plain
//         `void` return cannot do.
//
// A type predicate (ex014) answers a question; an assertion function makes
// a promise: return normally and the value IS that type from here on. The
// return type is spelled `asserts value is T`, and the narrowing applies to
// the code AFTER the call, with no `if` anywhere.
//
// Two restrictions that have no obvious reason until you hit them. An
// assertion function called through a VARIABLE needs that variable to carry
// an explicit type annotation — `const check = assertString;` then
// `check(v)` narrows nothing, because the assertion signature does not
// survive inference. And the same goes for an arrow function: you cannot
// write `const assertString = (v: unknown): asserts v is string => {…}`
// without annotating the const, so these are declared as functions.
//
// Like a predicate, an assertion is UNCHECKED: TypeScript does not verify
// that the body establishes what it claims. An empty body compiles.

/** TODO: throw a TypeError unless `value` is a string — and narrow it. */
export function assertString(_value: unknown): void {
  throw new Error("TODO: implement assertString");
}

/** TODO: throw a TypeError when `value` is null or undefined — and narrow
 *  it to whatever it was without them. */
export function assertDefined<T>(_value: T | null | undefined): void {
  throw new Error("TODO: implement assertDefined");
}

/** TODO: throw a RangeError when `items` is empty — and narrow it to a
 *  tuple with at least one element, so items[0] is no longer undefined. */
export function assertNonEmpty<T>(_items: readonly T[]): void {
  throw new Error("TODO: implement assertNonEmpty");
}
