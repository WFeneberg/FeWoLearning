import { ChangeDetectionStrategy, ChangeDetectorRef, Component, inject } from "@angular/core";

// Exercise 076 — ChangeDetectorRef: markForCheck, detach, and manual detection (reference solution).

@Component({
  selector: "app-ticking-badge",
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<span class="badge">{{ label }}</span>`,
})
export class TickingBadgeComponent {
  private readonly cdr = inject(ChangeDetectorRef);

  label = "0";

  setLabel(value: string): void {
    this.label = value;
    this.cdr.markForCheck();
  }

  pause(): void {
    this.cdr.detach();
  }

  renderNow(): void {
    this.cdr.detectChanges();
  }
}
