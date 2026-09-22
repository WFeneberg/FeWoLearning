import { beforeEach, describe, expect, it } from "vitest";
import { getProp, pick } from "@ex/01-beginner/ex018_keyof_basics/index";
import type { Product } from "@ex/01-beginner/ex018_keyof_basics/index";

let product: Product;

beforeEach(() => {
  product = { id: "p-1", name: "Widget", price: 9.5, inStock: true };
});

describe("ex018 getProp", () => {
  it("reads each property", () => {
    expect(getProp(product, "id")).toBe("p-1");
    expect(getProp(product, "price")).toBe(9.5);
    expect(getProp(product, "inStock")).toBe(true);
  });
});

describe("ex018 pick", () => {
  it("keeps only the listed keys", () => {
    expect(pick(product, ["id", "price"])).toEqual({ id: "p-1", price: 9.5 });
  });

  it("returns an empty object for an empty key list", () => {
    expect(pick(product, [])).toEqual({});
  });

  it("does not mutate the source", () => {
    pick(product, ["id"]);
    expect(product).toEqual({ id: "p-1", name: "Widget", price: 9.5, inStock: true });
  });
});
