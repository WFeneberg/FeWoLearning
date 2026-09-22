import { expectTypeOf, test } from "vitest";
import { partition } from "@ex/02-intermediate/ex056_promise_combinators/index";

// No return annotation on partition, so this grades the shape the body
// actually builds.
test("partition reports both halves with their own element types", () => {
  expectTypeOf(partition([Promise.resolve(1)])).toEqualTypeOf<
    Promise<{ values: number[]; errors: string[] }>
  >();
});
