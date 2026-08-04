import { Component, inject, Injectable, signal } from "@angular/core";

// Exercise 035 — TestBed basics (reference solution).

@Injectable({ providedIn: "root" })
export class Clock {
  now(): string {
    return new Date().toISOString();
  }
}

@Component({
  selector: "app-welcome",
  standalone: true,
  template: `
    <h2 class="greeting">{{ greeting() }}</h2>
    <p class="stamp">{{ stamp() }}</p>
    <button class="refresh" type="button" (click)="refresh()">Refresh</button>
  `,
})
export class WelcomeComponent {
  // Asking for the type is all the component does; what it actually receives is the test's
  // decision, which is what makes the non-deterministic dependency testable.
  private readonly clock = inject(Clock);

  readonly name = signal("world");

  readonly stamp = signal("");

  readonly refreshes = signal(0);

  greeting(): string {
    const name = this.name();
    const capitalised = name.charAt(0).toUpperCase() + name.slice(1);
    return `Hello, ${capitalised}!`;
  }

  refresh(): void {
    this.stamp.set(this.clock.now());
    this.refreshes.update((n) => n + 1);
  }
}
