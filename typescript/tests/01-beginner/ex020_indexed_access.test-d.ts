import { expectTypeOf, test } from "vitest";
import type {
  CustomerName,
  OrderLine,
  OrderValue,
} from "@ex/01-beginner/ex020_indexed_access/index";

// These grade the resulting type and cannot see how it was reached — see the
// note in the exercise header. Derive them anyway; that is the row.
test("CustomerName is the nested field's type", () => {
  expectTypeOf<CustomerName>().toEqualTypeOf<string>();
});

test("OrderLine is the element type of the lines array", () => {
  expectTypeOf<OrderLine>().toEqualTypeOf<{ sku: string; qty: number }>();
});

test("OrderValue is the union of every value type", () => {
  expectTypeOf<OrderValue>().toEqualTypeOf<
    string | { name: string; email: string } | { sku: string; qty: number }[]
  >();
});
