import { Component, ElementRef, signal, viewChild, viewChildren } from "@angular/core";

// Exercise 024 — viewChild() and viewChildren() (reference solution).

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
  template: `
    <input #nameBox class="name" />
    <app-badge />
    <app-badge />
    @if (expanded()) {
      <input #extraBox class="extra" />
    }
    <p class="typed">{{ typedLength() }}</p>
    <p class="count">{{ badgeCount() }}</p>
  `,
})
export class PanelComponent {
  readonly expanded = signal(false);

  // Queried by the template-reference name, so the result is an ElementRef, not the node.
  readonly nameBox = viewChild<ElementRef<HTMLInputElement>>("nameBox");

  // Optional for a real reason: while `expanded` is false this element is not rendered at
  // all, so the query has nothing to match and the signal reads undefined.
  readonly extraBox = viewChild<ElementRef<HTMLInputElement>>("extraBox");

  // Queried by component type, so the result is the child *instance*.
  readonly badge = viewChild.required(BadgeComponent);

  readonly badges = viewChildren(BadgeComponent);

  typedLength(): number {
    // An optional query is typed as possibly-undefined whether or not it can be in
    // practice, so the ?. and ?? 0 are what the type demands rather than paranoia.
    return this.nameBox()?.nativeElement.value.length ?? 0;
  }

  badgeCount(): number {
    return this.badges().length;
  }

  shoutFirst(): string {
    return this.badge().shout();
  }

  labelAll(prefix: string): void {
    this.badges().forEach((badge, index) => badge.text.set(`${prefix} ${index + 1}`));
  }
}
