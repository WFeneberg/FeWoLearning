import { Component, Directive, inject, output, signal } from "@angular/core";

// Exercise 082 — hostDirectives: composing behaviour without inheritance (reference solution).

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
  hostDirectives: [ExpandableDirective, HighlightableDirective],
  template: `
    <button type="button" class="header" (click)="onHeaderClick()">Toggle</button>
    <div class="body">Panel content</div>
  `,
})
export class PanelComponent {
  private readonly expandable = inject(ExpandableDirective, { self: true });

  onHeaderClick(): void {
    this.expandable.toggle();
  }
}
