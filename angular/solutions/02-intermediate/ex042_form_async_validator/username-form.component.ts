import { Component, inject, Injectable } from "@angular/core";
import {
  AbstractControl,
  AsyncValidatorFn,
  FormControl,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from "@angular/forms";
import { first, map, Observable } from "rxjs";

// Exercise 042 — asynchronous validators (reference solution).

@Injectable({ providedIn: "root" })
export class UsernameService {
  readonly queries: string[] = [];

  private readonly resolvers: Array<(taken: boolean) => void> = [];

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

  resolve(taken: boolean): void {
    const next = this.resolvers.shift();
    if (next === undefined) {
      throw new Error("resolve() called with no outstanding query");
    }
    next(taken);
  }
}

export function uniqueUsername(service: UsernameService): AsyncValidatorFn {
  return (control: AbstractControl): Observable<ValidationErrors | null> => {
    const name = String(control.value ?? "");
    return service.isTaken(name).pipe(
      map((taken) => (taken ? { taken: { name } } : null)),
      // Angular waits for *completion*, not merely an emission. Without this, a stream that
      // stays open leaves the control PENDING forever, with no error to debug.
      first(),
    );
  };
}

@Component({
  selector: "app-username-form",
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <input class="username" [formControl]="username" />
    <p class="status">{{ statusLabel() }}</p>
    <button class="submit" type="button" [disabled]="!canSubmit()">Submit</button>
  `,
})
export class UsernameFormComponent {
  private readonly service = inject(UsernameService);

  readonly username = new FormControl("", {
    nonNullable: true,
    validators: [Validators.required],
    // The third slot. An async validator passed as a sync one is a silent no-op.
    asyncValidators: [uniqueUsername(this.service)],
  });

  statusLabel(): string {
    if (this.username.pending) {
      return "checking…";
    }
    if (this.username.hasError("required")) {
      return "required";
    }
    return this.username.hasError("taken") ? "taken" : "free";
  }

  canSubmit(): boolean {
    // `valid` is false while PENDING, which is exactly what is wanted here — a button wired
    // to `!invalid` instead would be enabled mid-flight.
    return this.username.valid;
  }

  takenName(): string | null {
    const error = this.username.getError("taken") as { name: string } | undefined;
    return error?.name ?? null;
  }
}
