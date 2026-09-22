// Exercise 014 — user-defined type guards (beginner).
// Goal:   teach the checker something it cannot work out on its own.
// Drills: the `value is T` return type, narrowing at the call site.
// Passes: both guards narrow their argument inside an `if`, not merely
//         return the right boolean.
//
// A guard returning plain `boolean` is useful at runtime and invisible to the
// checker: the value stays `unknown` inside the `if`. Changing the return
// type to `value is T` is what publishes the conclusion — and that is the
// whole exercise, so the stubs are declared `boolean` on purpose.
//
// The sharp edge: a predicate is a PROMISE, not a proof. TypeScript does not
// verify that the body actually establishes it, so `(v): v is User => true`
// compiles and lies. The narrowing is only as sound as the checks you write.

export interface User {
  id: string;
  email: string;
}

/** TODO: true when `value` is an object with string `id` and `email`. */
export function isUser(_value: unknown): boolean {
  throw new Error("TODO: implement isUser");
}

/** TODO: true when `value` is an array whose every element is a string. */
export function isStringArray(_value: unknown): boolean {
  throw new Error("TODO: implement isStringArray");
}
