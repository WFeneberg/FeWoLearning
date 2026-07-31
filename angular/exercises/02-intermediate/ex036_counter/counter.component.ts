import { Component, signal } from "@angular/core";

// Exercise 036 — CounterComponent (intermediate).
// Goal:   a standalone component showing a count with +/- buttons.
//         The count must never go below zero.
// Drills: standalone components, signals, event binding, template rendering.
@Component({
  selector: "app-counter",
  standalone: true,
  template: `<p>TODO: render count and buttons</p>`,
})
export class CounterComponent {
  readonly count = signal(0);

  increment(): void {
    throw new Error("TODO: implement increment");
  }

  decrement(): void {
    throw new Error("TODO: implement decrement (floor at 0)");
  }
}
