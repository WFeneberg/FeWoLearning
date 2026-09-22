import { expectTypeOf, test } from "vitest";
import { reasonsOf } from "@ex/02-intermediate/ex057_aggregate_error/index";

// `AggregateError.errors` is typed any[] in the standard library, so an
// implementation that maps over it without narrowing infers any[] here
// and fails.
test("reasonsOf produces strings, not anys", () => {
  expectTypeOf(reasonsOf(new AggregateError([], "x"))).toEqualTypeOf<string[]>();
});
