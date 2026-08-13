import { Component, resource, signal } from "@angular/core";

// Exercise 086 — resource(): async loading as signal state (advanced).
// Goal:   drive a piece of UI off an async load without ever touching a Subscription or an
//         AsyncPipe — the loading/error/value states are just signals, read like any other.
// Drills: `resource({ params, loader })`, its returned `.value()` / `.status()` / `.error()` /
//         `.isLoading()` / `.hasValue()` signals, and re-triggering the loader by changing the
//         reactive `params`.
// Passes: when `npx jest exercises/03-advanced/ex086_resource_async_signal` is green.
//
// `resource()` is `computed()`'s async counterpart: instead of deriving a value synchronously from
// other signals, it derives a *pending* value from them, by handing whatever `params()` currently
// reads to `loader`. Every signal `params()` reads becomes a dependency the same way it would for a
// `computed()` — change `userId` here, and `resource()` notices, cancels any in-flight load for the
// old id, and calls `loader` again with the new one. Nothing about wiring that up is manual, the
// same way nothing about a `computed()` re-running is manual.
//
// The loader itself is deliberately a plain `async function` with no `setTimeout`, no real network —
// it settles on the microtask queue and nothing else, which keeps it deterministic. That said,
// `resource()` schedules its loader call through framework machinery that exercise 083's
// `fakeAsync`/`tick()`/`flushMicrotasks()` do not reliably observe, so the spec instead calls
// `fixture.detectChanges()` (to let the resource's reactive graph run at all) and awaits
// `fixture.whenStable()` (the framework's own way of waiting for pending async work, resources
// included, to settle) — no manual promise or timer bookkeeping required.
//
// `.value()` is only meaningful once `.hasValue()` is true — reading it while still loading returns
// whatever `defaultValue` was configured (here, none was, so it is `undefined`), and reading it in
// the `error` state throws. The template below never touches `.value()` without checking
// `.hasValue()` first, for exactly that reason.

export interface UserRecord {
  readonly id: number;
  readonly name: string;
}

/**
 * TODO: implement fetchUser — resolve to `{ id, name: `User ${id}` }` for id > 0, and reject with
 * `new Error(`No user with id ${id}`)` for id <= 0. Keep this deterministic and timer-free: no
 * setTimeout, no real I/O — just resolve or reject the returned promise directly.
 */
async function fetchUser(id: number): Promise<UserRecord> {
  throw new Error("TODO: implement fetchUser");
}

@Component({
  selector: "app-user-profile",
  standalone: true,
  template: `
    <button type="button" class="next" (click)="nextUser()">Next user</button>
    @if (userResource.isLoading()) {
      <p class="loading">Loading…</p>
    } @else if (userResource.error()) {
      <p class="error">{{ userResource.error()?.message }}</p>
    } @else if (userResource.hasValue()) {
      <p class="name">{{ userResource.value().name }}</p>
    }
  `,
})
export class UserProfileComponent {
  readonly userId = signal(1);

  readonly userResource = resource({
    params: () => this.userId(),
    loader: ({ params }) => fetchUser(params),
  });

  /**
   * TODO: implement nextUser — advance userId by 1. Changing the signal `params` reads is all it
   * takes for `resource()` to re-run the loader; nothing else needs to be wired up by hand.
   */
  nextUser(): void {
    throw new Error("TODO: implement nextUser");
  }
}
