import { Component } from "@angular/core";
import { FormArray, FormControl, FormGroup } from "@angular/forms";

// Exercise 039 — FormBuilder (intermediate).
// Goal:   build the same tree as exercise 038 without the `new FormControl` noise.
// Drills: inject(FormBuilder), fb.group / fb.control / fb.array, the array-shorthand for
//         [value, validators], and why NonNullableFormBuilder is usually the one you want.
// Passes: when `npx jest exercises/02-intermediate/ex039_form_builder` is green.
//
// FormBuilder is nothing but sugar: `fb.group({name: ""})` produces exactly the FormGroup you
// would have written by hand. What it buys is readability once a form has more than three
// fields — the shape of the data stays visible instead of being buried in constructor calls.
//
// The shorthand `["", [Validators.required]]` means [initialValue, validators]. It is compact
// and it is also the one thing that trips people up, because `fb.group({tags: ["a", "b"]})`
// reads like two initial values and is actually parsed as value "a" with "b" as validators.
// Use `fb.control(["a", "b"])` when the value itself is an array.
//
// `fb.nonNullable` is the builder whose controls are non-nullable, so reset() returns to the
// initial value rather than null and the types come out as `string` rather than `string | null`.
// Plain `fb` gives nullable controls, which is the historical default rather than the good one.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <form [formGroup]="form">
//     <input class="email" formControlName="email" />
//     <div formGroupName="profile">
//       <input class="first" formControlName="firstName" />
//       <input class="last" formControlName="lastName" />
//     </div>
//   </form>
//   <p class="tags">{{ tagList().length }}</p>

@Component({
  selector: "app-signup-form",
  standalone: true,
  // TODO: import ReactiveFormsModule.
  template: `<p>TODO: render the form — see the template contract above</p>`,
})
export class SignupFormComponent {
  /**
   * TODO: build this with the non-nullable FormBuilder, injected with inject():
   *
   *   email: ""
   *   profile: { firstName: "", lastName: "" }
   *   tags: a FormArray, initially empty
   *
   * Declared empty so the stub compiles — replace the declaration.
   */
  readonly form = new FormGroup({});

  /** The nested profile group. */
  profileGroup(): FormGroup {
    throw new Error("TODO: implement profileGroup");
  }

  /** The tags FormArray. */
  tagArray(): FormArray {
    throw new Error("TODO: implement tagArray");
  }

  /** The tag values, in order. */
  tagList(): readonly string[] {
    throw new Error("TODO: implement tagList");
  }

  /** Append a tag control. A blank tag is a RangeError. */
  addTag(tag: string): void {
    throw new Error("TODO: implement addTag");
  }

  /** Remove the tag at an index. An out-of-range index is a RangeError. */
  removeTag(index: number): void {
    throw new Error("TODO: implement removeTag");
  }

  /**
   * TODO: a standalone control whose *value* is an array of two strings.
   *
   * Built with fb.control, not the group shorthand — `["a", "b"]` inside fb.group would be
   * read as [value, validators] and quietly produce the control value "a".
   */
  pairControl(): FormControl {
    throw new Error("TODO: implement pairControl");
  }

  /** `"<first> <last> <email>"` with blanks squeezed out. */
  describe(): string {
    throw new Error("TODO: implement describe");
  }
}
