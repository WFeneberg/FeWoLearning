import { Component, computed, linkedSignal, input } from "@angular/core";

// Exercise 070 — asserting signal state through a fixture (reference solution).

@Component({
  selector: "app-countdown",
  standalone: true,
  template: `
    <p class="remaining">{{ display() }}</p>
    <button class="tick" type="button" (click)="tick()">Tick</button>
  `,
})
export class CountdownComponent {
  readonly startFrom = input.required<number>();

  // Resets to the new startFrom on every rebind — that reset is exactly what the spec drills.
  readonly remaining = linkedSignal(() => this.startFrom());

  readonly finished = computed(() => this.remaining() === 0);

  readonly display = computed(() => (this.finished() ? "Done" : String(this.remaining())));

  tick(): void {
    this.remaining.update((n) => Math.max(0, n - 1));
  }
}
