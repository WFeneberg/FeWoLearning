import { computed, Injectable, signal } from "@angular/core";

// Exercise 033 — untracked() (beginner).
// Goal:   read a signal without becoming dependent on it.
// Drills: untracked(), which signals a computed does and does not subscribe to, reading a
//         whole helper untracked, and the over-subscription bug untracked() exists to fix.
// Passes: when `npx jest exercises/01-beginner/ex033_untracked_read` is green.
//
// A computed subscribes to every signal it *reads while running*. That is usually what you
// want and occasionally a bug: a formatter that peeks at a render counter, a validator that
// reads the current user for a log message, a derived value that consults a feature flag it
// does not really depend on. Each of those peeks makes the computed re-run whenever the
// peeked-at signal changes, which at best wastes work and at worst causes a loop.
//
// untracked(() => …) runs its callback outside the tracking context, so the reads inside do
// not create dependencies. The value still comes back current — this is not a cached or
// stale read, it is a read that does not subscribe.
//
// The flip side is the trap: an untracked read will *not* refresh when that signal changes.
// If the result depends on it, the derived value silently goes stale. Untracked reads belong
// to things that are incidental to the result — counters, logs, diagnostics — not inputs to it.

@Injectable({ providedIn: "root" })
export class AuditStore {
  /** The value the summary genuinely depends on. */
  readonly amount = signal(100);

  /** Incidental: it changes constantly and must not drive recomputation. */
  readonly reads = signal(0);

  /** A feature flag the summary peeks at without subscribing. */
  readonly verbose = signal(false);

  /** Bumped every time `summary` actually re-runs. */
  recomputes = 0;

  /**
   * TODO: `"amount: <amount>"`, or `"amount: <amount> (read <n> times)"` when verbose.
   *
   * Increment `recomputes` on each run. Depend on `amount` only: read both `reads` and
   * `verbose` through untracked(), so neither one can trigger a re-run.
   */
  readonly summary = computed<string>(() => {
    throw new Error("TODO: implement the summary computed");
  });

  /**
   * TODO: the same string, built by subscribing to everything.
   *
   * Read all three signals normally. This is the version that re-runs when `reads` changes,
   * which is the behaviour the spec contrasts against.
   */
  readonly eagerSummary = computed<string>(() => {
    throw new Error("TODO: implement the eagerSummary computed");
  });

  /** Bumped every time `eagerSummary` re-runs. */
  eagerRecomputes = 0;

  /** Note a read: bump `reads`. Nothing that depends only on `amount` should notice. */
  noteRead(): void {
    throw new Error("TODO: implement noteRead");
  }

  /**
   * The current `reads` value, read without subscribing.
   *
   * TODO: wrap the read in untracked() *here*, so any caller is protected rather than
   * having to remember. untracked() works wherever it is called, not just inline.
   */
  currentReadsUntracked(): number {
    throw new Error("TODO: implement currentReadsUntracked");
  }

  /**
   * TODO: a computed that calls `currentReadsUntracked()` and returns `reads * 2`.
   *
   * It must not subscribe to `reads` — proof that untracked() protects a caller from inside
   * a helper, not only when written inline in the computed itself.
   */
  readonly doubledReads = computed<number>(() => {
    throw new Error("TODO: implement the doubledReads computed");
  });
}
