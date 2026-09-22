import { test } from "vitest";
import { Temperature } from "@ex/01-beginner/ex024_accessors_and_static/index";

// The one type-level fact this row can honestly carry. Two others were tried
// and dropped because the stub already satisfies them: `fahrenheit` is
// declared as a getter with no setter, so it is readonly before any work is
// done, and `celsius` is already annotated `number`.
test("the constant cannot be reassigned", () => {
  // @ts-expect-error — ABSOLUTE_ZERO_C is readonly
  Temperature.ABSOLUTE_ZERO_C = 0;
});
