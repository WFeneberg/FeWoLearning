import { Component, EventEmitter, Input, Output, signal } from "@angular/core";

// Exercise 008 — QuantityStepperComponent (beginner).
// Goal:   let a child tell its parent something happened, the classic decorator way.
// Drills: @Output(), EventEmitter<T>, emit(), subscribing to an output, renaming one
//         with an alias, and the discipline of *not* emitting when nothing changed.
// Passes: when `npx jest exercises/01-beginner/ex008_output_emitter` is green.
//
// Inputs flow down, outputs flow up: the child never reaches into its parent, it just
// announces. An EventEmitter is an RxJS Subject, which is why a test can subscribe() to
// it directly instead of going through a host template — though the spec does both.
//
// The rule worth internalising: an output is a *notification that state changed*. When a
// click is clamped away and the value stays put, nothing changed, so nothing is emitted.
// Emitting anyway is how parents end up in redundant-update loops.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <p class="value">Quantity: {{ value() }}</p>
//   <button class="dec" type="button" (click)="dec()">-</button>
//   <button class="inc" type="button" (click)="inc()">+</button>
@Component({
  selector: "app-quantity-stepper",
  standalone: true,
  template: `<p>TODO: render the stepper — see the template contract above</p>`,
})
export class QuantityStepperComponent {
  @Input() max = 10;

  readonly value = signal(0);

  // The emitters exist so the stub compiles, but neither is an *output* yet: without
  // @Output() a parent template's (changed)="…" binds a DOM event that never fires.

  /** TODO: expose this as an output that emits the new quantity whenever it changes. */
  readonly changed = new EventEmitter<number>();

  /** TODO: expose this as an output whose public name is `limit`. */
  readonly limitReached = new EventEmitter<number>();

  /**
   * Add one, never going past `max`.
   *
   * Emits `changed` with the new value only when the value actually moved, and emits
   * `limit` with `max` on the step that arrives at the ceiling.
   */
  inc(): void {
    throw new Error("TODO: implement inc");
  }

  /** Subtract one, never going below zero. Emits `changed` only when the value moved. */
  dec(): void {
    throw new Error("TODO: implement dec");
  }
}
