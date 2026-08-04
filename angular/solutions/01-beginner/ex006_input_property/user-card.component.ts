import { booleanAttribute, Component, Input, numberAttribute } from "@angular/core";

// Exercise 006 — UserCardComponent (reference solution).
@Component({
  selector: "app-user-card",
  standalone: true,
  template: `
    <h3 class="name">{{ name }}</h3>
    <p class="badge">{{ badge() }}</p>
    <p class="score">Score: {{ score }}</p>
    <p class="mode">{{ compact ? "compact" : "full" }}</p>
  `,
})
export class UserCardComponent {
  // `required` is checked by the template type checker at build time, which is why the
  // definite-assignment `!` is honest here rather than a lie.
  @Input({ required: true }) name!: string;

  // No initialiser games needed: the field's own initialiser *is* the default.
  @Input() role = "member";

  // numberAttribute turns "42" into 42 (and anything unparseable into NaN), so the
  // field's declared type stays true even when the value came from an attribute.
  @Input({ transform: numberAttribute }) score = 0;

  // booleanAttribute maps "" (a bare attribute) to true and "false"/null/undefined to
  // false, which is how `<app-user-card compact>` behaves like an HTML boolean.
  @Input({ transform: booleanAttribute }) compact = false;

  // The public name templates bind is `admin`; the field stays `isAdmin`.
  @Input({ alias: "admin" }) isAdmin = false;

  badge(): string {
    return this.isAdmin ? "ADMIN" : this.role.toUpperCase();
  }
}
