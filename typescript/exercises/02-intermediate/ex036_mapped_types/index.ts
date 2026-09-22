// Exercise 036 — mapped types (intermediate).
// Goal:   build a new object type by walking the keys of an existing one.
// Drills: `{ [K in keyof T]: … }`, transforming the value type per key.
// Passes: Stringify and Boxed rebuild User with every value replaced, and
//         stringifyAll produces one at runtime.
//
// A mapped type is a for-loop over keys, evaluated by the compiler. `K in
// keyof T` binds each key in turn, and the type after the colon is the new
// value type — with K in scope, so `T[K]` is that key's original type.
//
// This has no C# counterpart at all. The nearest thing is a source
// generator, and the difference is that this one runs in the type checker,
// composes with everything else, and emits nothing.
//
// Two conveniences to notice in the solutions: `keyof T` is the union of
// T's keys (ex018), and `T[K]` is indexed access (ex020). Mapped types are
// where those two start earning their keep.

export interface User {
  id: string;
  age: number;
  active: boolean;
}

export interface Box<V> {
  value: V;
}

/** TODO: the same keys as T, every value typed string. */
export type Stringify<T> = unknown;

/** TODO: the same keys as T, every value wrapped in a Box of its own type. */
export type Boxed<T> = unknown;

/** TODO: every field of `user` rendered with String(). */
export function stringifyAll(_user: User): Stringify<User> {
  throw new Error("TODO: implement stringifyAll");
}
