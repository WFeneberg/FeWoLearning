import { Component, EventEmitter, Input, Output, signal } from "@angular/core";

// Exercise 008 — QuantityStepperComponent (reference solution).
@Component({
  selector: "app-quantity-stepper",
  standalone: true,
  template: `
    <p class="value">Quantity: {{ value() }}</p>
    <button class="dec" type="button" (click)="dec()">-</button>
    <button class="inc" type="button" (click)="inc()">+</button>
  `,
})
export class QuantityStepperComponent {
  @Input() max = 10;

  readonly value = signal(0);

  @Output() readonly changed = new EventEmitter<number>();

  // The alias is the name parents bind; `limitReached` stays the field name.
  @Output("limit") readonly limitReached = new EventEmitter<number>();

  inc(): void {
    const current = this.value();
    if (current >= this.max) {
      // Already at the ceiling: nothing changed, so nothing is announced.
      return;
    }
    const next = current + 1;
    this.value.set(next);
    this.changed.emit(next);
    if (next === this.max) {
      // Exactly on arrival — a later blocked click must not re-announce it.
      this.limitReached.emit(this.max);
    }
  }

  dec(): void {
    const current = this.value();
    if (current === 0) {
      return;
    }
    const next = current - 1;
    this.value.set(next);
    this.changed.emit(next);
  }
}
