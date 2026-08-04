import { Component, signal } from "@angular/core";

// Exercise 023 — template reference variables (beginner).
// Goal:   name a DOM element in the template and use it without going through the class.
// Drills: #ref on an element, reading properties off it in an expression, passing it to a
//         handler, calling DOM methods on it, and the limits of template-only scope.
// Passes: when `npx jest exercises/01-beginner/ex023_template_ref_var` is green.
//
// `<input #nameBox>` makes `nameBox` the actual HTMLInputElement for the rest of that
// template. No ElementRef, no query, no class field — which makes it the cheapest way to
// hand one element to a handler on another (`(click)="copyFrom(nameBox)"`).
//
// Two limits worth meeting now. First, the name exists *only* in the template: the class
// cannot see `nameBox`, which is what exercise 024's viewChild() is for. Second, an
// expression like `{{ flagBox.checked }}` is only re-evaluated when change detection runs
// — the DOM property is read at that moment rather than watched, so a change that does not
// trigger a cycle leaves the rendered value stale.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <input #nameBox class="name" value="" />
//   <button class="copy" type="button" (click)="copyFrom(nameBox)">Copy</button>
//   <button class="focus" type="button" (click)="nameBox.focus()">Focus</button>
//   <button class="clear" type="button" (click)="clearVia(nameBox)">Clear</button>
//   <p class="echo">{{ echo() }}</p>
//   <p class="length">{{ nameBox.value.length }}</p>
//
//   <input #flagBox class="flag" type="checkbox" />
//   <p class="flag-state">{{ flagBox.checked ? "on" : "off" }}</p>
@Component({
  selector: "app-echo-box",
  standalone: true,
  template: `<p>TODO: render the box — see the template contract above</p>`,
})
export class EchoBoxComponent {
  readonly echo = signal("");

  /** How many times a copy has been taken. */
  readonly copies = signal(0);

  /**
   * Copy the input's current text into `echo`, trimmed, and count the copy.
   *
   * The parameter is the real HTMLInputElement handed over by the template.
   */
  copyFrom(input: HTMLInputElement): void {
    throw new Error("TODO: implement copyFrom");
  }

  /** Empty the input element itself and reset `echo`. The copy count is left alone. */
  clearVia(input: HTMLInputElement): void {
    throw new Error("TODO: implement clearVia");
  }
}
