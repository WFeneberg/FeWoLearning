import { Component, InjectionToken } from "@angular/core";

// Exercise 068 — element vs environment injectors (intermediate).
// Goal:   predict which provider wins when several are in scope for the same token.
// Drills: an environment-injector default via a token factory, element-injector overrides via a
//         component's `providers` array, walking the element tree before the environment falls
//         back, `skipSelf` to reach past a component's own provider, and `optional` for a token
//         nobody provides anywhere.
// Passes: when `npx jest exercises/02-intermediate/ex068_injector_hierarchies` is green.
//
// Angular keeps two injector trees. The *element* injectors mirror the component tree — one node
// per component that lists something in `providers`. The *environment* injectors are the flatter,
// module-level tree that root services and injectable tokens with `providedIn` live in. A lookup
// walks up the element tree first, component by component, and only consults the environment
// injector once it runs out of ancestors — which is why a component-level provider always beats the
// root default, no matter how deep it is nested.
//
// `skipSelf` does not mean "look further away" in some vague sense — it means "start the walk one
// step above where I am, ignoring whatever I provide myself." A component that provides its own
// value and also wants its *ambient* (ancestor) value uses it deliberately, not by accident: without
// skipSelf, inject() would just hand the component its own provider straight back.
//
// `optional` changes what happens at the very top of the walk. With no provider anywhere and no
// factory default, inject() normally throws NullInjectorError; `{ optional: true }` returns null
// instead. That is the difference between "this dependency is required" and "this dependency may
// simply not apply here."
//
// Every injected value below is read once, in a field initializer — the constructor is where an
// injection context is guaranteed. The methods just return what was already resolved; calling
// inject() from inside a template-bound method would not be safe.

/** Environment-injector default: "[root]" when nothing more specific provides it. */
export const LOG_PREFIX = new InjectionToken<string>("LOG_PREFIX", {
  providedIn: "root",
  factory: () => "[root]",
});

/** A token nobody ever provides — there is no factory, and no default. */
export const FEATURE_FLAG = new InjectionToken<boolean>("FEATURE_FLAG");

@Component({
  selector: "app-leaf",
  standalone: true,
  template: `
    <p class="resolved">{{ resolved() }}</p>
    <p class="flag">{{ flag() === null ? "none" : flag() }}</p>
  `,
})
export class LeafComponent {
  /**
   * TODO: inject LOG_PREFIX normally — the nearest element-injector provider, or the environment
   * default if no ancestor component provides one at all.
   */
  private readonly prefix!: string;

  /** TODO: inject FEATURE_FLAG as optional — null rather than a NullInjectorError. */
  private readonly featureFlag!: boolean | null;

  resolved(): string {
    throw new Error("TODO: implement resolved");
  }

  flag(): boolean | null {
    throw new Error("TODO: implement flag");
  }
}

@Component({
  selector: "app-inner-zone",
  standalone: true,
  imports: [LeafComponent],
  // TODO: provide LOG_PREFIX as "[inner]" for this component and everything nested inside it.
  providers: [],
  template: `
    <p class="own">{{ own() }}</p>
    <p class="ancestor">{{ ancestor() }}</p>
    <app-leaf />
  `,
})
export class InnerZoneComponent {
  /** TODO: inject LOG_PREFIX — resolved from this component's own element injector. */
  private readonly prefix!: string;

  /** TODO: inject LOG_PREFIX with skipSelf — the ancestor's value, not this component's own. */
  private readonly ancestorPrefix!: string;

  own(): string {
    throw new Error("TODO: implement own");
  }

  ancestor(): string {
    throw new Error("TODO: implement ancestor");
  }
}

@Component({
  selector: "app-outer-zone",
  standalone: true,
  imports: [InnerZoneComponent],
  // TODO: provide LOG_PREFIX as "[outer]".
  providers: [],
  template: `
    <p class="own">{{ own() }}</p>
    <p class="ancestor">{{ ancestor() }}</p>
    <app-inner-zone />
  `,
})
export class OuterZoneComponent {
  /** TODO: inject LOG_PREFIX — resolved from this component's own element injector. */
  private readonly prefix!: string;

  /** TODO: inject LOG_PREFIX with skipSelf — no ancestor component provides one, so this should
   *  land on the environment injector's default. */
  private readonly ancestorPrefix!: string;

  own(): string {
    throw new Error("TODO: implement own");
  }

  ancestor(): string {
    throw new Error("TODO: implement ancestor");
  }
}
