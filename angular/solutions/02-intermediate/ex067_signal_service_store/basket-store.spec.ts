import { TestBed } from "@angular/core/testing";
import { BasketStore, EMPTY_BASKET } from "./basket-store";

describe("BasketStore", () => {
  let store: BasketStore;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    store = TestBed.inject(BasketStore);
  });

  it("starts empty and fills up", () => {
    expect(store.isEmpty()).toBe(true);
    expect(store.itemCount()).toBe(0);
    expect(store.subtotal()).toBe(0);

    store.add("pen", 2, 1.5);

    expect(store.isEmpty()).toBe(false);
    expect(store.itemCount()).toBe(2);
    expect(store.subtotal()).toBe(3);
  });

  it("never holds the shared constant", () => {
    expect(store.state()).not.toBe(EMPTY_BASKET);

    store.add("pen", 1, 1);

    expect(EMPTY_BASKET.lines).toEqual([]);
  });

  it("merges a duplicate sku rather than appending", () => {
    store.add("pen", 2, 1.5);
    store.add("pen", 3, 1.5);

    expect(store.lines()).toEqual([{ sku: "pen", qty: 5, unitPrice: 1.5 }]);
  });

  it("keeps a merged line in place", () => {
    store.add("pen", 1, 1);
    store.add("pad", 1, 2);
    store.add("pen", 1, 1);

    expect(store.lines().map((line) => line.sku)).toEqual(["pen", "pad"]);
  });

  it("appends a new sku", () => {
    store.add("pen", 1, 1.5);
    store.add("pad", 2, 4);

    expect(store.itemCount()).toBe(3);
    expect(store.subtotal()).toBe(9.5);
  });

  it("refuses a quantity below one", () => {
    expect(() => store.add("pen", 0, 1)).toThrow(RangeError);
    expect(() => store.add("pen", -1, 1)).toThrow(RangeError);
    expect(store.lines()).toEqual([]);
  });

  it("removes a sku", () => {
    store.add("pen", 1, 1);
    store.add("pad", 1, 2);

    store.remove("pen");

    expect(store.lines().map((line) => line.sku)).toEqual(["pad"]);
  });

  it("ignores removing an unknown sku", () => {
    store.add("pen", 1, 1);

    store.remove("nope");

    expect(store.lines()).toHaveLength(1);
  });

  it("sets a quantity", () => {
    store.add("pen", 1, 2);

    store.setQuantity("pen", 4);

    expect(store.itemCount()).toBe(4);
    expect(store.subtotal()).toBe(8);
  });

  it("removes a line set to zero", () => {
    store.add("pen", 1, 2);

    store.setQuantity("pen", 0);

    expect(store.lines()).toEqual([]);
    expect(store.isEmpty()).toBe(true);
  });

  it("refuses a negative quantity", () => {
    store.add("pen", 1, 2);

    expect(() => store.setQuantity("pen", -1)).toThrow(RangeError);
    expect(store.itemCount()).toBe(1);
  });

  it("applies no discount without a voucher", () => {
    store.add("pen", 2, 10);

    expect(store.discount()).toBe(0);
    expect(store.total()).toBe(20);
  });

  it("applies a known voucher", () => {
    store.add("pen", 2, 10);

    store.applyVoucher("SAVE10");

    expect(store.discount()).toBe(2);
    expect(store.total()).toBe(18);
  });

  it("normalises the voucher code", () => {
    store.add("pen", 2, 10);

    store.applyVoucher("  save10  ");

    expect(store.state().voucher).toBe("SAVE10");
    expect(store.discount()).toBe(2);
  });

  it("ignores an unknown voucher", () => {
    store.add("pen", 2, 10);

    store.applyVoucher("NOPE");

    expect(store.discount()).toBe(0);
    expect(store.total()).toBe(20);
  });

  it("clears the voucher with a blank code", () => {
    store.add("pen", 2, 10);
    store.applyVoucher("SAVE10");

    store.applyVoucher("   ");

    expect(store.state().voucher).toBeNull();
    expect(store.discount()).toBe(0);
  });

  it("rounds the discount to two decimals", () => {
    store.add("pen", 1, 9.99);
    store.applyVoucher("SAVE10");

    expect(store.discount()).toBe(1);
    expect(store.total()).toBe(8.99);
  });

  it("recomputes the projections after every command", () => {
    store.add("pen", 1, 10);
    expect(store.total()).toBe(10);

    store.add("pad", 1, 10);
    expect(store.total()).toBe(20);

    store.applyVoucher("SAVE10");
    expect(store.total()).toBe(18);

    store.remove("pad");
    expect(store.total()).toBe(9);
  });

  it("clears everything", () => {
    store.add("pen", 2, 10);
    store.applyVoucher("SAVE10");

    store.clear();

    expect(store.isEmpty()).toBe(true);
    expect(store.state().voucher).toBeNull();
    expect(store.total()).toBe(0);
  });

  it("replaces the state object rather than mutating it", () => {
    store.add("pen", 1, 1);
    const before = store.state();

    store.add("pad", 1, 1);

    expect(store.state()).not.toBe(before);
    expect(before.lines).toHaveLength(1);
  });

  it("seals the write end of the public state", () => {
    expect(store.isSealed()).toBe(true);

    // Concretely: no component can reach in and set the basket behind the store's back.
    const asWritable = store.state as unknown as { set?: unknown; update?: unknown };
    expect(asWritable.set).toBeUndefined();
    expect(asWritable.update).toBeUndefined();
  });

  it("is shared, as a root-provided service", () => {
    store.add("pen", 1, 1);

    expect(TestBed.inject(BasketStore)).toBe(store);
    expect(TestBed.inject(BasketStore).itemCount()).toBe(1);
  });
});
