import { Component, Injectable } from "@angular/core";
import { AsyncValidatorFn, FormControl } from "@angular/forms";
import { Observable } from "rxjs";

// Exercise 042 — asynchronous validators (intermediate).
// Goal:   validate against something that has to be asked, and handle the wait.
// Drills: AsyncValidatorFn, the PENDING status, why the observable must complete, and the fact
//         that async validators only run once the synchronous ones are happy.
// Passes: when `npx jest exercises/02-intermediate/ex042_form_async_validator` is green.
//
// An AsyncValidatorFn returns an Observable or Promise of the usual `ValidationErrors | null`.
// Between the value changing and the answer arriving, the control's status is neither VALID nor
// INVALID but **PENDING** — a third state the UI has to account for, because a submit button
// wired only to `invalid` will happily submit mid-flight.
//
// The requirement that catches everyone: Angular waits for the observable to **complete**, not
// merely to emit. A validator built on a long-lived stream — a Subject, an interval, anything
// still open — leaves the control PENDING forever, with no error and no clue. `first()` or
// `take(1)` is the fix.
//
// The ordering is a deliberate optimisation: async validators do not run while a synchronous
// validator is failing. There is no point asking a server whether "" is taken. So a control
// with `required` plus this validator goes straight to INVALID when emptied, without a query.
//
// The service below is already written. It never resolves on its own — the spec calls
// `resolve()` to answer each query, which is what makes the PENDING window observable.

@Injectable({ providedIn: "root" })
export class UsernameService {
  /** Every name asked about, in order — so a test can assert what was and was not queried. */
  readonly queries: string[] = [];

  private readonly resolvers: Array<(taken: boolean) => void> = [];

  /** How many queries are still waiting for an answer. */
  get outstanding(): number {
    return this.resolvers.length;
  }

  isTaken(name: string): Observable<boolean> {
    this.queries.push(name);
    return new Observable<boolean>((subscriber) => {
      this.resolvers.push((taken) => {
        subscriber.next(taken);
        subscriber.complete();
      });
    });
  }

  /** Answer the oldest outstanding query. */
  resolve(taken: boolean): void {
    const next = this.resolvers.shift();
    if (next === undefined) {
      throw new Error("resolve() called with no outstanding query");
    }
    next(taken);
  }
}

/**
 * TODO: an async validator rejecting names the service reports as taken.
 *
 * Returns `{taken: {name}}` when taken and null when free. Make sure the observable you hand
 * back completes — Angular waits for completion, and a stream that stays open leaves the
 * control PENDING with no error and nothing to debug.
 */
export function uniqueUsername(service: UsernameService): AsyncValidatorFn {
  throw new Error("TODO: implement uniqueUsername");
}

// Template contract the spec asserts (classes are the query hooks — keep them):
//   <input class="username" [formControl]="username" />
//   <p class="status">{{ statusLabel() }}</p>
//   <button class="submit" type="button" [disabled]="!canSubmit()">Submit</button>
@Component({
  selector: "app-username-form",
  standalone: true,
  // TODO: import ReactiveFormsModule.
  template: `<p>TODO: render the form — see the template contract above</p>`,
})
export class UsernameFormComponent {
  /**
   * TODO: a non-nullable FormControl<string> starting at "", with Validators.required as its
   * synchronous validator and uniqueUsername as its async one.
   *
   * Async validators are the *third* constructor argument, or the `asyncValidators` option —
   * passing one in the sync slot is a silent no-op. Get the service with inject().
   *
   * Declared plainly so the stub compiles — replace the declaration.
   */
  readonly username = new FormControl("", { nonNullable: true });

  /** "checking…" while pending, "taken" / "free" once known, "required" when empty. */
  statusLabel(): string {
    throw new Error("TODO: implement statusLabel");
  }

  /** Submit is allowed only when the control is genuinely valid — never while pending. */
  canSubmit(): boolean {
    throw new Error("TODO: implement canSubmit");
  }

  /** The name the control reported as taken, or null. */
  takenName(): string | null {
    throw new Error("TODO: implement takenName");
  }
}
