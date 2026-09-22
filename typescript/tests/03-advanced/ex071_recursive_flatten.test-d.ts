import { expectTypeOf, test } from "vitest";
import type { Flatten, FlattenOnce } from "@ex/03-advanced/ex071_recursive_flatten/index";

test("Flatten reaches the innermost element type", () => {
  expectTypeOf<Flatten<number[][]>>().toEqualTypeOf<number>();
  expectTypeOf<Flatten<string[][][]>>().toEqualTypeOf<string>();
});

test("Flatten leaves a non-array alone", () => {
  expectTypeOf<Flatten<number>>().toEqualTypeOf<number>();
});

// A string is indexable but is not an array type, so it survives whole
// rather than dissolving into characters.
test("Flatten does not take a string apart", () => {
  expectTypeOf<Flatten<string>>().toEqualTypeOf<string>();
  expectTypeOf<Flatten<string[]>>().toEqualTypeOf<string>();
});

// The contrast that shows what the recursive call buys.
test("FlattenOnce stops after a single level", () => {
  expectTypeOf<FlattenOnce<number[][]>>().toEqualTypeOf<number[]>();
  expectTypeOf<FlattenOnce<number[]>>().toEqualTypeOf<number>();
});
