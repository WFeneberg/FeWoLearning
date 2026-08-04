import { NgStyle } from "@angular/common";
import { Component, signal } from "@angular/core";

// Exercise 014 — ProgressBarComponent (reference solution).
@Component({
  selector: "app-progress-bar",
  standalone: true,
  imports: [NgStyle],
  template: `
    <div class="track">
      <!-- The .% and .px suffixes let the component keep returning plain numbers. -->
      <div class="fill" [style.width.%]="percent()" [style.backgroundColor]="color()"></div>
    </div>

    <div class="label" [style.fontSize.px]="labelSize()">{{ percent() }}%</div>

    <div class="boxed" [style]="boxStyles()"></div>

    <div class="legacy" [ngStyle]="boxStyles()"></div>
  `,
})
export class ProgressBarComponent {
  readonly value = signal(0);
  readonly total = signal(100);
  readonly labelSize = signal(12);

  percent(): number {
    const total = this.total();
    if (total <= 0) {
      throw new RangeError("total must be greater than zero");
    }
    // Clamp rather than reject: a bar past its end is worse than one pinned to it.
    const ratio = Math.min(1, Math.max(0, this.value() / total));
    return Math.round(ratio * 100);
  }

  color(): string {
    const percent = this.percent();
    if (percent < 34) {
      return "crimson";
    }
    if (percent < 67) {
      return "orange";
    }
    return "seagreen";
  }

  boxStyles(): Record<string, string> {
    // An object binding has no unit suffix, so every value is a finished string.
    return {
      "border-color": this.color(),
      opacity: this.percent() < 100 ? "0.5" : "1",
    };
  }
}
