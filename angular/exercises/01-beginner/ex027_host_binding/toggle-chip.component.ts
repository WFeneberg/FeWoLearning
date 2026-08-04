import { Component, signal } from "@angular/core";

// Exercise 027 — host bindings and listeners (beginner).
// Goal:   style and wire up a component's *own* element, not something inside its template.
// Drills: the `host` metadata object, [class.x]/[attr.x] on the host, host event listeners
//         including the (keydown.enter) key shorthand, and the @HostBinding/@HostListener
//         decorator equivalents.
// Passes: when `npx jest exercises/01-beginner/ex027_host_binding` is green.
//
// Every component renders inside an element named by its selector — `<app-toggle-chip>`.
// Nothing in the template can touch that element, which is a problem when the thing you
// need to set is a class, an ARIA attribute or a click handler on the component *as a
// whole*. That is what host bindings are for: `host: { "[class.active]": "active()" }`
// binds against the host element with the component as the expression's context.
//
// This is not cosmetic. A chip that other code can style and screen readers can announce
// has to carry `role`, `aria-pressed` and `tabindex` on its own element — wrapping a div
// inside the template instead leaves the outer element inert and unfocusable.
//
// `(keydown.enter)` is Angular's key-event shorthand: it filters keydown to that key for
// you, so there is no `event.key === "Enter"` check to forget.
//
// The two classes below must end up behaving identically. `host` metadata keeps everything
// in one place and is what new code uses; @HostBinding / @HostListener are the older
// per-member form, still common in directives you will read.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <span class="label">{{ label() }}</span>

@Component({
  selector: "app-toggle-chip",
  standalone: true,
  // TODO: add host bindings so the host element carries:
  //   - a static class "chip"
  //   - class "active" while active()
  //   - class "disabled" while disabled()
  //   - attribute role="button"
  //   - attribute aria-pressed reflecting active()
  //   - attribute tabindex of -1 when disabled, otherwise 0
  //   - a click listener calling toggle()
  //   - an Enter-key listener calling toggle()
  template: `<p>TODO: render the chip — see the template contract above</p>`,
})
export class ToggleChipComponent {
  readonly label = signal("chip");
  readonly active = signal(false);
  readonly disabled = signal(false);

  /** How many times the chip actually toggled — a blocked attempt must not count. */
  readonly toggles = signal(0);

  /** Flip `active` and count it. While disabled, do nothing at all. */
  toggle(): void {
    throw new Error("TODO: implement toggle");
  }
}

@Component({
  selector: "app-decorated-chip",
  standalone: true,
  template: `<span class="label">{{ label() }}</span>`,
})
export class DecoratedChipComponent {
  readonly label = signal("chip");
  readonly active = signal(false);

  readonly toggles = signal(0);

  // TODO: reproduce the "chip" class, the "active" class, the aria-pressed attribute and
  // the click listener using @HostBinding and @HostListener instead of `host` metadata.

  toggle(): void {
    throw new Error("TODO: implement toggle");
  }
}
