import { Component, inject, Injectable, signal } from "@angular/core";

// Exercise 035 — TestBed basics (beginner).
// Goal:   understand the harness every other exercise in this tier has been using.
// Drills: TestBed.configureTestingModule, TestBed.inject, createComponent, the
//         ComponentFixture surface, and swapping a real dependency for a fake with a provider.
// Passes: when `npx jest exercises/01-beginner/ex035_testbed_basics` is green.
//
// TestBed builds a miniature Angular application: an injector, a change-detection cycle and
// somewhere to render. `configureTestingModule({providers, imports})` is the composition
// step, and it is where a test decides what the component under test will actually receive.
// That last part is the point of this exercise — providing `{provide: Clock, useValue: fake}`
// gives you a component wired to a clock you control, without the component knowing.
//
// The fixture is the handle on the result:
//   componentInstance — the class, for calling methods and reading state
//   nativeElement     — the host DOM element
//   debugElement      — the Angular-side view, for queries and child injectors
//   detectChanges()   — run change detection, i.e. re-render
//   destroy()         — tear down, running ngOnDestroy and DestroyRef callbacks
//
// The habit worth forming: nothing renders until detectChanges() runs. A test that reads the
// DOM after changing state without calling it is testing the previous render.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <h2 class="greeting">{{ greeting() }}</h2>
//   <p class="stamp">{{ stamp() }}</p>
//   <button class="refresh" type="button" (click)="refresh()">Refresh</button>

/** A dependency worth faking: the real one is non-deterministic. */
@Injectable({ providedIn: "root" })
export class Clock {
  /** The current time, as an ISO-8601 string. */
  now(): string {
    return new Date().toISOString();
  }
}

@Component({
  selector: "app-welcome",
  standalone: true,
  template: `<p>TODO: render the welcome — see the template contract above</p>`,
})
export class WelcomeComponent {
  /** TODO: inject the Clock. */
  private readonly clock!: Clock;

  readonly name = signal("world");

  /** The timestamp captured by the last refresh, or "" before the first one. */
  readonly stamp = signal("");

  /** How many times refresh() has run. */
  readonly refreshes = signal(0);

  /** `"Hello, world!"` — capitalised name, so `signal("ada")` reads "Hello, Ada!". */
  greeting(): string {
    throw new Error("TODO: implement greeting");
  }

  /** Ask the clock for the time, store it in `stamp`, and count the refresh. */
  refresh(): void {
    throw new Error("TODO: implement refresh");
  }
}
