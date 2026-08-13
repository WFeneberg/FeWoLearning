import { Component, Directive, inject, output, signal } from "@angular/core";

// Exercise 082 — hostDirectives: composing behaviour without inheritance (advanced).
// Goal:   assemble a component's behaviour out of small, independently reusable directives,
//         instead of copy-pasting their logic into every component that needs it.
// Drills: `hostDirectives`, composing more than one directive onto the same host, and reaching
//         a composed directive's public API from the component it is attached to.
// Passes: when `npx jest exercises/03-advanced/ex082_directive_composition` is green.
//
// Before `hostDirectives`, the only way to share behaviour like "this element can be expanded and
// collapsed" or "this element highlights on hover" across unrelated components was inheritance (one
// base class, brittle the moment two components need *both* behaviours) or duplicating the host
// bindings and state in every component that wanted them. `hostDirectives` lets a component apply
// another directive to its own host element the same way a template author would apply it to any
// element — `ExpandableDirective` and `HighlightableDirective` below are not aware PanelComponent
// exists; they were written to be dropped onto anything.
//
// Composing a directive this way puts a *second* instance of it (private to this component) on the
// same host element the component itself renders as — its host bindings apply directly to that
// element, exactly as if you had written `<div appExpandable appHighlightable>` by hand. That is
// also why the composed directive is reachable via `inject(Type, { self: true })` from inside the
// component: `self` means "look only at this exact element's own providers," which includes every
// directive hostDirectives put there.
//
// The point of delegating is real, not decorative: PanelComponent's click handler does not
// reimplement "toggle a boolean and reflect it as a class" — it asks the directive it composed to
// do that, because that directive already owns that behaviour correctly and PanelComponent has no
// business knowing how it is implemented.

@Directive({
  selector: "[appExpandable]",
  standalone: true,
  host: {
    "[class.expanded]": "expanded()",
    "[attr.aria-expanded]": "expanded()",
  },
})
export class ExpandableDirective {
  private readonly expanded = signal(false);
  readonly expandedChange = output<boolean>();

  toggle(): void {
    this.expanded.update((value) => !value);
    this.expandedChange.emit(this.expanded());
  }

  isExpanded(): boolean {
    return this.expanded();
  }
}

@Directive({
  selector: "[appHighlightable]",
  standalone: true,
  host: {
    "(mouseenter)": "setHighlighted(true)",
    "(mouseleave)": "setHighlighted(false)",
    "[class.highlighted]": "highlighted()",
  },
})
export class HighlightableDirective {
  private readonly highlighted = signal(false);

  setHighlighted(value: boolean): void {
    this.highlighted.set(value);
  }
}

@Component({
  selector: "app-panel",
  standalone: true,
  hostDirectives: [
    ExpandableDirective,
    // TODO: this panel is supposed to highlight on hover too, the same way any other element that
    // wrote `appHighlightable` on itself would — compose HighlightableDirective here as well.
  ],
  template: `
    <button type="button" class="header" (click)="onHeaderClick()">Toggle</button>
    <div class="body">Panel content</div>
  `,
})
export class PanelComponent {
  private readonly expandable = inject(ExpandableDirective, { self: true });

  /**
   * TODO: implement onHeaderClick — delegate to the composed ExpandableDirective's toggle().
   * PanelComponent should not reimplement open/closed state; the whole point of composing
   * ExpandableDirective is that this component only needs to know it *can* be toggled.
   */
  onHeaderClick(): void {
    throw new Error("TODO: implement onHeaderClick");
  }
}
