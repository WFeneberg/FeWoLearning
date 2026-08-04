import { Component } from "@angular/core";
import { FormsModule } from "@angular/forms";

// Exercise 015 — ProfileFormComponent (reference solution).

export type Role = "member" | "admin";

@Component({
  selector: "app-profile-form",
  standalone: true,
  // Without this, [(ngModel)] is an unknown attribute and binds nothing.
  imports: [FormsModule],
  template: `
    <input class="name" name="name" [(ngModel)]="name" />
    <input class="age" name="age" type="number" [(ngModel)]="age" />
    <input class="subscribed" name="subscribed" type="checkbox" [(ngModel)]="subscribed" />
    <select class="role" name="role" [(ngModel)]="role">
      <option value="member">Member</option>
      <option value="admin">Admin</option>
    </select>
    <p class="summary">{{ summary() }}</p>
  `,
})
export class ProfileFormComponent {
  // Plain properties: [(ngModel)] expands to an assignment, `name = $event`.
  name = "";
  age = 0;
  subscribed = false;
  role: Role = "member";

  summary(): string {
    const name = this.name.trim() === "" ? "Anonymous" : this.name.trim();
    const subscription = this.subscribed ? "subscribed" : "unsubscribed";
    return `${name} (${this.age}, ${this.role}, ${subscription})`;
  }
}
