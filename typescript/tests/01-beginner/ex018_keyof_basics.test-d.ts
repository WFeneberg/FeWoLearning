import { expectTypeOf, test } from "vitest";
import { getProp } from "@ex/01-beginner/ex018_keyof_basics/index";
import type { Product, ProductKey } from "@ex/01-beginner/ex018_keyof_basics/index";

test("ProductKey is the union of Product's names", () => {
  expectTypeOf<ProductKey>().toEqualTypeOf<"id" | "name" | "price" | "inStock">();
});

// One call per property type: a signature returning Product[keyof Product]
// would hand back the whole union every time and fail both of these.
test("getProp reports the type of the key it was given", () => {
  const product = { id: "p-1", name: "Widget", price: 9.5, inStock: true } as Product;
  expectTypeOf(getProp(product, "price")).toEqualTypeOf<number>();
  expectTypeOf(getProp(product, "name")).toEqualTypeOf<string>();
  expectTypeOf(getProp(product, "inStock")).toEqualTypeOf<boolean>();
});

test("getProp rejects a key Product does not have", () => {
  const product = { id: "p-1", name: "Widget", price: 9.5, inStock: true } as Product;
  // @ts-expect-error — "colour" is not a key of Product
  getProp(product, "colour");
});
