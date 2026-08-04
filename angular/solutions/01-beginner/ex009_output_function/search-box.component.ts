import { Component, output, signal } from "@angular/core";

// Exercise 009 — SearchBoxComponent (reference solution).
@Component({
  selector: "app-search-box",
  standalone: true,
  template: `
    <p class="term">{{ term() }}</p>
    <button class="submit" type="button" (click)="submit()">Search</button>
    <button class="clear" type="button" (click)="clear()">Clear</button>
  `,
})
export class SearchBoxComponent {
  readonly term = signal("");

  readonly submitted = output<string>();

  readonly cleared = output<void>();

  readonly termChanged = output<{ from: string; to: string }>({ alias: "changed" });

  type(next: string): void {
    const from = this.term();
    if (from === next) {
      return;
    }
    this.term.set(next);
    this.termChanged.emit({ from, to: next });
  }

  submit(): void {
    const trimmed = this.term().trim();
    if (trimmed === "") {
      // A blank box is not a search. Emitting here would make parents fetch nothing.
      return;
    }
    this.submitted.emit(trimmed);
  }

  clear(): void {
    if (this.term() === "") {
      return;
    }
    // Reuse type() so the generic "the term moved" notification still goes out...
    this.type("");
    // ...then the specific one. output<void>() emits with no argument at all.
    this.cleared.emit();
  }
}
