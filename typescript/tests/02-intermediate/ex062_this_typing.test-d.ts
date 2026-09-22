import { expectTypeOf, test } from "vitest";
import { increment } from "@ex/02-intermediate/ex062_this_typing/index";
import type { Counter } from "@ex/02-intermediate/ex062_this_typing/index";

// Asserted as a pair. Split apart, the Parameters half is green on the
// untouched stub — the `this` parameter is erased from Parameters whether
// it is declared or not, so only the ThisParameterType half moves.
test("increment declares a receiver, and it is not one of the real parameters", () => {
  expectTypeOf<
    [ThisParameterType<typeof increment>, Parameters<typeof increment>]
  >().toEqualTypeOf<[Counter, [by: number]]>();
});

// strictBindCallApply checks the receiver. Without a `this` parameter
// this line compiles, which is what makes the fact red on the stub.
test("increment rejects a receiver that is not a Counter", () => {
  // @ts-expect-error — {} is not a Counter
  increment.call({}, 1);
});
