import { expectTypeOf, test } from "vitest";
import { chunk, first, last } from "@ex/01-beginner/ex017_generic_array_helpers/index";

// The undefined is not optional here: under noUncheckedIndexedAccess a read
// can miss, and a signature promising plain T would be lying.
test("first admits the undefined an empty list produces", () => {
  expectTypeOf(first(["a"])).toEqualTypeOf<string | undefined>();
});

test("last does the same, for the caller's element type", () => {
  expectTypeOf(last([1, 2])).toEqualTypeOf<number | undefined>();
});

test("chunk keeps the element type through both levels", () => {
  expectTypeOf(chunk([1, 2, 3], 2)).toEqualTypeOf<number[][]>();
});

test("chunk accepts a readonly array", () => {
  const frozen = ["a", "b"] as const;
  expectTypeOf(chunk(frozen, 1)).toEqualTypeOf<("a" | "b")[][]>();
});
