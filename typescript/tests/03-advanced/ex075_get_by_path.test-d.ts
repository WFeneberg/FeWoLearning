import { expectTypeOf, test } from "vitest";
import { getByPath } from "@ex/03-advanced/ex075_get_by_path/index";
import type { Order, ValueAt } from "@ex/03-advanced/ex075_get_by_path/index";

test("ValueAt resolves a top-level key", () => {
  expectTypeOf<ValueAt<Order, "id">>().toEqualTypeOf<string>();
  expectTypeOf<ValueAt<Order, "total">>().toEqualTypeOf<number>();
});

// `infer` on a template literal is greedy from the LEFT, so the walk is
// one segment per step rather than one from the end.
test("ValueAt walks a nested path", () => {
  expectTypeOf<ValueAt<Order, "customer.name">>().toEqualTypeOf<string>();
  expectTypeOf<ValueAt<Order, "customer.address.zip">>().toEqualTypeOf<number>();
});

test("ValueAt lands on a branch as a whole", () => {
  expectTypeOf<ValueAt<Order, "customer.address">>().toEqualTypeOf<{
    city: string;
    zip: number;
  }>();
});

test("ValueAt gives never for a path that does not exist", () => {
  expectTypeOf<ValueAt<Order, "nope">>().toEqualTypeOf<never>();
  expectTypeOf<ValueAt<Order, "customer.nope">>().toEqualTypeOf<never>();
});

test("getByPath reports the type the path lands on", () => {
  const source = {} as Order;
  expectTypeOf(getByPath(source, "customer.address.zip")).toEqualTypeOf<number>();
  expectTypeOf(getByPath(source, "id")).toEqualTypeOf<string>();
});

// The payoff: a typo is a compile error at the call site.
test("getByPath rejects a path that does not exist", () => {
  const source = {} as Order;
  // @ts-expect-error — "customer.nmae" is not a path into Order
  getByPath(source, "customer.nmae");
});
