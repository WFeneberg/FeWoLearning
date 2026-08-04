import { Component, Directive, ElementRef, inject, signal } from "@angular/core";

// Exercise 029 — an attribute directive (beginner).
// Goal:   attach behaviour to an existing element instead of wrapping it in a component.
// Drills: @Directive with an attribute selector, injecting ElementRef to reach the host
//         node, an input named after the selector, host listeners, and reading the element.
// Passes: when `npx jest exercises/01-beginner/ex029_attribute_directive` is green.
//
// A component brings its own element and template; a directive brings neither. It attaches
// to an element that already exists — `<p appHighlight>` — which is what you want when the
// behaviour is orthogonal to the markup: highlighting, autofocus, drag handles, tooltips.
// The same directive then works on a <p>, a <button> or a component, without any of them
// knowing about it.
//
// Injecting ElementRef gives the host node, and `.nativeElement` unwraps it. Writing to
// `.style` directly like this is the direct approach and is fine for a drill; in real code
// a host binding (`host: { "[style.backgroundColor]": "…" }`) is usually better, because it
// goes through Angular rather than around it and so survives re-renders.
//
// The naming trick worth knowing: an input called `appHighlight` — the same as the selector —
// lets `<p appHighlight="lime">` both apply the directive and pass it a value in one
// attribute.
//
// And the sharp edge that comes with it: `<p appHighlight>` does *not* leave the input at
// its default. The attribute is present with no value, so Angular sets the input to the
// empty string, and an input default only applies when nothing is bound at all — which for
// a directive named after its selector never happens. Painting with "" clears the colour
// instead of setting one. Absorb it with a transform that maps blank to the fallback, so
// callers of appHighlight() always get a usable colour.

@Directive({
  selector: "[appHighlight]",
  standalone: true,
  // TODO: add host listeners for "mouseenter" and "mouseleave" calling onEnter/onLeave.
})
export class HighlightDirective {
  /** The element this directive is attached to. */
  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);

  /**
   * TODO: an input named `appHighlight` whose value is always a usable colour.
   *
   * "yellow" when nothing was passed — including the bare-attribute case, which arrives as
   * the empty string and therefore needs a transform rather than a default.
   *
   * Declared as a plain signal so the stub compiles — replace the declaration.
   */
  readonly appHighlight = signal("yellow");

  /** How many times the pointer has entered. */
  readonly entries = signal(0);

  /** Paint the host's background with the current colour, and count the entry. */
  onEnter(): void {
    throw new Error("TODO: implement onEnter");
  }

  /** Clear the background again, leaving the count alone. */
  onLeave(): void {
    throw new Error("TODO: implement onLeave");
  }

  /** The host element's tag name, upper-case — proof of what ElementRef handed over. */
  hostTag(): string {
    throw new Error("TODO: implement hostTag");
  }
}

/** A host for the directive, with three different attachments. */
@Component({
  selector: "app-highlight-host",
  standalone: true,
  imports: [HighlightDirective],
  template: `
    <p class="plain" appHighlight>default colour</p>
    <p class="lime" appHighlight="lime">explicit colour</p>
    <button class="btn" type="button" appHighlight="pink">a button works too</button>
  `,
})
export class HighlightHostComponent {}
