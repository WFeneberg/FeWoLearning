import { describe, expect, it } from "vitest";
import { addTo, detach, preset } from "@ex/02-intermediate/ex063_bind_call_apply/index";
import type { Store } from "@ex/02-intermediate/ex063_bind_call_apply/index";

describe("ex063 addTo", () => {
  it("works through call", () => {
    const store: Store = { total: 10 };
    expect(addTo.call(store, 5)).toBe(15);
    expect(store.total).toBe(15);
  });

  it("works through apply", () => {
    const store: Store = { total: 0 };
    expect(addTo.apply(store, [7])).toBe(7);
  });
});

describe("ex063 detach", () => {
  it("keeps adding to the store it was bound to", () => {
    const store: Store = { total: 1 };
    const add = detach(store);
    expect(add(2)).toBe(3);
    expect(add(4)).toBe(7);
    expect(store.total).toBe(7);
  });

  it("needs no receiver, so it survives being passed on", () => {
    const store: Store = { total: 0 };
    const add = detach(store);
    const run = (fn: (n: number) => number): number => fn(5);
    expect(run(add)).toBe(5);
  });

  it("binds each store separately", () => {
    const a: Store = { total: 0 };
    const b: Store = { total: 100 };
    detach(a)(1);
    expect([a.total, b.total]).toEqual([1, 100]);
  });
});

describe("ex063 preset", () => {
  it("fixes the receiver and the amount", () => {
    const store: Store = { total: 0 };
    const addFive = preset(store, 5);
    expect([addFive(), addFive()]).toEqual([5, 10]);
  });

  // bind's partial application removes the bound argument from the
  // signature, so the result really takes none.
  it("takes no arguments", () => {
    const store: Store = { total: 0 };
    expect(preset(store, 5).length).toBe(0);
  });
});
