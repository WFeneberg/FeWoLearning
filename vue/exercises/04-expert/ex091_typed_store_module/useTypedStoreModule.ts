// Exercise 091 — useTypedStoreModule composable (expert).
// Goal:   a fully-typed store module factory (state/getters/actions) that is
//         usable on its own, independent of any Pinia internals: a `defineStoreModule`
//         call returns a `useXxxStore()` hook exposing reactive state, computed
//         getters, and action functions whose payload types are inferred from
//         each action's own definition via generics.
// Drills: generics across multiple type parameters, mapped types, conditional
//         (infer) types, reactive()/computed(), runtime shape-guards standing
//         in for what TypeScript's static checking would reject at compile time.
import { type ComputedRef } from "vue";

/** A single typed action: a runtime shape-guard plus the state mutator it guards. */
export interface ActionDef<S, P> {
  guard: (payload: unknown) => payload is P;
  handler: (state: S, payload: P) => void;
}

export type ActionDefs<S> = Record<string, ActionDef<S, any>>;

export type GetterDefs<S> = Record<string, (state: S) => unknown>;

export interface StoreModuleConfig<
  S extends object,
  G extends GetterDefs<S>,
  A extends ActionDefs<S>,
> {
  state: () => S;
  getters: G;
  actions: A;
}

/** Each getter becomes a read-only computed ref of its declared return type. */
export type BoundGetters<S, G extends GetterDefs<S>> = {
  [K in keyof G]: ComputedRef<ReturnType<G[K]>>;
};

/** Each action becomes a function taking exactly the payload type it declared. */
export type BoundActions<S, A extends ActionDefs<S>> = {
  [K in keyof A]: A[K] extends ActionDef<S, infer P> ? (payload: P) => void : never;
};

export interface StoreModule<
  S extends object,
  G extends GetterDefs<S>,
  A extends ActionDefs<S>,
> {
  state: S;
  getters: BoundGetters<S, G>;
  actions: BoundActions<S, A>;
}

/**
 * Defines a typed store module and returns a `useStore()` hook. The hook
 * memoizes a single reactive instance per `defineStoreModule` call (Pinia-style
 * singleton), so every consumer of the same hook shares the same state.
 */
export function defineStoreModule<
  S extends object,
  G extends GetterDefs<S>,
  A extends ActionDefs<S>,
>(_config: StoreModuleConfig<S, G, A>): () => StoreModule<S, G, A> {
  throw new Error("TODO: implement defineStoreModule");
}
