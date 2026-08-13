import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { FormArray, FormGroup, NonNullableFormBuilder, ReactiveFormsModule } from "@angular/forms";

// Exercise 043 — FormArray (reference solution).
//
// changeDetection is explicit here because Angular 22.1.1's JIT compiler compiles an
// omitted `changeDetection` decorator property as OnPush rather than the intended
// CheckAlways default (see @angular/compiler's compileComponentFromMetadata). Reactive
// forms push value changes through RxJS/zone patching, not the signal graph, so the
// `@for (item of items().controls; ...)` block and `{{ doneCount() }}` need CheckAlways
// to be re-read after a plain FormArray mutation (push/insert/removeAt/clear).
@Component({
  selector: "app-checklist-form",
  standalone: true,
  imports: [ReactiveFormsModule],
  changeDetection: ChangeDetectionStrategy.Default,
  template: `
    <form [formGroup]="form">
      <input class="title" formControlName="title" />
      <div formArrayName="items">
        <!-- Iterate .controls, and track the FormGroup reference: those references survive
             renumbering, so a moved row keeps its DOM node. -->
        @for (item of items().controls; track item; let i = $index) {
          <div class="item" [formGroupName]="i">
            <input class="label" formControlName="label" />
            <input class="done" type="checkbox" formControlName="done" />
          </div>
        }
      </div>
    </form>
    <p class="count">{{ doneCount() }}/{{ items().length }}</p>
  `,
})
export class ChecklistFormComponent {
  private readonly fb = inject(NonNullableFormBuilder);

  readonly form = this.fb.group({
    title: "",
    items: this.fb.array<FormGroup>([]),
  });

  items(): FormArray {
    return this.form.controls.items;
  }

  itemAt(index: number): FormGroup {
    if (index < 0 || index >= this.items().length) {
      throw new RangeError(`no item at index ${index}`);
    }
    return this.items().at(index) as FormGroup;
  }

  labels(): readonly string[] {
    // getRawValue(), so a disabled row still reports its label.
    return (this.items().getRawValue() as Array<{ label: string }>).map((item) => item.label);
  }

  doneCount(): number {
    return (this.items().getRawValue() as Array<{ done: boolean }>).filter(
      (item) => item.done,
    ).length;
  }

  private buildItem(label: string): FormGroup {
    return this.fb.group({ label, done: false });
  }

  addItem(label: string): void {
    const trimmed = label.trim();
    if (trimmed === "") {
      throw new RangeError("label must not be blank");
    }
    this.items().push(this.buildItem(trimmed));
  }

  insertItem(index: number, label: string): void {
    // <= length, not < length: inserting *at* the end is legitimate.
    if (index < 0 || index > this.items().length) {
      throw new RangeError(`cannot insert at index ${index}`);
    }
    const trimmed = label.trim();
    if (trimmed === "") {
      throw new RangeError("label must not be blank");
    }
    this.items().insert(index, this.buildItem(trimmed));
  }

  removeAt(index: number): void {
    if (index < 0 || index >= this.items().length) {
      throw new RangeError(`no item at index ${index}`);
    }
    // Everything after this point is renumbered, because the index *is* the control name.
    this.items().removeAt(index);
  }

  setDone(index: number, done: boolean): void {
    this.itemAt(index).controls["done"].setValue(done);
  }

  clearDone(): void {
    // Backwards, so removing a row does not shift the indices still to be examined.
    for (let index = this.items().length - 1; index >= 0; index -= 1) {
      if (this.itemAt(index).controls["done"].value === true) {
        this.items().removeAt(index);
      }
    }
  }

  clearAll(): void {
    this.items().clear();
  }
}
