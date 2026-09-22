import { expectTypeOf, test } from "vitest";
import type { IsNamed } from "@ex/01-beginner/ex001_structural_typing/index";

test("an exactly matching shape is Named", () => {
  expectTypeOf<IsNamed<{ name: string }>>().toEqualTypeOf<true>();
});

test("extra properties do not disqualify a type", () => {
  expectTypeOf<IsNamed<{ name: string; age: number }>>().toEqualTypeOf<true>();
});

test("a separately declared class instance type is Named by shape", () => {
  class Robot {
    constructor(public readonly name: string) {}
  }
  expectTypeOf<IsNamed<Robot>>().toEqualTypeOf<true>();
});

test("the wrong property type is not Named", () => {
  expectTypeOf<IsNamed<{ name: number }>>().toEqualTypeOf<false>();
});

test("a missing property is not Named", () => {
  expectTypeOf<IsNamed<{ age: number }>>().toEqualTypeOf<false>();
});
