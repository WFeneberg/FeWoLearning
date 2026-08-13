import { Component, forwardRef, signal } from "@angular/core";
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from "@angular/forms";

// Exercise 079 — ControlValueAccessor: a custom form control (advanced).
// Goal:   let `[(ngModel)]` / `formControl` treat a hand-built star-rating widget exactly like a
//         native `<input>` — the form layer should not need to know this isn't one.
// Drills: ControlValueAccessor's four methods, NG_VALUE_ACCESSOR, and the two directions a custom
//         control has to keep in sync: model → view and view → model.
// Passes: when `npx jest exercises/03-advanced/ex079_control_value_accessor` is green.
//
// Reactive forms never touch the DOM directly — every native or custom control sits behind a
// ControlValueAccessor, which is the only thing the form actually talks to. That is what makes a
// FormControl agnostic to whether it is bound to an `<input>`, a `<select>`, or a `StarRating`:
// the accessor is the adapter, and its job is exactly two conversions.
//
// Model → view is `writeValue()`. Angular calls it once when the control is first wired up (to
// push whatever initial value the FormControl already holds) and again on every `setValue()` /
// `patchValue()` after that — including calls that did not come from this widget's own UI at all.
// Skipping it means a form reset or a `patchValue()` from elsewhere in the app would silently
// leave the stars showing the old rating.
//
// View → model is the other half: when the user clicks a star, nothing updates the FormControl by
// itself — a plain click handler mutating this component's own signal would only ever change what
// this component displays, leaving the FormControl (and any validators, or a submit handler
// reading its value) none the wiser. `registerOnChange()` hands over the one function that closes
// that loop: calling it is what tells the form "the value is now this." The matching
// `registerOnTouched()` callback exists for the same reason on the "has this been interacted with"
// axis — validators like a "required, but only after interaction" rule depend on it firing.

@Component({
  selector: "app-star-rating",
  standalone: true,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => StarRatingComponent),
      multi: true,
    },
  ],
  template: `
    <div class="stars" role="radiogroup" [attr.aria-disabled]="disabled()">
      @for (star of stars; track star) {
        <button
          type="button"
          class="star"
          [class.filled]="star <= value()"
          [attr.aria-checked]="star === value()"
          [disabled]="disabled()"
          (click)="selectStar(star)"
        >
          ★
        </button>
      }
    </div>
  `,
})
export class StarRatingComponent implements ControlValueAccessor {
  protected readonly stars = [1, 2, 3, 4, 5];

  /** The current rating. Public (not private) so this spec can assert on it directly. */
  readonly value = signal(0);
  readonly disabled = signal(false);

  private onChange: (value: number) => void = () => {};
  private onTouched: () => void = () => {};

  /**
   * TODO: implement writeValue — this is the model → view direction. Angular calls this whenever
   * the bound FormControl's value changes for any reason, including ones that did not originate
   * from a click in this component (e.g. `patchValue()` from elsewhere).
   */
  writeValue(value: number): void {
    throw new Error("TODO: implement writeValue");
  }

  registerOnChange(fn: (value: number) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  /**
   * TODO: implement selectStar — this is the view → model direction. A click must update this
   * component's own displayed value *and* call the two registered callbacks, or the FormControl
   * this widget is bound to will never learn a rating was picked. Ignore clicks while disabled.
   */
  protected selectStar(star: number): void {
    throw new Error("TODO: implement selectStar");
  }
}
