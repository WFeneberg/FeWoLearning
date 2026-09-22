// Exercise 038 — remapping keys with `as` (intermediate).
// Goal:   choose and rename the keys a mapped type produces, not just
//         their values.
// Drills: the `as` clause, dropping a key by mapping it to `never`.
// Passes: MethodsOf and DataOf split Api in two, Prefixed renames every
//         key, and dataOnly produces the data half at runtime.
//
// `{ [K in keyof T as <new key>]: … }` computes the OUTPUT key. Two things
// follow, and they are the whole row.
//
// Mapping a key to `never` DROPS it. That is not a special case bolted on;
// `never` is the empty union, so contributing it contributes no key at all.
// A conditional in the `as` clause therefore acts as a filter.
//
// And mapping it to a different string renames it. Prefixed below uses a
// template literal type to do that, which ex047 covers properly — take it
// on trust here, or read ahead.
//
// One trap worth knowing: `Function` as a constraint matches every function
// but is otherwise a poor type. The idiomatic spelling for "some function"
// is `(...args: never[]) => unknown`, which is what the solution uses.

export interface Api {
  getUser(): string;
  getPost(): string;
  id: number;
  label: string;
}

/** TODO: only the function-valued properties of T. */
export type MethodsOf<T> = unknown;

/** TODO: only the properties of T that are NOT functions. */
export type DataOf<T> = unknown;

/** TODO: every key of T renamed with a `raw_` prefix, values unchanged. */
export type Prefixed<T> = unknown;

/** TODO: a copy of `api` carrying only its data properties. */
export function dataOnly(_api: Api): DataOf<Api> {
  throw new Error("TODO: implement dataOnly");
}
