import { expectTypeOf, test } from "vitest";
import {
  assertDefined,
  assertNonEmpty,
  assertString,
} from "@ex/02-intermediate/ex052_assertion_functions/index";

// Narrowing AFTER the call, with no branch anywhere. A `void` return
// leaves the value exactly as it was, which is what makes these red.
test("assertString narrows the caller's value", () => {
  const value: unknown = "x";
  assertString(value);
  expectTypeOf(value).toEqualTypeOf<string>();
});

// The value comes from a CALL, not from a literal. Measured: with
// `const value: string | null | undefined = "x"`, control-flow analysis
// narrows it to string at the initializer and this fact is green on the
// untouched stub, grading nothing. A function's declared return type gives
// the checker nothing to narrow from.
function load(): string | null | undefined {
  return "x";
}

test("assertDefined removes null and undefined", () => {
  const value = load();
  assertDefined(value);
  expectTypeOf(value).toEqualTypeOf<string>();
});

// Narrowed to a non-empty tuple, which is what removes the undefined from
// items[0] under noUncheckedIndexedAccess (ex006, ex007).
test("assertNonEmpty makes the first element definitely present", () => {
  const items: readonly number[] = [1, 2];
  assertNonEmpty(items);
  expectTypeOf(items[0]).toEqualTypeOf<number>();
});
