import { expectTypeOf, test } from "vitest";
import type { Repeat, Reverse, SumOf } from "@ex/04-expert/ex094_tail_recursive_types/index";

test("Reverse turns a tuple around", () => {
  expectTypeOf<Reverse<[1, 2, 3]>>().toEqualTypeOf<[3, 2, 1]>();
  expectTypeOf<Reverse<["a"]>>().toEqualTypeOf<["a"]>();
  expectTypeOf<Reverse<[]>>().toEqualTypeOf<[]>();
});

test("Reverse keeps each position's own type", () => {
  expectTypeOf<Reverse<[string, number, boolean]>>().toEqualTypeOf<
    [boolean, number, string]
  >();
});

test("Repeat concatenates a string N times", () => {
  expectTypeOf<Repeat<"ab", 3>>().toEqualTypeOf<"ababab">();
  expectTypeOf<Repeat<"x", 1>>().toEqualTypeOf<"x">();
  expectTypeOf<Repeat<"x", 0>>().toEqualTypeOf<"">();
});

test("SumOf adds a tuple of numbers", () => {
  expectTypeOf<SumOf<[1, 2, 3]>>().toEqualTypeOf<6>();
  expectTypeOf<SumOf<[]>>().toEqualTypeOf<0>();
  expectTypeOf<SumOf<[5]>>().toEqualTypeOf<5>();
});

// The reason for the accumulator, at a depth a nested version does not
// reach. ex095 has the measured boundary.
//
// Asserted as "still a literal" rather than by length: a string literal
// type has no literal `length`, only `number`, so measuring it that way
// proves nothing. If the recursion had given up, this would be `string`.
type IsLiteral<S extends string> = string extends S ? false : true;

test("the accumulator form survives a depth the nested one does not", () => {
  expectTypeOf<IsLiteral<Repeat<"x", 120>>>().toEqualTypeOf<true>();
});
