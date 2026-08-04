import { Component, Directive, ElementRef, inject, input, signal } from "@angular/core";

// Exercise 029 — an attribute directive (reference solution).

@Directive({
  selector: "[appHighlight]",
  standalone: true,
  host: {
    "(mouseenter)": "onEnter()",
    "(mouseleave)": "onLeave()",
  },
})
export class HighlightDirective {
  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);

  // Named after the selector, so `appHighlight="lime"` applies the directive *and* passes
  // it a value.
  //
  // The transform is not decoration. `<p appHighlight>` binds the empty string rather than
  // nothing at all, so the "yellow" default never gets a chance to apply — without this,
  // the bare form would paint with "" and clear the colour instead of setting one.
  readonly appHighlight = input("yellow", {
    transform: (value: string) => (value.trim() === "" ? "yellow" : value),
  });

  readonly entries = signal(0);

  onEnter(): void {
    this.host.nativeElement.style.backgroundColor = this.appHighlight();
    this.entries.update((n) => n + 1);
  }

  onLeave(): void {
    // Back to empty, not to a hard-coded colour — the element keeps whatever CSS says.
    this.host.nativeElement.style.backgroundColor = "";
  }

  hostTag(): string {
    return this.host.nativeElement.tagName;
  }
}

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
