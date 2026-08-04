import { Component, ElementRef, signal } from "@angular/core";

// Exercise 024 — viewChild() and viewChildren() (beginner).
// Goal:   reach a template element or child component from the class.
// Drills: viewChild() by template-reference name, viewChild() by component type,
//         viewChild.required(), viewChildren() for all matches, and query timing.
// Passes: when `npx jest exercises/01-beginner/ex024_view_child_signal` is green.
//
// Exercise 023's `#nameBox` only existed inside the template. viewChild() is how the class
// gets at the same thing: query by that name and you get an ElementRef wrapping the DOM
// node, or query by a component type and you get the child component *instance*, methods
// and all. The signal-based form returns a plain signal, so a computed can depend on it.
//
// Signal queries are computed lazily, on read, and a component instance always comes with
// its view already created — so unlike the old @ViewChild decorator (which stayed unset
// until ngAfterViewInit) these resolve straight away, with no detectChanges() needed.
//
// What is still genuinely optional is a target that may not be rendered at all: a query
// into a false @if finds nothing and comes back undefined. viewChild.required() turns
// "found nothing" into a thrown error, which is what you want for an element that is
// always there — the undefined would only ever be a bug you had to chase.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <input #nameBox class="name" />
//   <app-badge />
//   <app-badge />
//   @if (expanded()) {
//     <input #extraBox class="extra" />
//   }
//   <p class="typed">{{ typedLength() }}</p>
//   <p class="count">{{ badgeCount() }}</p>

@Component({
  selector: "app-badge",
  standalone: true,
  template: `<span class="text">{{ text() }}</span>`,
})
export class BadgeComponent {
  readonly text = signal("badge");

  shout(): string {
    return this.text().toUpperCase();
  }
}

@Component({
  selector: "app-panel",
  standalone: true,
  imports: [BadgeComponent],
  template: `<p>TODO: render the panel — see the template contract above</p>`,
})
export class PanelComponent {
  // The three placeholders below are plain signals so the stub compiles and the methods
  // have something to read. Replace each declaration with the real query.

  /** Whether the conditional half of the template is rendered. */
  readonly expanded = signal(false);

  /** TODO: query the #nameBox element with viewChild(). */
  readonly nameBox = signal<ElementRef<HTMLInputElement> | undefined>(undefined);

  /** TODO: query the #extraBox element with viewChild() — it is often not rendered. */
  readonly extraBox = signal<ElementRef<HTMLInputElement> | undefined>(undefined);

  /** TODO: query the first BadgeComponent with viewChild.required(). */
  readonly badge = signal<BadgeComponent | undefined>(undefined);

  /** TODO: query every BadgeComponent with viewChildren(). */
  readonly badges = signal<readonly BadgeComponent[]>([]);

  /** How many characters are in the input right now, or 0 if the query found nothing. */
  typedLength(): number {
    throw new Error("TODO: implement typedLength");
  }

  /** How many badges the view contains. */
  badgeCount(): number {
    throw new Error("TODO: implement badgeCount");
  }

  /** The first badge's text, shouted — proof that a query hands back the instance. */
  shoutFirst(): string {
    throw new Error("TODO: implement shoutFirst");
  }

  /** Set every badge's text, numbering them from 1: "tag 1", "tag 2", … */
  labelAll(prefix: string): void {
    throw new Error("TODO: implement labelAll");
  }
}
