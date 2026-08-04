import { Component } from "@angular/core";
import { BehaviorSubject, Observable, Subject } from "rxjs";

// Exercise 055 — AsyncPipe (intermediate).
// Goal:   render an observable from a template without subscribing by hand.
// Drills: | async, the null it yields before the first emission, automatic unsubscription, the
//         `as` alias, and the double-subscription trap of using | async twice on one source.
// Passes: when `npx jest exercises/02-intermediate/ex055_async_pipe_template` is green.
//
// AsyncPipe subscribes for you and — the part that matters — unsubscribes when the component is
// destroyed. That removes the whole class of leak exercise 022 was about, which is why template
// subscription is preferred over a manual one in ngOnInit.
//
// Its return value before the first emission is `null`, not undefined and not the source's type.
// So `{{ user$ | async }}` renders empty at first, and `(user$ | async).name` is a type error for
// good reason. The idiom is `@if (user$ | async; as user)`, which both waits and gives the block
// a non-null binding.
//
// The trap worth meeting properly: each `| async` is its own subscription. Writing
// `{{ total$ | async }}` in two places subscribes twice, and over a cold HTTP observable that is
// two requests. The fix is one `@if (... ; as x)` wrapping both uses, or shareReplay on the source.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <p class="count">{{ count$ | async }}</p>
//
//   @if (title$ | async; as title) {
//     <h2 class="title">{{ title }}</h2>
//   } @else {
//     <p class="waiting">waiting</p>
//   }
//
//   <!-- Deliberately twice, so the spec can count the subscriptions. -->
//   <p class="twice-a">{{ tracked$ | async }}</p>
//   <p class="twice-b">{{ tracked$ | async }}</p>
//
//   <!-- And once, sharing a single subscription across both readings. -->
//   @if (shared$ | async; as value) {
//     <p class="shared-a">{{ value }}</p>
//     <p class="shared-b">{{ value }}</p>
//   }

@Component({
  selector: "app-feed",
  standalone: true,
  // TODO: import AsyncPipe.
  template: `<p>TODO: render the feed — see the template contract above</p>`,
})
export class FeedComponent {
  /** Has a value from the start, so | async renders it on the first pass. */
  readonly count$ = new BehaviorSubject(0);

  /** Has no value yet, so | async yields null until something arrives. */
  readonly title$ = new Subject<string>();

  /** How many times something subscribed to `tracked$`. */
  trackedSubscriptions = 0;

  /** How many times something subscribed to `shared$`. */
  sharedSubscriptions = 0;

  /**
   * TODO: an observable that emits "tracked" to each subscriber and counts the subscriptions.
   *
   * Build it with `new Observable(subscriber => …)`, incrementing `trackedSubscriptions` inside
   * the subscribe function — that function runs once per subscriber, which is the point.
   */
  readonly tracked$: Observable<string> = new Observable<string>();

  /**
   * TODO: the same, counting into `sharedSubscriptions`.
   *
   * Used once in the template, so the count stays at one however many times its value is read.
   */
  readonly shared$: Observable<string> = new Observable<string>();

  /** Push a new count. */
  setCount(value: number): void {
    throw new Error("TODO: implement setCount");
  }

  /** Push a title, which is what lets the @if branch flip. */
  setTitle(value: string): void {
    throw new Error("TODO: implement setTitle");
  }
}
