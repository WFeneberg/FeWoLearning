import { expectTypeOf, test } from "vitest";
import { take } from "@ex/01-beginner/ex034_iterables/index";

test("take keeps the element type it was given", () => {
  expectTypeOf(take(["a", "b"], 1)).toEqualTypeOf<string[]>();
});

test("take reads the element type out of any iterable, not just arrays", () => {
  expectTypeOf(take(new Set([1, 2]), 1)).toEqualTypeOf<number[]>();
});
