import { Component } from "@angular/core";

// Exercise 006 — UserCardComponent (beginner).
// Goal:   turn plain fields into a component's public API with @Input().
// Drills: @Input(), keeping a field's initialiser as the default, @Input({required:true}),
//         renaming the public name with `alias`, and normalising incoming values with
//         `transform` — including the booleanAttribute / numberAttribute helpers from
//         @angular/core.
// Passes: when `npx jest exercises/01-beginner/ex006_input_property` is green.
//
// Why transforms: a value written in a template as an *attribute* arrives as a string,
// so `<app-user-card score="42">` would otherwise set the number field to "42". And
// `<app-user-card compact>` arrives as the empty string, which booleanAttribute maps to
// true — that is what makes a bare attribute work like an HTML boolean attribute.
//
// A note on `required`: it is enforced by Angular's *template type checker* at build
// time, not at runtime, so the spec cannot assert that omitting it throws. Declare it
// anyway — it is the difference between a compile error and a silent undefined.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <h3 class="name">{{ name }}</h3>
//   <p class="badge">{{ badge() }}</p>
//   <p class="score">Score: {{ score }}</p>
//   <p class="mode">{{ compact ? "compact" : "full" }}</p>
@Component({
  selector: "app-user-card",
  standalone: true,
  template: `<p>TODO: render the card — see the template contract above</p>`,
})
export class UserCardComponent {
  /** TODO: a required input. */
  name!: string;

  /** TODO: an optional input; "member" stays the default when nobody binds it. */
  role = "member";

  /** TODO: an input that coerces whatever arrives into a number. */
  score = 0;

  /** TODO: an input that coerces an attribute-style value into a boolean. */
  compact = false;

  /** TODO: an input whose *public* name is `admin` while the field stays `isAdmin`. */
  isAdmin = false;

  /** "ADMIN" when isAdmin, otherwise the role upper-cased. */
  badge(): string {
    throw new Error("TODO: implement badge");
  }
}
