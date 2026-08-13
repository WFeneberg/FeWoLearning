import { Component, Injectable, signal } from "@angular/core";
import { CanDeactivateFn } from "@angular/router";

// Exercise 072 — a functional CanDeactivate guard (advanced).
// Goal:   stop a navigation away from unsaved work, but only ask when there is something to lose.
// Drills: CanDeactivateFn, the component-implements-an-interface pattern it is generic over, a
//         guard injecting a service to ask the user, and short-circuiting the ask when it is not
//         needed.
// Passes: when `npx jest exercises/03-advanced/ex072_route_guard_can_deactivate` is green.
//
// CanDeactivateFn<T> hands the guard the *component instance* being navigated away from, typed as
// whatever interface you declare — here, anything with a canDeactivate() method. The route
// configuration does not need to know which concrete component that is; it only needs the guard,
// and the guard only needs the interface.
//
// The interface's canDeactivate() is a pure question — "am I safe to leave?" — with no side effects
// and no user interaction. Asking the *user* is a separate concern, done only when the answer is
// no, through an injectable wrapper around window.confirm() so a test can substitute a fake answer
// instead of a real modal popping up mid test run.
//
// The short-circuit matters: a clean component should never trigger a confirmation dialog. Calling
// the confirmation service unconditionally would nag the user on every navigation, not just the ones
// that actually lose something.

/** Anything a CanDeactivate guard can ask "is it safe to navigate away from you?" */
export interface CanComponentDeactivate {
  canDeactivate(): boolean;
}

@Injectable({ providedIn: "root" })
export class DiscardConfirmation {
  /** A thin, mockable wrapper — tests substitute this instead of dealing with window.confirm(). */
  confirm(message: string): boolean {
    return window.confirm(message);
  }
}

// Template contract the spec asserts (classes are the query hooks — keep them):
//   <textarea class="note" [value]="text()" (input)="onInput($event)"></textarea>
//   <button class="save" type="button" (click)="save()">Save</button>
@Component({
  selector: "app-note-editor",
  standalone: true,
  template: `<p>TODO: render the editor — see the template contract above</p>`,
})
export class NoteEditorComponent implements CanComponentDeactivate {
  private savedText = "";

  readonly text = signal("");

  onInput(event: Event): void {
    this.text.set((event.target as HTMLTextAreaElement).value);
  }

  /** TODO: whether the current text differs from what was last saved. */
  isDirty(): boolean {
    throw new Error("TODO: implement isDirty");
  }

  /** Record the current text as saved. */
  save(): void {
    throw new Error("TODO: implement save");
  }

  /** TODO: the CanDeactivate contract for this component — true means "safe to leave". */
  canDeactivate(): boolean {
    throw new Error("TODO: implement canDeactivate");
  }
}

/**
 * TODO: implement the guard.
 *
 * A component that is already safe to leave: return true, without asking anything. Otherwise: ask
 * DiscardConfirmation and return whatever it answers.
 */
export const unsavedChangesGuard: CanDeactivateFn<CanComponentDeactivate> = (_component) => {
  throw new Error("TODO: implement unsavedChangesGuard");
};
