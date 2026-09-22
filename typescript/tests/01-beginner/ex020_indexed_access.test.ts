import { beforeEach, describe, expect, it } from "vitest";
import { lineAt, totalQty } from "@ex/01-beginner/ex020_indexed_access/index";
import type { Order } from "@ex/01-beginner/ex020_indexed_access/index";

let order: Order;

beforeEach(() => {
  order = {
    id: "o-1",
    customer: { name: "Ada", email: "ada@example.com" },
    lines: [
      { sku: "a", qty: 2 },
      { sku: "b", qty: 5 },
    ],
  };
});

describe("ex020 totalQty", () => {
  it("sums every line", () => {
    expect(totalQty(order)).toBe(7);
  });

  it("is 0 for an order with no lines", () => {
    order.lines = [];
    expect(totalQty(order)).toBe(0);
  });
});

describe("ex020 lineAt", () => {
  it("reads a line in range", () => {
    expect(lineAt(order, 1)).toEqual({ sku: "b", qty: 5 });
  });

  it("returns undefined past the end", () => {
    expect(lineAt(order, 9)).toBeUndefined();
  });
});
