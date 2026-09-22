// Exercise 035 — validating at the boundary (beginner).
// Goal:   turn untrusted text into a value the rest of the program can
//         trust, without lying to the checker.
// Drills: JSON.parse returning any, a type predicate over an unknown shape,
//         rejecting what does not fit.
// Passes: isConfig narrows, and parseConfig accepts good input and rejects
//         every broken shape.
//
// JSON.parse is declared to return `any`, so `JSON.parse(text) as Config` is
// two lies stacked: the parse claims to know nothing, and the cast claims to
// know everything. Neither is checked, and the program fails later, far from
// here, when a field is missing.
//
// This is the boundary pattern, and it is the one place the work is
// genuinely worth doing by hand before reaching for a schema library: parse
// to `unknown`, then narrow with a predicate that actually inspects. What
// makes the facts below grade the mechanism rather than the happy path is
// the broken inputs — a cast passes every valid case and fails all of those.
//
// Note that the array check has to look at the ELEMENTS. `Array.isArray`
// alone says nothing about what is in it.

export interface Config {
  name: string;
  port: number;
  tags: string[];
}

/** TODO: true when `value` is a Config — and narrow it for the caller. */
export function isConfig(_value: unknown): boolean {
  throw new Error("TODO: implement isConfig");
}

/**
 * TODO: parse `text` and return it as a Config, or undefined when the text
 * is not valid JSON or does not describe one. Do not cast your way there.
 */
export function parseConfig(_text: string): Config | undefined {
  throw new Error("TODO: implement parseConfig");
}
