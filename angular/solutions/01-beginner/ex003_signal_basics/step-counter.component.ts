import { Component, signal } from "@angular/core";

// Exercise 003 — StepCounterComponent (reference solution).
@Component({
  selector: "app-step-counter",
  standalone: true,
  template: `
    <p class="count">Count: {{ count() }}</p>
    <p class="step">Step: {{ step() }}</p>
    <button class="inc" type="button" (click)="increment()">+</button>
    <button class="dec" type="button" (click)="decrement()">-</button>
    <button class="reset" type="button" (click)="reset()">Reset</button>
  `,
})
export class StepCounterComponent {
  readonly count = signal(0);
  readonly step = signal(1);

  increment(): void {
    // update() when the new value depends on the old one.
    this.count.update((n) => n + this.step());
  }

  decrement(): void {
    this.count.update((n) => Math.max(0, n - this.step()));
  }

  setStep(next: number): void {
    // Validate before writing: a rejected change must leave the signal as it was.
    if (!Number.isInteger(next) || next < 1) {
      throw new RangeError("step must be an integer of at least 1");
    }
    this.step.set(next);
  }

  reset(): void {
    // set() when the new value does not depend on the old one.
    this.count.set(0);
  }
}
