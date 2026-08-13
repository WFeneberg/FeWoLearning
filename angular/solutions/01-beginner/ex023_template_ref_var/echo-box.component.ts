import { ChangeDetectionStrategy, Component, signal } from "@angular/core";

// Exercise 023 — template reference variables (reference solution).
//
// changeDetection is explicit here because Angular 22.1.1's JIT compiler compiles an
// omitted `changeDetection` decorator property as OnPush rather than the intended
// CheckAlways default (see @angular/compiler's compileComponentFromMetadata, which only
// emits a `changeDetection` field for an explicit non-OnPush value). `nameBox.value.length`
// and `flagBox.checked` are raw DOM reads, not signals, so without this the template never
// re-evaluates them past the first change-detection pass.
@Component({
  selector: "app-echo-box",
  standalone: true,
  changeDetection: ChangeDetectionStrategy.Default,
  template: `
    <input #nameBox class="name" value="" />
    <!-- The reference is passed to a handler on a *different* element. -->
    <button class="copy" type="button" (click)="copyFrom(nameBox)">Copy</button>
    <!-- A DOM method called straight from the template, with no class involvement. -->
    <button class="focus" type="button" (click)="nameBox.focus()">Focus</button>
    <button class="clear" type="button" (click)="clearVia(nameBox)">Clear</button>
    <p class="echo">{{ echo() }}</p>
    <p class="length">{{ nameBox.value.length }}</p>

    <input #flagBox class="flag" type="checkbox" />
    <p class="flag-state">{{ flagBox.checked ? "on" : "off" }}</p>
  `,
})
export class EchoBoxComponent {
  readonly echo = signal("");

  readonly copies = signal(0);

  copyFrom(input: HTMLInputElement): void {
    this.echo.set(input.value.trim());
    this.copies.update((n) => n + 1);
  }

  clearVia(input: HTMLInputElement): void {
    // Writing to the DOM node directly, because that node *is* what the template named.
    input.value = "";
    this.echo.set("");
  }
}
