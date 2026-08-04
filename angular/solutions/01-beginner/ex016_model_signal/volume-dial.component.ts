import { Component, model, signal } from "@angular/core";

// Exercise 016 — VolumeDialComponent (reference solution).
@Component({
  selector: "app-volume-dial",
  standalone: true,
  template: `
    <p class="level">{{ label() }}: {{ level() }}</p>
    <p class="muted">{{ muted() ? "muted" : "live" }}</p>
    <button class="up" type="button" (click)="up()">+</button>
    <button class="down" type="button" (click)="down()">-</button>
    <button class="mute" type="button" (click)="toggleMute()">Mute</button>
  `,
})
export class VolumeDialComponent {
  // An input `label` plus an output `labelChange`, in one declaration.
  readonly label = model.required<string>();

  readonly level = model(50);

  // Not a model: nothing outside this component can bind or observe it.
  readonly muted = signal(false);

  up(): void {
    const current = this.level();
    if (current >= 100) {
      // Writing 100 over 100 would still emit levelChange and nudge the parent.
      return;
    }
    this.level.set(Math.min(100, current + 10));
  }

  down(): void {
    const current = this.level();
    if (current <= 0) {
      return;
    }
    this.level.set(Math.max(0, current - 10));
  }

  toggleMute(): void {
    // The level is left alone, which is what makes unmuting restore it.
    this.muted.update((muted) => !muted);
  }

  effective(): number {
    return this.muted() ? 0 : this.level();
  }
}
