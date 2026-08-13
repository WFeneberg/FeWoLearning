import { Component, inject } from "@angular/core";
import { ActivatedRoute } from "@angular/router";

// Exercise 084 — RouterTestingHarness: navigating routed components in tests (advanced).
// Goal:   test a routed component the way it is actually reached — by navigating a real `Router`
//         to a URL — instead of hand-constructing an `ActivatedRoute` stub and hoping it matches
//         what the router would really produce.
// Drills: `provideRouter(routes)`, `RouterTestingHarness.create()`, `navigateByUrl(url, type)`, and
//         reading route params from an injected `ActivatedRoute` inside the routed component.
// Passes: when `npx jest exercises/03-advanced/ex084_testing_router_harness` is green.
//
// Exercises 062–064 read route params directly off a hand-built `ActivatedRoute`. That is fine for
// a component in isolation, but it proves nothing about whether the *route configuration* actually
// wires that component up correctly — a typo in a path or a param name would go unnoticed. Routing
// this through a real, testing-configured `Router` closes that gap: `provideRouter(routes)` installs
// the exact same routing machinery the app uses at runtime, just pointed at an in-memory location
// instead of the browser's.
//
// `RouterTestingHarness` exists to remove the remaining boilerplate of driving that router: it
// creates its own host component with a `RouterOutlet`, and `navigateByUrl(url, type)` triggers a
// real navigation, waits for it to complete, asserts the outlet activated the *expected* component
// type, and hands that instance straight back. There is only ever one harness per test — call
// `.create()` again in the same test and it throws — but a single harness can `navigateByUrl` as
// many times as a test needs, reusing its outlet, which is exactly what the "same harness, second
// navigation updates the component" case below exercises.
//
// Route params only exist once real navigation has happened, so UserDetailComponent's `label` reads
// them lazily through `ActivatedRoute.snapshot`. It is a plain method, not a `computed()` — the same
// routed component instance is reused across navigations to the same route (only its params change),
// and `route.snapshot` is not a signal read, so a `computed()` here would cache its first answer
// forever instead of noticing the second navigation. Reading the snapshot fresh on every call is
// what makes the "same harness, second navigation" case below observe the new id.

@Component({
  selector: "app-user-detail",
  standalone: true,
  template: `<p class="label">{{ label() }}</p>`,
})
export class UserDetailComponent {
  private readonly route = inject(ActivatedRoute);

  /**
   * TODO: implement label — read the `id` route param off `this.route.snapshot.paramMap` and
   * return it formatted as the string `User <id>`.
   */
  label(): string {
    throw new Error("TODO: implement label");
  }
}

@Component({
  selector: "app-not-found",
  standalone: true,
  template: `<p class="not-found">User not found</p>`,
})
export class NotFoundComponent {}
