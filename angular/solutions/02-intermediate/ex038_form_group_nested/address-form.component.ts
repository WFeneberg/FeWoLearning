import { ChangeDetectionStrategy, Component } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";

// Exercise 038 — nested FormGroups (reference solution).
//
// changeDetection is explicit here because Angular 22.1.1's JIT compiler compiles an
// omitted `changeDetection` decorator property as OnPush rather than the intended
// CheckAlways default (see @angular/compiler's compileComponentFromMetadata). Reactive
// forms push value changes through RxJS/zone patching, not the signal graph, so
// `{{ summary() }}` needs CheckAlways to be re-read after a plain form mutation.
@Component({
  selector: "app-address-form",
  standalone: true,
  imports: [ReactiveFormsModule],
  changeDetection: ChangeDetectionStrategy.Default,
  template: `
    <form [formGroup]="form">
      <input class="name" formControlName="name" />
      <div formGroupName="address">
        <input class="street" formControlName="street" />
        <input class="city" formControlName="city" />
        <input class="zip" formControlName="zip" />
      </div>
    </form>
    <p class="summary">{{ summary() }}</p>
  `,
})
export class AddressFormComponent {
  readonly form = new FormGroup({
    name: new FormControl("", { nonNullable: true }),
    // A group inside a group, so `form.value` comes back nested to match.
    address: new FormGroup({
      street: new FormControl("", { nonNullable: true }),
      city: new FormControl("", { nonNullable: true }),
      zip: new FormControl("", { nonNullable: true }),
    }),
  });

  addressGroup(): FormGroup {
    return this.form.get("address") as FormGroup;
  }

  controlAt(path: string): FormControl {
    const control = this.form.get(path);
    if (control === null) {
      throw new Error(`no control at "${path}"`);
    }
    return control as FormControl;
  }

  summary(): string {
    // getRawValue(), not value: a disabled control should still show up in a summary.
    const { name, address } = this.form.getRawValue();
    const location = [address.zip, address.city].filter((part) => part !== "").join(" ");
    return [name, address.street, location].filter((part) => part !== "").join(", ");
  }

  replaceAll(value: {
    name: string;
    address: { street: string; city: string; zip: string };
  }): void {
    // Strict: a missing key throws, which is the right behaviour for replacing a record.
    this.form.setValue(value);
  }

  applyPatch(changes: {
    name?: string;
    address?: { street?: string; city?: string; zip?: string };
  }): void {
    // Lenient: applies what it recognises at any depth, and silently ignores the rest.
    this.form.patchValue(changes);
  }

  payload(): unknown {
    // `value` omits disabled controls entirely, which would quietly drop fields from a save.
    return this.form.getRawValue();
  }
}
