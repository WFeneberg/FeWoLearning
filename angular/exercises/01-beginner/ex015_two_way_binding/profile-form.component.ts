import { Component } from "@angular/core";

// Exercise 015 — ProfileFormComponent (beginner).
// Goal:   bind form controls both ways with [(ngModel)].
// Drills: importing FormsModule into a standalone component, [(ngModel)] on text,
//         number, checkbox and select controls, the banana-in-a-box as sugar for
//         [ngModel] + (ngModelChange), and why the field must be a plain property.
// Passes: when `npx jest exercises/01-beginner/ex015_two_way_binding` is green.
//
// `[(ngModel)]="name"` expands to `[ngModel]="name" (ngModelChange)="name = $event"` —
// there is no magic, just a naming convention: a two-way binding on `x` needs an input
// `x` and an output `xChange`. That is also why ngModel needs a *writable property*, not
// a signal: the generated assignment is `name = $event`, which a signal cannot accept.
// Exercise 016 does the same thing with model() and gets signals back.
//
// FormsModule must be in the component's `imports` or ngModel is silently just an
// unknown attribute. A standalone component imports it directly — no NgModule needed.
//
// A note on the tests: writing to an <input> from a test means setting `.value` and then
// dispatching an "input" event, because that is what a real keystroke does. Reading the
// other direction only needs a detectChanges().
//
// Template contract the spec asserts (name attributes matter — ngModel needs them):
//   <input class="name" name="name" [(ngModel)]="name" />
//   <input class="age" name="age" type="number" [(ngModel)]="age" />
//   <input class="subscribed" name="subscribed" type="checkbox" [(ngModel)]="subscribed" />
//   <select class="role" name="role" [(ngModel)]="role">
//     <option value="member">Member</option>
//     <option value="admin">Admin</option>
//   </select>
//   <p class="summary">{{ summary() }}</p>

export type Role = "member" | "admin";

@Component({
  selector: "app-profile-form",
  standalone: true,
  // TODO: import FormsModule here, or [(ngModel)] does nothing at all.
  template: `<p>TODO: render the form — see the template contract above</p>`,
})
export class ProfileFormComponent {
  // Plain writable properties, not signals: ngModel assigns straight to them.
  name = "";
  age = 0;
  subscribed = false;
  role: Role = "member";

  /**
   * A one-line description of the current form state.
   *
   * `"Ada (34, admin, subscribed)"` — the name, then the age, role and either
   * "subscribed" or "unsubscribed" in parentheses. A blank name reads as "Anonymous".
   */
  summary(): string {
    throw new Error("TODO: implement summary");
  }
}
