import { expectTypeOf, test } from "vitest";
import type { Add, AtLeast, Subtract, Tuple } from "@ex/04-expert/ex093_tuple_arithmetic/index";

test("Tuple builds a tuple of the requested length", () => {
  expectTypeOf<Tuple<0>>().toEqualTypeOf<[]>();
  expectTypeOf<Tuple<3>["length"]>().toEqualTypeOf<3>();
  expectTypeOf<Tuple<2, string>>().toEqualTypeOf<[string, string]>();
});

test("Add concatenates, so the lengths add", () => {
  expectTypeOf<Add<2, 3>>().toEqualTypeOf<5>();
  expectTypeOf<Add<0, 0>>().toEqualTypeOf<0>();
  expectTypeOf<Add<7, 0>>().toEqualTypeOf<7>();
});

test("Subtract matches a prefix away", () => {
  expectTypeOf<Subtract<5, 2>>().toEqualTypeOf<3>();
  expectTypeOf<Subtract<4, 4>>().toEqualTypeOf<0>();
});

// The match simply fails when B is the larger, and never is the honest
// answer: the technique has no negatives in it.
test("Subtract below zero is never, not a negative", () => {
  expectTypeOf<Subtract<2, 5>>().toEqualTypeOf<never>();
});

test("AtLeast compares", () => {
  expectTypeOf<[AtLeast<5, 3>, AtLeast<3, 3>, AtLeast<2, 3>]>().toEqualTypeOf<
    [true, true, false]
  >();
});

test("the operations compose", () => {
  expectTypeOf<Subtract<Add<4, 5>, 3>>().toEqualTypeOf<6>();
});
