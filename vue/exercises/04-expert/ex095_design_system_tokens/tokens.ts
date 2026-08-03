// Exercise 095 — Design-system token layer (expert).
// Goal:   a provide/inject token layer for a design system. A provider publishes
//         a flat token map; consumers resolve dotted paths against it. Nested
//         providers *merge* over their parent rather than replacing it, so a
//         sub-tree can override "color.bg" while still seeing every other token.
//         Tokens also render as CSS custom properties so real styling can use them.
// Drills: InjectionKey typing, provide/inject with defaults, layered/merged
//         injection, computed derived from injected state, CSS custom properties.
import { type ComputedRef, type InjectionKey } from "vue";

export type TokenMap = Record<string, string>;

/** Typed injection key. Consumers outside any provider fall back to {}. */
export const tokensKey = Symbol("design-tokens") as InjectionKey<ComputedRef<TokenMap>>;

/**
 * Publishes `tokens`, merged over whatever a parent provider already published.
 * The child's own entries win. Call this in a provider component's setup.
 *
 * Returns the merged map so the provider can bind it as CSS custom properties.
 */
export function provideTokens(_tokens: TokenMap): ComputedRef<TokenMap> {
  throw new Error("TODO: implement provideTokens");
}

export interface UseTokens {
  /** All tokens visible at this point in the tree. */
  all: ComputedRef<TokenMap>;
  /** Resolves a token path, returning `fallback` when it is absent. */
  get: (path: string, fallback?: string) => string;
  /**
   * Renders the visible tokens as a CSS custom-property style object:
   * `{ "color.bg": "red" }` becomes `{ "--color-bg": "red" }` — dots become
   * dashes, because dots are not legal in custom-property names.
   */
  cssVars: ComputedRef<Record<string, string>>;
}

/** Reads the token layer. Safe to call with no provider above: `all` is empty. */
export function useTokens(): UseTokens {
  throw new Error("TODO: implement useTokens");
}
