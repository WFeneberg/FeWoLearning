import { Component, computed, input } from "@angular/core";

// Exercise 094 — SSR-safe rendering: hydration-stable initial state (expert).
// Goal:   make a component's FIRST render fully deterministic given its inputs — the property an
//         Angular Universal server render and the client's hydration pass both depend on. If the
//         two renders don't produce byte-identical output, Angular flags a hydration mismatch and
//         re-renders the whole subtree from scratch, throwing away everything the server did.
// Drills: deriving all displayed state from `input()` values ONLY — never `Date.now()`, `new
//         Date()` called with no arguments, `Math.random()`, or any other "ambient", per-call read —
//         and a plain deterministic hash function as the pattern for anything that looks like it
//         needs randomness but must not have any.
// Passes: when `npx jest exercises/04-expert/ex094_ssr_hydration_component` is green.
//
// This repository has no real Angular Universal server to render against, so the test below
// stands in for "server renders once, client hydrates once" the only way a plain Jest/jsdom suite
// can: it constructs the SAME component TWICE from the SAME inputs and asserts the two renders are
// identical. That is a real simplification of the actual guarantee (a genuine SSR pipeline also has
// to survive serialization to a string and back), but the property under test — same inputs in,
// same output out, no per-call ambient state involved — is exactly the property that makes real
// hydration succeed or fail.
//
// `new Date(startedAt())` — parsing an ISO string the caller handed in — is fine: given the same
// string it always produces the same instant. `new Date()` / `Date.now()` with NO arguments is the
// forbidden move, because it reads the wall clock at the moment of the call — the server's "now"
// and the client's "now" (however many milliseconds later hydration actually runs) are never the
// same instant, so a component built on it renders two different strings and Angular flags a
// hydration mismatch.

/**
 * TODO: implement stableSessionKey — a pure, deterministic function of its two string arguments.
 *   - Concatenate them as `${sessionId}|${startedAt}`.
 *   - Sum the char codes (`charCodeAt`) of every character in that combined string.
 *   - Return `sum % 997`, formatted as a zero-padded 3-digit string (e.g. 7 -> "007", 42 -> "042").
 *   - Must not use Math.random(), Date.now(), or any other non-deterministic source — the same two
 *     arguments must always produce the same output, in this call or any other, in this process or
 *     any other.
 */
export function stableSessionKey(sessionId: string, startedAt: string): string {
  throw new Error("TODO: implement stableSessionKey");
}

@Component({
  selector: "app-session-banner",
  standalone: true,
  template: `<p class="banner">Session {{ sessionKey() }} started {{ startedAtLabel() }}</p>`,
})
export class SessionBannerComponent {
  readonly sessionId = input.required<string>();

  /**
   * The instant this session started, as an ISO-8601 string — handed down by whatever produced
   * this component (a server render, or the client re-hydrating from the server's serialized
   * state). Never call `new Date()`/`Date.now()` inside this component to obtain "now" — that
   * value MUST arrive from outside, or the server and client renders will disagree.
   */
  readonly startedAt = input.required<string>();

  /**
   * TODO: computed() deriving "HH:MM UTC" from startedAt() alone, e.g.
   * "2026-03-01T14:05:00.000Z" -> "14:05 UTC". Use `new Date(startedAt())` (parsing the given
   * string is fine — see the header comment) and `getUTCHours()`/`getUTCMinutes()`, each zero-padded
   * to 2 digits.
   */
  readonly startedAtLabel = computed((): string => {
    throw new Error("TODO: implement startedAtLabel");
  });

  /** TODO: computed() wrapping stableSessionKey(sessionId(), startedAt()). */
  readonly sessionKey = computed((): string => {
    throw new Error("TODO: implement sessionKey");
  });
}
