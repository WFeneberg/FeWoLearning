import { expectTypeOf, test } from "vitest";
import type { Box, Boxed, Stringify, User } from "@ex/02-intermediate/ex036_mapped_types/index";

test("Stringify keeps the keys and replaces every value", () => {
  expectTypeOf<Stringify<User>>().toEqualTypeOf<{
    id: string;
    age: string;
    active: string;
  }>();
});

// T[K] rather than one shared type: each box carries that key's own value
// type, which a `{ [K in keyof T]: Box<unknown> }` would flatten away.
test("Boxed wraps each value in a box of its own type", () => {
  expectTypeOf<Boxed<User>>().toEqualTypeOf<{
    id: Box<string>;
    age: Box<number>;
    active: Box<boolean>;
  }>();
});

test("a mapped type works over any object type, not just User", () => {
  expectTypeOf<Stringify<{ a: number }>>().toEqualTypeOf<{ a: string }>();
});
