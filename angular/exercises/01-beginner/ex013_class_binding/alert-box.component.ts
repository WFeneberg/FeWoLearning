import { Component, signal } from "@angular/core";

// Exercise 013 — AlertBoxComponent (beginner).
// Goal:   drive an element's classes from state, three different ways.
// Drills: [class.name] for one class, [class] for a whole object or string, [ngClass]
//         from CommonModule, and the fact that none of them disturb a static class="".
// Passes: when `npx jest exercises/01-beginner/ex013_class_binding` is green.
//
// Which to reach for: [class.active]="expr" is the clearest when you have a couple of
// independent flags. [class]="{a: x, b: y}" is better once the set is computed, and it
// is built in — [ngClass] does the same job but needs CommonModule imported, which is
// why new code rarely uses it. All three merge with a static class attribute rather
// than replacing it, so `class="alert"` survives.
//
// One trap worth meeting now: [class] *replaces* the bound set on each change. Two
// [class] bindings on one element is a compile error, and mixing [class]="obj" with
// [class.x] works but the specific binding wins.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <div
//     class="alert"
//     [class.error]="severity() === 'error'"
//     [class.warning]="severity() === 'warning'"
//     [class.dismissed]="dismissed()"
//   >{{ message() }}</div>
//
//   <div class="badge" [class]="badgeClasses()"></div>
//
//   <div class="legacy" [ngClass]="badgeClasses()"></div>

export type Severity = "info" | "warning" | "error";

@Component({
  selector: "app-alert-box",
  standalone: true,
  template: `<p>TODO: render the alert — see the template contract above</p>`,
})
export class AlertBoxComponent {
  readonly severity = signal<Severity>("info");
  readonly message = signal("All good");
  readonly dismissed = signal(false);
  readonly pinned = signal(false);

  /**
   * The class set for the badge, as an object of class name -> on/off.
   *
   * Exactly these five keys, always present: "info", "warning", "error", "pinned" and
   * "muted". The one matching `severity()` is true and the other two false; "pinned"
   * mirrors `pinned()`; "muted" mirrors `dismissed()`.
   *
   * Keys that are off stay in the object with a false value rather than being left out —
   * that is how an object binding turns a class back off instead of just forgetting it.
   */
  badgeClasses(): Record<string, boolean> {
    throw new Error("TODO: implement badgeClasses");
  }
}
