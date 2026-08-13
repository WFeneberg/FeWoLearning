import { ChangeDetectorRef, Component, inject, signal } from "@angular/core";

// Exercise 089 — zoneless change detection: signals as the one notification primitive (reference solution).

@Component({
  selector: "app-zoneless-counter",
  standalone: true,
  template: `
    <span class="count">{{ count() }}</span>
    <span class="legacy">{{ legacyCount }}</span>
    <button class="inc" (click)="increment()">+</button>
  `,
})
export class ZonelessCounterComponent {
  private readonly cdr = inject(ChangeDetectorRef);

  readonly count = signal(0);
  legacyCount = 0;

  increment(): void {
    this.count.update((value) => value + 1);
  }

  bumpLegacyCount(): void {
    this.legacyCount++;
    this.cdr.markForCheck();
  }
}
