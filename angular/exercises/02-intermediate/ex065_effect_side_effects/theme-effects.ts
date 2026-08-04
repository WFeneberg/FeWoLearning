import { Injectable, signal } from "@angular/core";

// Exercise 065 — effect() (intermediate).
// Goal:   run a side effect when a signal changes, and clean up after the previous run.
// Drills: effect(), automatic dependency tracking, the onCleanup callback, manual .destroy(),
//         the injection-context requirement, and why an effect must not be used to derive state.
// Passes: when `npx jest exercises/02-intermediate/ex065_effect_side_effects` is green.
//
// An effect is for reaching *outside* the signal graph: writing to localStorage, setting
// document.title, logging, talking to a non-signal API. It tracks whatever it reads, exactly like a
// computed, and re-runs when any of that changes.
//
// The rule that keeps the graph sane: an effect must not be how you derive state. If the output is
// a value other code reads, it is a computed — which is lazy, memoised, and cannot desynchronise.
// An effect that writes to another signal is a manual, eager, order-dependent computed, and it is
// the main way people build unpredictable signal code. Angular used to require allowSignalWrites
// to do it at all, which was the framework saying "are you sure".
//
// The onCleanup callback runs before each re-run and once more on destruction. That is what makes
// an effect safe for anything with a lifetime — a timer, a listener, a subscription — because the
// previous one is always torn down before the next one starts.
//
// Effects are scheduled, not synchronous. They run during change detection, which is why the spec
// calls TestBed.flushEffects() rather than expecting an immediate result.

@Injectable({ providedIn: "root" })
export class ThemeStore {
  readonly theme = signal<"light" | "dark">("light");

  readonly fontSize = signal(14);

  /** A stand-in for anything outside the signal graph. */
  readonly written: string[] = [];

  /** Cleanups that have run, in order. */
  readonly cleanups: string[] = [];

  /**
   * TODO: an effect that records `"theme:<value>"` into `written` whenever the theme changes.
   *
   * Called from an injection context by the spec. Return the EffectRef so it can be destroyed.
   */
  watchTheme(): unknown {
    throw new Error("TODO: implement watchTheme");
  }

  /**
   * TODO: an effect that reads *both* theme and fontSize and records
   * `"both:<theme>/<size>"`.
   *
   * Proof that dependency tracking is automatic — no dependency list anywhere.
   */
  watchBoth(): unknown {
    throw new Error("TODO: implement watchBoth");
  }

  /**
   * TODO: an effect with a cleanup.
   *
   * Records `"open:<theme>"` on each run, and registers a cleanup recording `"close:<theme>"`
   * with the theme value *from that run*. The cleanup must run before the next run and once on
   * destruction.
   */
  watchWithCleanup(): unknown {
    throw new Error("TODO: implement watchWithCleanup");
  }

  /**
   * TODO: an effect that reads the theme but ignores the font size, even though it uses it.
   *
   * Records `"themeOnly:<theme>/<size>"`. Read fontSize through untracked() (exercise 033) so a
   * size change does not re-run it.
   */
  watchThemeIgnoringSize(): unknown {
    throw new Error("TODO: implement watchThemeIgnoringSize");
  }
}
