import { Component, computed, input, signal } from "@angular/core";

// Exercise 096 — a design-system a11y widget: an ARIA listbox with keyboard navigation (expert).
// Goal:   a selectable list whose ARIA state and keyboard behaviour match the APG listbox pattern —
//         not a `<ul>` of clickable `<li>`s that merely looks like one but is invisible to a screen
//         reader and unusable from a keyboard.
// Drills: `role="listbox"`/`role="option"`, `aria-selected`, `aria-activedescendant` for tracking
//         which option is "active" without moving real DOM focus off the listbox container, and
//         keyboard handling for ArrowUp/ArrowDown/Home/End/Enter/Space.
// Passes: when `npx jest exercises/04-expert/ex096_design_system_a11y_widget` is green.
//
// A real focusable option per `<li>` is one legitimate way to build this widget, but it means
// moving DOM focus on every arrow key, which is exactly the kind of thing that behaves differently
// across browsers and is flaky under jsdom. The APG documents a second pattern for exactly this
// reason: keep ONE focusable element (the listbox `<ul>` itself, `tabindex="0"`), track which
// option is logically "active" in component state, and point `aria-activedescendant` at that
// option's id. Assistive tech announces the active option exactly as if focus had moved there, but
// the DOM's real focus never leaves the container — one `tabindex`, one place keyboard events are
// ever handled.
//
// `aria-selected` and "active" are two different concepts and this widget must not conflate them:
// `aria-selected` reflects which option was actually CHOSEN (Enter/Space/click) and normally
// changes rarely; `activeIndex` is which option the arrow keys are currently ON, and changes on
// every ArrowUp/ArrowDown — a listbox can arrow through five options without selecting any of them.
//
// Handled keys must call `event.preventDefault()` (Space, unhandled, would scroll the page; arrow
// keys, unhandled, may scroll a containing element) — but ONLY the keys this widget actually
// understands. Calling it unconditionally would make the listbox swallow every other keystroke a
// user might have meant for something else on the page.
//
// Template contract the spec asserts (roles/classes are the query hooks — keep them):
//   <ul class="listbox" role="listbox" tabindex="0"
//       [attr.aria-activedescendant]="activeOptionId()" (keydown)="onKeydown($event)">
//     @for (option of options(); track option; let i = $index) {
//       <li [id]="optionElementId(i)" class="option" role="option"
//           [attr.aria-selected]="i === selectedIndex()" [class.active]="i === activeIndex()"
//           (click)="selectIndex(i)">{{ option }}</li>
//     }
//   </ul>

@Component({
  selector: "app-a11y-listbox",
  standalone: true,
  // TODO: render the listbox — see the template contract above.
  template: `<p>TODO: render the listbox — see the template contract above</p>`,
})
export class A11yListboxComponent {
  readonly options = input.required<readonly string[]>();

  /** Which option the arrow keys are currently on — NOT the same thing as "selected". */
  readonly activeIndex = signal(0);

  /** Which option was actually chosen (Enter/Space/click). null until something is chosen. */
  readonly selectedIndex = signal<number | null>(null);

  readonly activeOptionId = computed(() => this.optionElementId(this.activeIndex()));

  optionElementId(index: number): string {
    return `a11y-listbox-option-${index}`;
  }

  /**
   * TODO: implement onKeydown.
   *   - "ArrowDown": move activeIndex forward by one, clamped to the last option (no wraparound
   *     past the end).
   *   - "ArrowUp": move activeIndex back by one, clamped to 0 (no wraparound below the start).
   *   - "Home": jump activeIndex to 0.
   *   - "End": jump activeIndex to the last option's index.
   *   - "Enter" or " " (space): call `this.selectIndex(this.activeIndex())`.
   *   - Any other key: do nothing.
   *   - Call `event.preventDefault()` for every key handled above, and ONLY those — an unhandled
   *     key must be left alone.
   */
  onKeydown(event: KeyboardEvent): void {
    throw new Error("TODO: implement onKeydown");
  }

  /** TODO: implement selectIndex — sets selectedIndex to `index` AND moves activeIndex there too. */
  selectIndex(index: number): void {
    throw new Error("TODO: implement selectIndex");
  }
}
