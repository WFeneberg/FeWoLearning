import { expectTypeOf, test } from "vitest";
import { isCircle } from "@ex/02-intermediate/ex053_in_and_instanceof_narrowing/index";
import type { Circle, Shape, Square } from "@ex/02-intermediate/ex053_in_and_instanceof_narrowing/index";

test("isCircle narrows to Circle in the true branch", () => {
  const shape = { radius: 1 } as Shape;
  if (isCircle(shape)) {
    expectTypeOf(shape).toEqualTypeOf<Circle>();
  }
});

// A predicate narrows the false branch too, by removing the matched
// member from the union — which for a two-member union leaves exactly one.
test("isCircle narrows to Square in the false branch", () => {
  const shape = { side: 1 } as Shape;
  if (!isCircle(shape)) {
    expectTypeOf(shape).toEqualTypeOf<Square>();
  }
});
