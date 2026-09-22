import { expectTypeOf, test } from "vitest";
import type { Result } from "@ex/01-beginner/ex030_result_type/index";
import { err, ok, unwrapOr } from "@ex/01-beginner/ex030_result_type/index";

test("Result is the two-arm union", () => {
  expectTypeOf<Result<number, string>>().toEqualTypeOf<
    { ok: true; value: number } | { ok: false; error: string }
  >();
});

// Both unions are spelled out rather than written as Result<number, never>:
// while Result is still `unknown` on the stub, so is any instantiation of
// it, and the fact would compare unknown with unknown and pass before any
// work was done.
//
// Note that the unused arm does NOT disappear — measured. `Result<number,
// never>` is still a two-member union whose failure arm carries an
// uninhabited `error`, which is exactly what lets the result union cleanly
// with whatever the other branch of a call site produces.
test("ok builds a success and leaves the error type open", () => {
  expectTypeOf(ok(42)).toEqualTypeOf<
    { ok: true; value: number } | { ok: false; error: never }
  >();
});

test("err builds a failure and leaves the value type open", () => {
  expectTypeOf(err("nope")).toEqualTypeOf<
    { ok: true; value: never } | { ok: false; error: string }
  >();
});

test("narrowing on ok reaches the value", () => {
  const result = ok(42) as Result<number, string>;
  if (result.ok) {
    expectTypeOf(result.value).toEqualTypeOf<number>();
  } else {
    expectTypeOf(result.error).toEqualTypeOf<string>();
  }
});

test("unwrapOr collapses to the value type", () => {
  const result = ok(42) as Result<number, string>;
  expectTypeOf(unwrapOr(result, 0)).toEqualTypeOf<number>();
});
