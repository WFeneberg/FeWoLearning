import { Component, signal } from "@angular/core";

// Exercise 014 — ProgressBarComponent (beginner).
// Goal:   drive inline styles from state, and get the units right.
// Drills: [style.prop], the [style.prop.unit] suffix, camelCase vs dashed property
//         names, binding a whole style object with [style], and [ngStyle].
// Passes: when `npx jest exercises/01-beginner/ex014_style_binding` is green.
//
// The unit suffix is the point of this exercise. `[style.width]="percent()"` sets width
// to the bare number "42", which CSS ignores. You either write the unit into the value
// yourself or let Angular append it with `[style.width.%]="percent()"` — the second is
// harder to get wrong and keeps the component returning plain numbers.
//
// Property names work either way: [style.background-color] and [style.backgroundColor]
// both land on the same CSS property, and reading it back through el.style always uses
// the camelCase form.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <div class="track">
//     <div class="fill" [style.width.%]="percent()" [style.backgroundColor]="color()"></div>
//   </div>
//
//   <div class="label" [style.fontSize.px]="labelSize()">{{ percent() }}%</div>
//
//   <div class="boxed" [style]="boxStyles()"></div>
//
//   <div class="legacy" [ngStyle]="boxStyles()"></div>

@Component({
  selector: "app-progress-bar",
  standalone: true,
  template: `<p>TODO: render the bar — see the template contract above</p>`,
})
export class ProgressBarComponent {
  readonly value = signal(0);
  readonly total = signal(100);
  readonly labelSize = signal(12);

  /**
   * How far along, 0 to 100, rounded to a whole number.
   *
   * A `total` of zero or less is not a range to be a fraction of — throw a RangeError.
   * A `value` outside 0..total is clamped rather than rejected, because a progress bar
   * that renders 110% is worse than one that sits at the end.
   */
  percent(): number {
    throw new Error("TODO: implement percent");
  }

  /** "crimson" below 34, "orange" below 67, "seagreen" from 67 up. */
  color(): string {
    throw new Error("TODO: implement color");
  }

  /**
   * A style object for the boxed variant, keyed by CSS property name.
   *
   * Exactly two keys: "border-color" set to the current `color()`, and "opacity" set to
   * "0.5" while below 100 and "1" once complete. Values are strings — an object binding
   * has no unit suffix to lean on.
   */
  boxStyles(): Record<string, string> {
    throw new Error("TODO: implement boxStyles");
  }
}
