import { Component, signal } from "@angular/core";
import { FormControl } from "@angular/forms";

// Exercise 037 — FormControl basics (intermediate).
// Goal:   drive a single input from a reactive FormControl instead of ngModel.
// Drills: a typed FormControl, value / setValue / patchValue / reset, valueChanges as a
//         stream, emitEvent: false, and disable()/enable() with their effect on `value`.
// Passes: when `npx jest exercises/02-intermediate/ex037_form_control_basics` is green.
//
// The difference from exercise 015's [(ngModel)]: there, the template owned the state and the
// class held a plain property. Here the *control* is the state — a real object with a value,
// a validity, a dirty flag and a stream of changes — and the template is bound to it with
// [formControl]. That is what makes reactive forms testable without a DOM at all: most of
// this spec never touches an element.
//
// `new FormControl("", {nonNullable: true})` is worth the extra words. Without it the type is
// `string | null`, because reset() puts the control back to null by default; with it, reset()
// returns to the initial value and the type stays `string`.
//
// Two behaviours that surprise people:
//   - setValue emits on valueChanges. A listener that itself calls setValue will loop unless
//     one side passes {emitEvent: false}.
//   - A disabled control is *excluded* from its parent group's value, and `control.value`
//     still reports the value it holds. Disabled means "not submitted", not "empty".
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <input class="nickname" [formControl]="nickname" />
//   <p class="echo">{{ nickname.value }}</p>
//   <p class="changes">{{ changes().length }}</p>

@Component({
  selector: "app-nickname-field",
  standalone: true,
  // TODO: import ReactiveFormsModule.
  template: `<p>TODO: render the field — see the template contract above</p>`,
})
export class NicknameFieldComponent {
  /**
   * TODO: a non-nullable FormControl<string> starting at "".
   *
   * Declared loosely so the stub compiles — replace the declaration.
   */
  readonly nickname = new FormControl();

  /** Every value seen on the stream, in order. */
  readonly changes = signal<readonly string[]>([]);

  /**
   * Start recording `valueChanges` into `changes`.
   *
   * Called once by the spec. The initial value is not a change, so nothing is recorded
   * until the value actually moves.
   */
  startRecording(): void {
    throw new Error("TODO: implement startRecording");
  }

  /** Set the value, trimmed. Emits on valueChanges as usual. */
  rename(next: string): void {
    throw new Error("TODO: implement rename");
  }

  /** Set the value without notifying anybody — the {emitEvent: false} escape hatch. */
  renameQuietly(next: string): void {
    throw new Error("TODO: implement renameQuietly");
  }

  /** Back to the control's initial value. */
  clear(): void {
    throw new Error("TODO: implement clear");
  }

  /** Whether the control currently holds anything but whitespace. */
  hasValue(): boolean {
    throw new Error("TODO: implement hasValue");
  }
}
