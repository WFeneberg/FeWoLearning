import { Component, forwardRef, signal } from "@angular/core";
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from "@angular/forms";

// Exercise 079 — ControlValueAccessor: a custom form control (reference solution).

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

  readonly value = signal(0);
  readonly disabled = signal(false);

  private onChange: (value: number) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: number): void {
    this.value.set(value ?? 0);
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

  protected selectStar(star: number): void {
    if (this.disabled()) {
      return;
    }
    this.value.set(star);
    this.onChange(star);
    this.onTouched();
  }
}
