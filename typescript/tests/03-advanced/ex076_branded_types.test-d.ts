import { expectTypeOf, test } from "vitest";
import type { OrderId, UserId } from "@ex/03-advanced/ex076_branded_types/index";

type Assignable<A, B> = A extends B ? true : false;

// One fact, both directions: a branded id is still a string, and a plain
// string is not an id. Split apart, the first half is green while the
// stub's types are `unknown` (register 1b).
test("the brand blocks only the direction that matters", () => {
  expectTypeOf<[Assignable<UserId, string>, Assignable<string, UserId>]>().toEqualTypeOf<
    [true, false]
  >();
});

// The reason the row exists: two structurally identical ids that will not
// be swapped.
test("two differently branded ids do not mix", () => {
  expectTypeOf<[Assignable<UserId, OrderId>, Assignable<OrderId, UserId>]>().toEqualTypeOf<
    [false, false]
  >();
});

// A `not.toEqualTypeOf<string>()` fact was written and dropped: `unknown`
// is not string either, so it was green on the untouched stub. The paired
// assignability facts above say the same thing and do move.
