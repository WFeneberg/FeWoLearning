import { expectTypeOf, test } from "vitest";
import { at } from "@ex/01-beginner/ex007_safe_indexing/index";

// at() carries no return annotation, so this grades what the body actually
// produces. `return items[index]!` infers string here and fails — which is
// the point of the row.
test("at reports the undefined it can return", () => {
  expectTypeOf(at(["a"], 0)).toEqualTypeOf<string | undefined>();
});

test("at keeps the element type it was given", () => {
  expectTypeOf(at([1, 2], 0)).toEqualTypeOf<number | undefined>();
});
