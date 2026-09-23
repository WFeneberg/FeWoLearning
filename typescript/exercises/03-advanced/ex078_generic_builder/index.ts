// Exercise 078 — a builder that knows what it still needs (advanced).
// Goal:   accumulate the set of supplied keys in the type, and refuse to
//         build until it is complete.
// Drills: a type parameter threaded through a fluent chain, intersection
//         as accumulation, a `this` parameter as a precondition.
// Passes: each `set` widens the accumulated type, and `build` is callable
//         only once every required key is present.
//
// A fluent builder normally gives up on types: every method returns the
// same builder, and `build()` either returns a Partial or throws at
// runtime. Threading a type parameter fixes both. `set` returns
// `Builder<T & Record<K, V>>`, so the accumulated type grows one key per
// call, and the chain remembers.
//
// `build` is the interesting half. It is declared with a THIS PARAMETER
// (ex062) — `build(this: Builder<Config>): Config` — which is not about
// receivers at all here: it is a PRECONDITION. The method exists on every
// builder, and calling it type-checks only on one whose accumulated T is
// assignable to Config. A builder missing `port` is simply not a valid
// receiver, and the error lands on the `.build()` call.
//
// That is the trick worth taking away. A `this` parameter can express
// "this method is only available in this state" for any state you can
// name in the type.
//
// Note the value constraint: `V extends Config[K]` ties the value to the
// key that was passed, exactly as ex049's setIn did.
//
// MEASURED, and without it the `this` trick silently does nothing: T has
// to appear somewhere the checker can COMPARE. With T used only inside
// `set`'s return type, `Builder<{host, port}>` and `Builder<Config>` are
// structurally identical and every incomplete builder passes the `this`
// check. The phantom `supplied` property below puts T in a plain
// covariant position, which is what makes the comparison bite. It is
// optional and never assigned — the same phantom-property trick as
// ex076's brand, used for a different purpose.

export interface Config {
  host: string;
  port: number;
  secure: boolean;
}

/** TODO: a builder accumulating the keys supplied so far in T. */
export interface Builder<T> {
  /** Phantom. Never assigned; it exists so that T is comparable. */
  readonly supplied?: T;

  /** TODO: record one key, widening T. Introduce the two type parameters
   *  yourself — the stub's signature accepts any key and any value, which
   *  is exactly what needs tightening. */
  set(key: string, value: unknown): Builder<unknown>;
  /** TODO: produce the Config — but only from a builder that has all of
   *  it. Say so with a `this` parameter. */
  build(): Config;

}

/** TODO: an empty builder. */
export function configBuilder(): Builder<unknown> {
  throw new Error("TODO: implement configBuilder");
}
