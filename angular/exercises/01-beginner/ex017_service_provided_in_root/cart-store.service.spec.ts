import { Component, inject } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { CartStore } from "./cart-store.service";

/** Two unrelated components, both asking for the cart. */
@Component({ selector: "app-a", standalone: true, template: `` })
class ComponentA {
  readonly cart = inject(CartStore);
}

@Component({ selector: "app-b", standalone: true, template: `` })
class ComponentB {
  readonly cart = inject(CartStore);
}

describe("CartStore", () => {
  let store: CartStore;

  beforeEach(() => {
    // No providers array: the service registers itself with providedIn: "root".
    TestBed.configureTestingModule({});
    store = TestBed.inject(CartStore);
  });

  it("starts empty", () => {
    expect(store.items()).toEqual([]);
    expect(store.count()).toBe(0);
  });

  it("adds a line", () => {
    store.add("apple");

    expect(store.items()).toEqual([{ sku: "apple", qty: 1 }]);
    expect(store.count()).toBe(1);
  });

  it("adds a quantity", () => {
    store.add("apple", 3);

    expect(store.count()).toBe(3);
  });

  it("merges a repeated sku", () => {
    store.add("apple", 2);
    store.add("apple", 3);

    expect(store.items()).toEqual([{ sku: "apple", qty: 5 }]);
  });

  it("keeps a merged line in its original position", () => {
    store.add("apple");
    store.add("pear");
    store.add("apple");

    expect(store.items().map((line) => line.sku)).toEqual(["apple", "pear"]);
  });

  it("appends a new sku", () => {
    store.add("apple");
    store.add("pear", 2);

    expect(store.items()).toEqual([
      { sku: "apple", qty: 1 },
      { sku: "pear", qty: 2 },
    ]);
    expect(store.count()).toBe(3);
  });

  it("rejects a non-positive quantity", () => {
    expect(() => store.add("apple", 0)).toThrow(RangeError);
    expect(() => store.add("apple", -1)).toThrow(RangeError);
    expect(store.items()).toEqual([]);
  });

  it("removes a sku", () => {
    store.add("apple");
    store.add("pear");
    store.remove("apple");

    expect(store.items()).toEqual([{ sku: "pear", qty: 1 }]);
  });

  it("ignores removing something absent", () => {
    store.add("apple");
    store.remove("durian");

    expect(store.items()).toEqual([{ sku: "apple", qty: 1 }]);
  });

  it("clears everything", () => {
    store.add("apple", 4);
    store.clear();

    expect(store.items()).toEqual([]);
    expect(store.count()).toBe(0);
  });

  it("hands the same instance to every injector", () => {
    const again = TestBed.inject(CartStore);

    expect(again).toBe(store);
  });

  it("shares state between unrelated components", () => {
    const a = TestBed.createComponent(ComponentA).componentInstance;
    const b = TestBed.createComponent(ComponentB).componentInstance;

    a.cart.add("apple", 2);

    // One instance, so B sees what A did without either knowing about the other.
    expect(b.cart).toBe(a.cart);
    expect(b.cart.count()).toBe(2);
    expect(store.count()).toBe(2);
  });

  it("starts clean in each spec despite being a singleton", () => {
    // TestBed rebuilds the root injector between specs, so nothing leaked in here.
    expect(store.count()).toBe(0);

    store.add("apple");
    expect(store.count()).toBe(1);
  });
});
