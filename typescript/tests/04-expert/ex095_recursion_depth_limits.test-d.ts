import { expectTypeOf, test } from "vitest";
import type { NaiveLength, TailLength } from "@ex/04-expert/ex095_recursion_depth_limits/index";

test("both forms agree while the numbers are small", () => {
  expectTypeOf<NaiveLength<10>>().toEqualTypeOf<10>();
  expectTypeOf<TailLength<10>>().toEqualTypeOf<10>();
});

// Measured on TypeScript 7.0.2: the nested form works at 47 and fails
// at 48. The @ts-expect-error IS the assertion — it is unused, and
// therefore an error itself, while the stub's `unknown` compiles at any
// depth.
test("the nested form reaches 47 and no further", () => {
  expectTypeOf<NaiveLength<47>>().toEqualTypeOf<47>();
});

test("the nested form gives up at 48", () => {
  // @ts-expect-error — Type instantiation is excessively deep and possibly infinite
  type _TooDeep = NaiveLength<48>;
});

// Roughly twenty-one times further, for one structural change.
test("the accumulator form goes far past it", () => {
  expectTypeOf<TailLength<500>>().toEqualTypeOf<500>();
  expectTypeOf<TailLength<999>>().toEqualTypeOf<999>();
});

test("and has a ceiling of its own, at 1000", () => {
  // @ts-expect-error — Type instantiation is excessively deep and possibly infinite
  type _TooDeep = TailLength<1000>;
});
