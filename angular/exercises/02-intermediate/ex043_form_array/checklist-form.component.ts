import { Component } from "@angular/core";
import { FormArray, FormGroup } from "@angular/forms";

// Exercise 043 — FormArray (intermediate).
// Goal:   manage a list of controls whose length is decided at runtime.
// Drills: fb.array of FormGroups, push / removeAt / insert / clear, at(), reading .controls in
//         a template with formArrayName + [formGroupName]="$index", and the .length trap.
// Passes: when `npx jest exercises/02-intermediate/ex043_form_array` is green.
//
// A FormGroup has a fixed set of named children; a FormArray has an ordered, growable list of
// numbered ones. Anything the user can add rows to needs the array — and the elements can be
// whole FormGroups, which is what makes a row with several fields work.
//
// Two things to get right. In the template, iterate `items().controls`, not the array itself,
// and bind each row with `[formGroupName]="$index"` — the index *is* the control name, which is
// why removing a row renumbers everything after it. And `track` on a FormGroup reference is
// safe precisely because those references survive reordering; tracking `$index` here would
// rebuild every row below an insertion.
//
// The trap: `items().length` is the number of *controls*, while `items().value.length` is the
// number of values — normally identical, but a disabled row is missing from `value`, so the two
// disagree exactly when you are least expecting it.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <form [formGroup]="form">
//     <input class="title" formControlName="title" />
//     <div formArrayName="items">
//       @for (item of items().controls; track item; let i = $index) {
//         <div class="item" [formGroupName]="i">
//           <input class="label" formControlName="label" />
//           <input class="done" type="checkbox" formControlName="done" />
//         </div>
//       }
//     </div>
//   </form>
//   <p class="count">{{ doneCount() }}/{{ items().length }}</p>

@Component({
  selector: "app-checklist-form",
  standalone: true,
  // TODO: import ReactiveFormsModule.
  template: `<p>TODO: render the checklist — see the template contract above</p>`,
})
export class ChecklistFormComponent {
  /**
   * TODO: with the non-nullable FormBuilder (inject it):
   *
   *   title: ""
   *   items: a FormArray, initially empty, whose elements are groups of
   *          { label: string, done: boolean }
   *
   * Declared empty so the stub compiles — replace the declaration.
   */
  readonly form = new FormGroup({});

  /** The items FormArray. */
  items(): FormArray {
    throw new Error("TODO: implement items");
  }

  /** One row's group, by index. An out-of-range index is a RangeError. */
  itemAt(index: number): FormGroup {
    throw new Error("TODO: implement itemAt");
  }

  /** Every row's label, in order. */
  labels(): readonly string[] {
    throw new Error("TODO: implement labels");
  }

  /** How many rows are ticked. */
  doneCount(): number {
    throw new Error("TODO: implement doneCount");
  }

  /** Append a row, not done. A blank label is a RangeError. */
  addItem(label: string): void {
    throw new Error("TODO: implement addItem");
  }

  /** Insert a row at a position, pushing the rest down. Out of range is a RangeError. */
  insertItem(index: number, label: string): void {
    throw new Error("TODO: implement insertItem");
  }

  /** Remove one row. Out of range is a RangeError. */
  removeAt(index: number): void {
    throw new Error("TODO: implement removeAt");
  }

  /** Tick or untick one row. */
  setDone(index: number, done: boolean): void {
    throw new Error("TODO: implement setDone");
  }

  /** Drop every ticked row, keeping the rest in order. */
  clearDone(): void {
    throw new Error("TODO: implement clearDone");
  }

  /** Empty the list entirely. */
  clearAll(): void {
    throw new Error("TODO: implement clearAll");
  }
}
