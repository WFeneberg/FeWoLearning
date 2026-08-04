import { Component, inject } from "@angular/core";
import { FormArray, FormControl, FormGroup, NonNullableFormBuilder, ReactiveFormsModule } from "@angular/forms";

// Exercise 039 — FormBuilder (reference solution).
@Component({
  selector: "app-signup-form",
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <form [formGroup]="form">
      <input class="email" formControlName="email" />
      <div formGroupName="profile">
        <input class="first" formControlName="firstName" />
        <input class="last" formControlName="lastName" />
      </div>
    </form>
    <p class="tags">{{ tagList().length }}</p>
  `,
})
export class SignupFormComponent {
  // The non-nullable builder: reset() returns to the initial value and the types stay
  // `string` rather than `string | null`.
  private readonly fb = inject(NonNullableFormBuilder);

  readonly form = this.fb.group({
    email: "",
    profile: this.fb.group({
      firstName: "",
      lastName: "",
    }),
    tags: this.fb.array<string>([]),
  });

  profileGroup(): FormGroup {
    return this.form.controls.profile;
  }

  tagArray(): FormArray {
    return this.form.controls.tags;
  }

  tagList(): readonly string[] {
    return this.tagArray().value as readonly string[];
  }

  addTag(tag: string): void {
    const trimmed = tag.trim();
    if (trimmed === "") {
      throw new RangeError("tag must not be blank");
    }
    this.tagArray().push(this.fb.control(trimmed));
  }

  removeTag(index: number): void {
    if (index < 0 || index >= this.tagArray().length) {
      throw new RangeError(`no tag at index ${index}`);
    }
    this.tagArray().removeAt(index);
  }

  pairControl(): FormControl {
    // fb.control, not the group shorthand. Inside fb.group({pair: ["a", "b"]}) this array
    // would be read as [initialValue, validators] and the value would silently become "a".
    return this.fb.control(["a", "b"]);
  }

  describe(): string {
    const { email, profile } = this.form.getRawValue();
    return [profile.firstName, profile.lastName, email]
      .filter((part) => part !== "")
      .join(" ");
  }
}
