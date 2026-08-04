import { Component, HostBinding, HostListener, signal } from "@angular/core";

// Exercise 027 — host bindings and listeners (reference solution).

@Component({
  selector: "app-toggle-chip",
  standalone: true,
  // Everything about the host element in one place. The expressions are evaluated with
  // the component as their context, exactly like a template expression.
  host: {
    class: "chip",
    "[class.active]": "active()",
    "[class.disabled]": "disabled()",
    "[attr.role]": "'button'",
    "[attr.aria-pressed]": "active()",
    "[attr.tabindex]": "disabled() ? -1 : 0",
    "(click)": "toggle()",
    // The .enter suffix does the key filtering, so there is no event.key check here.
    "(keydown.enter)": "toggle()",
  },
  template: `<span class="label">{{ label() }}</span>`,
})
export class ToggleChipComponent {
  readonly label = signal("chip");
  readonly active = signal(false);
  readonly disabled = signal(false);

  readonly toggles = signal(0);

  toggle(): void {
    if (this.disabled()) {
      // A blocked attempt is not a toggle, so the counter must not move either.
      return;
    }
    this.active.update((active) => !active);
    this.toggles.update((n) => n + 1);
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

  // The older per-member form. Same effect, spread across the class instead of gathered
  // in the decorator — which is why `host` metadata is what new code reaches for.
  @HostBinding("class.chip") readonly isChip = true;

  @HostBinding("class.active")
  get isActive(): boolean {
    return this.active();
  }

  @HostBinding("attr.aria-pressed")
  get pressed(): boolean {
    return this.active();
  }

  @HostListener("click")
  onClick(): void {
    this.toggle();
  }

  toggle(): void {
    this.active.update((active) => !active);
    this.toggles.update((n) => n + 1);
  }
}
