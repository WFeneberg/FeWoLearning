import { Component, signal } from "@angular/core";

// Exercise 005 — ClickTrackerComponent (reference solution).
@Component({
  selector: "app-click-tracker",
  standalone: true,
  template: `
    <p class="taps">Taps: {{ taps() }}</p>
    <p class="outer-taps">Outer: {{ outerTaps() }}</p>
    <p class="modifiers">{{ modifiers().join(",") }}</p>
    <button class="tap" type="button" (click)="tap($event)">Tap</button>
    <a class="link" href="/nope" (click)="follow($event)">Details</a>
    <div class="outer" (click)="outerTap()">
      <button class="inner" type="button" (click)="innerTap($event)">Inner</button>
    </div>
    <!-- No $event: do not thread an argument a handler does not use. -->
    <button class="reset" type="button" (click)="reset()">Reset</button>
  `,
})
export class ClickTrackerComponent {
  readonly taps = signal(0);
  readonly outerTaps = signal(0);
  readonly modifiers = signal<readonly string[]>([]);

  tap(event: MouseEvent): void {
    this.taps.update((n) => n + 1);
    // $event is the real DOM MouseEvent, so the modifier flags are right there.
    const label = event.shiftKey ? "shift" : event.ctrlKey ? "ctrl" : "plain";
    this.record(label);
  }

  follow(event: Event): void {
    // Without this the browser would navigate away from the component under test.
    event.preventDefault();
    this.record("blocked");
  }

  innerTap(event: MouseEvent): void {
    this.taps.update((n) => n + 1);
    // The click would otherwise bubble to div.outer and be counted twice.
    event.stopPropagation();
  }

  outerTap(): void {
    this.outerTaps.update((n) => n + 1);
  }

  reset(): void {
    this.taps.set(0);
    this.outerTaps.set(0);
    this.modifiers.set([]);
  }

  private record(label: string): void {
    this.modifiers.update((labels) => [...labels, label]);
  }
}
