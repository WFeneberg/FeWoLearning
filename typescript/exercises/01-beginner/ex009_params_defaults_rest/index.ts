// Exercise 009 — optional parameters, defaults, rest parameters (beginner).
// Goal:   give a parameter a default, and see what that changes besides the
//         obvious.
// Drills: default parameter values, rest parameters, Function.length.
// Passes: css exposes (value, unit?, ...extras), defaults unit to "px", and
//         labelAll collects any number of items.
//
// A default does two things at once: it supplies the value, and it makes the
// parameter optional in the signature TypeScript exposes. A `??` in the body
// does only the first — and leaves the parameter counted in Function.length.
// That is the difference the .length fact grades, because the returned
// strings alone cannot tell the two mechanisms apart.
//
// `extras` is already declared as a rest parameter rather than left to you:
// a test that calls css(4, "em", "!important") cannot compile against a stub
// whose signature does not accept it yet, so the change would show up as a
// type error in the test file instead of a failing fact.

/**
 * TODO: default `unit` to "px", and return the value and unit joined to the
 * extras by spaces — css(4, "em", "!important") is `4em !important`.
 */
export function css(_value: number, _unit: string, ..._extras: string[]): string {
  throw new Error("TODO: implement css");
}

/** Prefixes every item — labelAll("a:", "x", "y") is ["a:x", "a:y"]. */
export function labelAll(_prefix: string, ..._items: string[]): string[] {
  throw new Error("TODO: implement labelAll");
}
