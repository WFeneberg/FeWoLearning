import { NgClass } from "@angular/common";
import { Component, signal } from "@angular/core";

// Exercise 013 — AlertBoxComponent (reference solution).

export type Severity = "info" | "warning" | "error";

@Component({
  selector: "app-alert-box",
  standalone: true,
  // NgClass has to be imported; [class] and [class.x] are built in and do not.
  imports: [NgClass],
  template: `
    <div
      class="alert"
      [class.error]="severity() === 'error'"
      [class.warning]="severity() === 'warning'"
      [class.dismissed]="dismissed()"
    >{{ message() }}</div>

    <div class="badge" [class]="badgeClasses()"></div>

    <div class="legacy" [ngClass]="badgeClasses()"></div>
  `,
})
export class AlertBoxComponent {
  readonly severity = signal<Severity>("info");
  readonly message = signal("All good");
  readonly dismissed = signal(false);
  readonly pinned = signal(false);

  badgeClasses(): Record<string, boolean> {
    const severity = this.severity();
    // Every key is always present. Omitting the false ones would leave a class that was
    // switched on earlier stuck on, because the binding only applies what it is told.
    return {
      info: severity === "info",
      warning: severity === "warning",
      error: severity === "error",
      pinned: this.pinned(),
      muted: this.dismissed(),
    };
  }
}
