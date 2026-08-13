import { Component, InjectionToken, inject } from "@angular/core";

// Exercise 068 — element vs environment injectors (reference solution).

export const LOG_PREFIX = new InjectionToken<string>("LOG_PREFIX", {
  providedIn: "root",
  factory: () => "[root]",
});

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
  private readonly prefix = inject(LOG_PREFIX);
  private readonly featureFlag = inject(FEATURE_FLAG, { optional: true });

  resolved(): string {
    return this.prefix;
  }

  flag(): boolean | null {
    return this.featureFlag;
  }
}

@Component({
  selector: "app-inner-zone",
  standalone: true,
  imports: [LeafComponent],
  providers: [{ provide: LOG_PREFIX, useValue: "[inner]" }],
  template: `
    <p class="own">{{ own() }}</p>
    <p class="ancestor">{{ ancestor() }}</p>
    <app-leaf />
  `,
})
export class InnerZoneComponent {
  private readonly prefix = inject(LOG_PREFIX);
  // skipSelf: ignore this component's own provider, ask the next injector up instead.
  private readonly ancestorPrefix = inject(LOG_PREFIX, { skipSelf: true });

  own(): string {
    return this.prefix;
  }

  ancestor(): string {
    return this.ancestorPrefix;
  }
}

@Component({
  selector: "app-outer-zone",
  standalone: true,
  imports: [InnerZoneComponent],
  providers: [{ provide: LOG_PREFIX, useValue: "[outer]" }],
  template: `
    <p class="own">{{ own() }}</p>
    <p class="ancestor">{{ ancestor() }}</p>
    <app-inner-zone />
  `,
})
export class OuterZoneComponent {
  private readonly prefix = inject(LOG_PREFIX);
  // No ancestor component provides one, so this lands on the environment injector's factory.
  private readonly ancestorPrefix = inject(LOG_PREFIX, { skipSelf: true });

  own(): string {
    return this.prefix;
  }

  ancestor(): string {
    return this.ancestorPrefix;
  }
}
