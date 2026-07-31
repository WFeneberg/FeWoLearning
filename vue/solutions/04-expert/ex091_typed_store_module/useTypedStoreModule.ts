// Exercise 091 — useTypedStoreModule composable (reference solution).
import { computed, reactive, type ComputedRef } from "vue";

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
>(config: StoreModuleConfig<S, G, A>): () => StoreModule<S, G, A> {
  let instance: StoreModule<S, G, A> | null = null;

  return function useStore(): StoreModule<S, G, A> {
    if (instance) return instance;

    const state = reactive(config.state()) as S;

    const getters = {} as BoundGetters<S, G>;
    for (const key of Object.keys(config.getters) as Array<keyof G>) {
      const getter = config.getters[key];
      getters[key] = computed(() => getter(state)) as BoundGetters<S, G>[typeof key];
    }

    const actions = {} as BoundActions<S, A>;
    for (const key of Object.keys(config.actions) as Array<keyof A>) {
      const def = config.actions[key];
      const bound = (payload: unknown): void => {
        if (!def.guard(payload)) {
          throw new TypeError(`Invalid payload for action "${String(key)}"`);
        }
        def.handler(state, payload);
      };
      actions[key] = bound as BoundActions<S, A>[typeof key];
    }

    instance = { state, getters, actions };
    return instance;
  };
}
