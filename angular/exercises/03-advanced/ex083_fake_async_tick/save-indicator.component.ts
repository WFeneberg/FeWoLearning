import { Component, signal } from "@angular/core";

// Exercise 083 — fakeAsync / tick / flushMicrotasks: testing timers and promises deterministically
// (advanced).
// Goal:   assert on the state of an async operation at exact, chosen points in time, instead of
//         either sprinkling `await new Promise(...)` guesses through a test or making it flaky.
// Drills: `fakeAsync`, `tick(ms)` to advance virtual macrotask time (timers), and
//         `flushMicrotasks()` to drain the microtask queue (promises) without touching timers.
// Passes: when `npx jest exercises/03-advanced/ex083_fake_async_tick` is green.
//
// A real save operation usually involves two clocks that do not run at the same speed: a promise
// chain (microtasks — a `.then()` runs as soon as the current synchronous stack unwinds, no matter
// how "long" it conceptually is) and a timer (a macrotask — `setTimeout` genuinely does not run
// until its delay elapses). Outside a test, you cannot tell the two apart by looking at the code;
// inside `fakeAsync`, you can, because each is drained by a different function.
//
// `flushMicrotasks()` runs every pending `.then()`/`await` continuation to completion but leaves
// timers exactly where they were — nothing scheduled by `setTimeout`/`setInterval` fires. `tick(ms)`
// does the opposite kind of work: it advances the fake clock by `ms` and fires any timer whose delay
// has now elapsed, in order. Calling `tick(2000)` when only 1999ms have "elapsed" leaves a timer
// still pending; calling it again with the remaining 1ms fires it. Neither function is a superset of
// the other — a promise chain that depends on a timer resolving first needs both, in the right order.
//
// save() below starts both kinds of work in the same call: a microtask-only validation flag, and a
// macrotask-only save confirmation. The spec exploits that gap to prove the two really are
// independent — validated flips true after `flushMicrotasks()` alone, while state stays "saving"
// until `tick(2000)` actually elapses.

@Component({
  selector: "app-save-indicator",
  standalone: true,
  template: `
    <button type="button" class="save" (click)="save()">Save</button>
    <p class="state">{{ state() }}</p>
  `,
})
export class SaveIndicatorComponent {
  readonly state = signal<"idle" | "saving" | "saved">("idle");

  /** Flipped by a microtask (a resolved promise), independent of the save timer below. */
  readonly validated = signal(false);

  /**
   * TODO: implement save.
   *   1. Set `state` to "saving" and `validated` to false, synchronously.
   *   2. Schedule `Promise.resolve().then(...)` to set `validated` to true — a microtask, drained
   *      by `flushMicrotasks()` alone, with no timer involved.
   *   3. Schedule `setTimeout(..., 2000)` to set `state` to "saved" — a macrotask, which only fires
   *      once `tick()` has advanced the fake clock by at least 2000ms.
   */
  save(): void {
    throw new Error("TODO: implement save");
  }
}
