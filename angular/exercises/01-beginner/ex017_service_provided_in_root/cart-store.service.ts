import { Injectable, signal } from "@angular/core";

// Exercise 017 — CartStore (beginner).
// Goal:   share one instance of a service across everything that asks for it.
// Drills: @Injectable({providedIn: "root"}), singleton scope, tree-shakability, and
//         holding state in a service rather than in a component.
// Passes: when `npx jest exercises/01-beginner/ex017_service_provided_in_root` is green.
//
// `providedIn: "root"` registers the service with the application's root injector, so
// every injector below it resolves to the *same* instance — that is what makes a service
// a natural home for shared state. It is also tree-shakable: the class names its own
// provider, so a service nobody injects is dropped from the bundle. Listing it in a
// module's `providers` array instead would keep it in the bundle whether used or not.
//
// In a test, TestBed *is* the root injector, and it is rebuilt between specs — which is
// why state does not leak from one `it` to the next even though the service is a
// singleton within each.

export interface CartLine {
  readonly sku: string;
  readonly qty: number;
}

// TODO: register this with the root injector.
@Injectable()
export class CartStore {
  private readonly lines = signal<readonly CartLine[]>([]);

  /** Everything in the cart, in insertion order. */
  items(): readonly CartLine[] {
    throw new Error("TODO: implement items");
  }

  /** Total quantity across all lines. */
  count(): number {
    throw new Error("TODO: implement count");
  }

  /**
   * Add `qty` of a sku.
   *
   * An existing sku has its quantity increased in place, keeping its original position;
   * a new sku is appended. A `qty` below 1 is a RangeError.
   */
  add(sku: string, qty = 1): void {
    throw new Error("TODO: implement add");
  }

  /** Drop a sku entirely. Removing something absent is a no-op, not an error. */
  remove(sku: string): void {
    throw new Error("TODO: implement remove");
  }

  /** Empty the cart. */
  clear(): void {
    throw new Error("TODO: implement clear");
  }
}
