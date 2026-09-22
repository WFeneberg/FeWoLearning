import { expectTypeOf, test } from "vitest";
import type {
  Animal,
  Dog,
  HandlerAssignable,
  MethodAssignable,
  PropertyAssignable,
} from "@ex/02-intermediate/ex066_function_variance/index";

// Both directions in one fact, because `unknown` accepts everything and a
// single positive half would be green on the untouched stub (register 1b).
test("a bare handler is contravariant in its parameter", () => {
  expectTypeOf<
    [HandlerAssignable<Animal, Dog>, HandlerAssignable<Dog, Animal>]
  >().toEqualTypeOf<[true, false]>();
});

test("a handler in property position follows the same rule", () => {
  expectTypeOf<
    [PropertyAssignable<Animal, Dog>, PropertyAssignable<Dog, Animal>]
  >().toEqualTypeOf<[true, false]>();
});

// The hole. Written with method syntax the very same members are
// assignable BOTH ways — unsound, deliberate, and there so that
// Array<Dog> stays assignable to Array<Animal>.
test("a handler in method position is bivariant", () => {
  expectTypeOf<
    [MethodAssignable<Animal, Dog>, MethodAssignable<Dog, Animal>]
  >().toEqualTypeOf<[true, true]>();
});
