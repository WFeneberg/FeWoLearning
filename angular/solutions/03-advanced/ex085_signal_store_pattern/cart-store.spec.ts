import { TestBed } from "@angular/core/testing";
import { CartState, CartStore, EMPTY_CART, reduce } from "./cart-store";

describe("reduce (pure reducer)", () => {
  it("appends a new line for an unseen sku", () => {
    const next = reduce(EMPTY_CART, { type: "add", sku: "mug", qty: 2, unitPrice: 5 });

    expect(next.lines).toEqual([{ sku: "mug", qty: 2, unitPrice: 5 }]);
  });

  it("never mutates the state it was given", () => {
    const before = { lines: [{ sku: "mug", qty: 1, unitPrice: 5 }] };
    const snapshot = JSON.parse(JSON.stringify(before));

    reduce(before, { type: "add", sku: "pen", qty: 1, unitPrice: 2 });

    expect(before).toEqual(snapshot);
  });

  it("merges a repeated sku into a single line, summing quantity and taking the latest price", () => {
    let state: CartState = EMPTY_CART;
    state = reduce(state, { type: "add", sku: "mug", qty: 2, unitPrice: 5 });
    state = reduce(state, { type: "add", sku: "mug", qty: 3, unitPrice: 6 });

    expect(state.lines).toEqual([{ sku: "mug", qty: 5, unitPrice: 6 }]);
  });

  it("rejects a qty below 1 on add", () => {
    expect(() => reduce(EMPTY_CART, { type: "add", sku: "mug", qty: 0, unitPrice: 5 })).toThrow(
      RangeError,
    );
  });

  it("removes a line by sku", () => {
    const withTwo = reduce(EMPTY_CART, { type: "add", sku: "mug", qty: 1, unitPrice: 5 });
    const after = reduce(withTwo, { type: "remove", sku: "mug" });

    expect(after.lines).toEqual([]);
  });

  it("treats removing an unknown sku as a no-op", () => {
    const after = reduce(EMPTY_CART, { type: "remove", sku: "nope" });

    expect(after.lines).toEqual([]);
  });

  it("clears back to an empty cart regardless of prior state", () => {
    const withLines = reduce(EMPTY_CART, { type: "add", sku: "mug", qty: 1, unitPrice: 5 });

    expect(reduce(withLines, { type: "clear" })).toEqual(EMPTY_CART);
  });
});

describe("CartStore (dispatch + selectors)", () => {
  let store: CartStore;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    store = TestBed.inject(CartStore);
  });

  it("starts empty, and dispatching add makes it non-empty", () => {
    expect(store.isEmpty()).toBe(true);
    expect(store.itemCount()).toBe(0);
    expect(store.subtotal()).toBe(0);

    store.dispatch({ type: "add", sku: "mug", qty: 1, unitPrice: 5 });

    expect(store.isEmpty()).toBe(false);
  });

  it("dispatching add updates lines, itemCount and subtotal together", () => {
    store.dispatch({ type: "add", sku: "mug", qty: 2, unitPrice: 5 });
    store.dispatch({ type: "add", sku: "pen", qty: 3, unitPrice: 1 });

    expect(store.itemCount()).toBe(5);
    expect(store.subtotal()).toBe(13);
    expect(store.isEmpty()).toBe(false);
  });

  it("dispatching remove drops just that line", () => {
    store.dispatch({ type: "add", sku: "mug", qty: 1, unitPrice: 5 });
    store.dispatch({ type: "add", sku: "pen", qty: 1, unitPrice: 1 });

    store.dispatch({ type: "remove", sku: "mug" });

    expect(store.lines()).toEqual([{ sku: "pen", qty: 1, unitPrice: 1 }]);
  });

  it("dispatching clear empties the cart", () => {
    store.dispatch({ type: "add", sku: "mug", qty: 1, unitPrice: 5 });

    store.dispatch({ type: "clear" });

    expect(store.isEmpty()).toBe(true);
  });
});
