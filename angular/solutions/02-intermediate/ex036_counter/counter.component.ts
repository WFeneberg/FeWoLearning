import { Component, signal } from "@angular/core";

// Exercise 036 — CounterComponent (reference solution).
@Component({
  selector: "app-counter",
  standalone: true,
  template: `
    <p>Count: {{ count() }}</p>
    <button type="button" (click)="decrement()">-</button>
    <button type="button" (click)="increment()">+</button>
  `,
})
export class CounterComponent {
  readonly count = signal(0);

  increment(): void {
    this.count.update((n) => n + 1);
  }

  decrement(): void {
    this.count.update((n) => Math.max(0, n - 1));
  }
}
