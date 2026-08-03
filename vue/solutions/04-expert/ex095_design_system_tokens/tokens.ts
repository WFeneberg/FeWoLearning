// Exercise 095 — Design-system token layer (reference solution).
import { computed, inject, provide, type ComputedRef, type InjectionKey } from "vue";

export type TokenMap = Record<string, string>;

export const tokensKey = Symbol("design-tokens") as InjectionKey<ComputedRef<TokenMap>>;

export function provideTokens(tokens: TokenMap): ComputedRef<TokenMap> {
  // Read the parent layer *before* providing our own, so this stays the parent's
  // value and not our own (provide/inject resolves upwards from the parent).
  const parent = inject(tokensKey, undefined);

  // A computed keeps the merge lazy and reactive: spreading `tokens` reads its
  // keys, so mutating the object the caller handed us propagates to consumers.
  const merged = computed<TokenMap>(() => ({
    ...(parent?.value ?? {}),
    ...tokens,
  }));

  provide(tokensKey, merged);
  return merged;
}

export interface UseTokens {
  all: ComputedRef<TokenMap>;
  get: (path: string, fallback?: string) => string;
  cssVars: ComputedRef<Record<string, string>>;
}

export function useTokens(): UseTokens {
  const empty = computed<TokenMap>(() => ({}));
  const all = inject(tokensKey, empty);

  const get = (path: string, fallback = ""): string => all.value[path] ?? fallback;

  const cssVars = computed(() =>
    Object.fromEntries(
      // Dots are not legal in custom-property names, so "color.bg" becomes
      // "--color-bg".
      Object.entries(all.value).map(([key, value]) => [`--${key.replace(/\./g, "-")}`, value]),
    ),
  );

  return { all, get, cssVars };
}
