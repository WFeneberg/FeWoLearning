import { Component, computed, input, signal } from "@angular/core";

// Exercise 096 — a design-system a11y widget: an ARIA listbox with keyboard navigation (reference
// solution).

@Component({
  selector: "app-a11y-listbox",
  standalone: true,
  template: `
    <ul
      class="listbox"
      role="listbox"
      tabindex="0"
      [attr.aria-activedescendant]="activeOptionId()"
      (keydown)="onKeydown($event)"
    >
      @for (option of options(); track option; let i = $index) {
        <li
          [id]="optionElementId(i)"
          class="option"
          role="option"
          [attr.aria-selected]="i === selectedIndex()"
          [class.active]="i === activeIndex()"
          (click)="selectIndex(i)"
        >
          {{ option }}
        </li>
      }
    </ul>
  `,
})
export class A11yListboxComponent {
  readonly options = input.required<readonly string[]>();

  readonly activeIndex = signal(0);
  readonly selectedIndex = signal<number | null>(null);

  readonly activeOptionId = computed(() => this.optionElementId(this.activeIndex()));

  optionElementId(index: number): string {
    return `a11y-listbox-option-${index}`;
  }

  onKeydown(event: KeyboardEvent): void {
    const lastIndex = this.options().length - 1;

    switch (event.key) {
      case "ArrowDown":
        event.preventDefault();
        this.activeIndex.update((i) => Math.min(i + 1, lastIndex));
        break;
      case "ArrowUp":
        event.preventDefault();
        this.activeIndex.update((i) => Math.max(i - 1, 0));
        break;
      case "Home":
        event.preventDefault();
        this.activeIndex.set(0);
        break;
      case "End":
        event.preventDefault();
        this.activeIndex.set(lastIndex);
        break;
      case "Enter":
      case " ":
        event.preventDefault();
        this.selectIndex(this.activeIndex());
        break;
      // Anything else: leave the event — and every other signal here — untouched.
    }
  }

  selectIndex(index: number): void {
    this.selectedIndex.set(index);
    this.activeIndex.set(index);
  }
}
