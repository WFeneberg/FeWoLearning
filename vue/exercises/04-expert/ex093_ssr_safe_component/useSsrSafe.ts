// Exercise 093 — SSR-safe component primitives (expert).
// Goal:   two composables that make a component safe to render on a server and
//         hydrate on a client without mismatches:
//           - useSsrSafeId(prefix) hands out ids that are deterministic, so the
//             markup the server produced matches what the client renders.
//           - useClientOnly(getter, fallback) exposes `fallback` during setup and
//             only calls `getter` once mounted, so browser-only APIs are never
//             touched while rendering.
// Drills: hydration mismatches, deterministic id generation, onMounted as the
//         client-only boundary, why Math.random/Date.now break SSR.
import { ref, type Ref } from "vue";

/**
 * Resets the id counter. Only needed so tests can start from a known state —
 * a real server would get a fresh module per request.
 */
export function resetIdCounter(): void {
  throw new Error("TODO: implement resetIdCounter");
}

/**
 * Returns the next id for `prefix`, counting from 1 per prefix:
 * `useSsrSafeId("field")` yields "field-1", then "field-2", …
 *
 * Must be deterministic: no Math.random, no Date.now, no crypto. Two runs that
 * make the same calls in the same order must produce the same ids, which is
 * exactly what hydration requires.
 */
export function useSsrSafeId(_prefix: string): string {
  throw new Error("TODO: implement useSsrSafeId");
}

/**
 * Returns a ref that holds `fallback` immediately and is updated to `getter()`
 * after the component mounts.
 *
 * `getter` must NOT be called during setup — that is the whole point: it is
 * allowed to read `window`, `document`, `localStorage` and friends, which do not
 * exist while server-rendering. Outside a component instance (no mount will ever
 * happen) the ref simply keeps the fallback.
 */
export function useClientOnly<T>(_getter: () => T, _fallback: T): Ref<T> {
  throw new Error("TODO: implement useClientOnly");
}
