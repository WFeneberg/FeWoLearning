import { expectTypeOf, test } from "vitest";
import { palette } from "@ex/02-intermediate/ex068_satisfies_operator/index";

// An annotation replaces the literal's type with Palette, and Palette's
// keys are `string`. `satisfies` checks and keeps what was written.
test("the keys stay literal", () => {
  expectTypeOf<keyof typeof palette>().toEqualTypeOf<"red" | "green">();
});

// Under the annotation this is the whole union and .toUpperCase() does
// not compile, though green is plainly a string.
test("each entry keeps its own value type", () => {
  expectTypeOf<(typeof palette)["green"]>().toEqualTypeOf<string>();
  expectTypeOf<(typeof palette)["red"]>().toEqualTypeOf<[number, number, number]>();
});
