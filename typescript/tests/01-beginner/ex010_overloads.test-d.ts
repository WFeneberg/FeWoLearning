import { expectTypeOf, test } from "vitest";
import { parse } from "@ex/01-beginner/ex010_overloads/index";

// Without the overloads each call reports string[] | number[] — the union the
// implementation signature declares. With them, each call reports one arm.
test("a string argument yields string[]", () => {
  expectTypeOf(parse("a,b")).toEqualTypeOf<string[]>();
});

test("a number argument yields number[]", () => {
  expectTypeOf(parse(407)).toEqualTypeOf<number[]>();
});

// The implementation signature is not part of the contract: once the
// overloads exist, a union argument matches neither of them. @ts-expect-error
// is itself the assertion here — it is an error when the call compiles, which
// is precisely the untouched stub's behaviour.
test("the implementation signature is not callable", () => {
  const input = 1 as string | number;
  // @ts-expect-error — no overload accepts string | number
  parse(input);
});
