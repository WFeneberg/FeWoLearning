import { ChangeDetectorRef, Component, inject, signal } from "@angular/core";

// Exercise 089 — zoneless change detection: signals as the one notification primitive (advanced).
// Goal:   make a component correct under `provideZonelessChangeDetection()`, where nothing patches
//         `setTimeout`/`Promise`/DOM events to trigger change detection for you automatically.
// Drills: `provideZonelessChangeDetection()`, why a signal write is enough on its own, and why a
//         plain mutable field still needs `ChangeDetectorRef.markForCheck()` even to be picked up
//         by a later *manual* `detectChanges()` call.
// Passes: when `npx jest exercises/03-advanced/ex089_zoneless_change_detection` is green.
//
// Zone.js's trick was patching every async API (`setTimeout`, `Promise.then`, DOM event listeners,
// XHR) so that *anything* asynchronous, anywhere, triggered a full change-detection pass when it
// returned — whether or not it touched something Angular could see change. `provideZonelessChangeDetection()`
// (this project's installed Angular no longer calls it "experimental" — the old
// `provideExperimentalZonelessChangeDetection` name is gone; check `node_modules/@angular/core`
// yourself next time an Angular upgrade lands, since exported names like this do shift release to
// release) removes that blanket patching. What is left to trigger a check is *notification*:
// a signal write, an `async` pipe emission, a template-bound event handler firing, or an explicit
// `markForCheck()`/`detectChanges()` call. Nothing else.
//
// That is not only a zoneless-mode fact in this Angular version — it is worth confirming for
// yourself directly, because it is easy to assume otherwise: even with the classic zone-based
// provider (no `provideZonelessChangeDetection()` at all) a *manually called* `fixture.detectChanges()`
// no longer blindly re-renders a Default-strategy component just because it was asked to. It still
// only refreshes state that was actually flagged — a signal write flags it for you for free, while a
// bare field mutation (`this.legacyCount++`) flags nothing at all unless you call `markForCheck()`
// yourself (exercise 076 drills that same lever for `OnPush`; here it applies even without `OnPush`).
// Angular's runtime has moved to notification-based scheduling everywhere, not just behind an
// explicit zoneless flag — `provideZonelessChangeDetection()` mainly then lets you *also* remove
// Zone.js's patching from the bundle, since the scheduling underneath no longer depends on it.
//
// `increment()` only has to write the signal; nothing else is required, and nothing else works
// better. `bumpLegacyCount()` exists to make the contrast concrete: implement it two ways in your
// head before you look at the solution, and predict which one a test can catch.

@Component({
  selector: "app-zoneless-counter",
  standalone: true,
  template: `
    <span class="count">{{ count() }}</span>
    <span class="legacy">{{ legacyCount }}</span>
    <button class="inc" (click)="increment()">+</button>
  `,
})
export class ZonelessCounterComponent {
  private readonly cdr = inject(ChangeDetectorRef);

  readonly count = signal(0);

  /** A plain field, deliberately not a signal — see `bumpLegacyCount` below. */
  legacyCount = 0;

  /**
   * TODO: implement increment — write the next value into the `count` signal. Do not read-then-set
   * through anything else; `count.update(...)` (or `.set(...)`) is the entire implementation.
   */
  increment(): void {
    throw new Error("TODO: implement increment");
  }

  /**
   * TODO: implement bumpLegacyCount — increment `legacyCount` (a plain field, not a signal), and
   * tell Angular this view may need checking. Skipping the second half compiles fine and even
   * "looks" done, but the count will never reach the DOM.
   */
  bumpLegacyCount(): void {
    throw new Error("TODO: implement bumpLegacyCount");
  }
}
