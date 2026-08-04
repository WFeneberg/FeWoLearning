import { Component, signal } from "@angular/core";

// Exercise 003 — StepCounterComponent (beginner).
// Goal:   a counter that moves by a configurable step, held entirely in signals.
// Drills: signal(), reading a signal by *calling* it, set() vs update(), and the fact
//         that a template that reads a signal re-renders when that signal changes.
// Passes: when `npx jest exercises/01-beginner/ex003_signal_basics` is green.
//
// set() vs update(): use set() when the new value does not depend on the old one
// (reset), and update() when it does (increment). `count.set(count() + 1)` works but
// reads and writes as two steps; update() is the single-step form.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <p class="count">Count: {{ count() }}</p>
//   <p class="step">Step: {{ step() }}</p>
//   <button class="inc" type="button" (click)="increment()">+</button>
//   <button class="dec" type="button" (click)="decrement()">-</button>
//   <button class="reset" type="button" (click)="reset()">Reset</button>
@Component({
  selector: "app-step-counter",
  standalone: true,
  template: `<p>TODO: render the counter — see the template contract above</p>`,
})
export class StepCounterComponent {
  readonly count = signal(0);
  readonly step = signal(1);

  /** Add the current step to the count. */
  increment(): void {
    throw new Error("TODO: implement increment");
  }

  /** Subtract the current step, but never go below zero. */
  decrement(): void {
    throw new Error("TODO: implement decrement");
  }

  /** Replace the step. Anything that is not an integer of at least 1 is a RangeError. */
  setStep(next: number): void {
    throw new Error("TODO: implement setStep");
  }

  /** Put the count back to zero and leave the step alone. */
  reset(): void {
    throw new Error("TODO: implement reset");
  }
}
