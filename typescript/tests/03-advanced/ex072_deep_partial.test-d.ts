import { expectTypeOf, test } from "vitest";
import type { DeepPartial } from "@ex/03-advanced/ex072_deep_partial/index";

test("nesting becomes optional at every level", () => {
  expectTypeOf<DeepPartial<{ a: { b: { c: number } } }>>().toEqualTypeOf<{
    a?: { b?: { c?: number } };
  }>();
});

// The canonical casualty. Recursing into a Date produces an object of
// optional methods, which accepts {} and is not a Date.
test("a Date is a leaf", () => {
  expectTypeOf<DeepPartial<{ at: Date }>>().toEqualTypeOf<{ at?: Date }>();
});

test("a function is a leaf", () => {
  expectTypeOf<DeepPartial<{ run: (v: string) => string }>>().toEqualTypeOf<{
    run?: (v: string) => string;
  }>();
});

// The property becomes optional; the elements do not, or the patch would
// describe a list with holes in it.
test("an array of primitives keeps solid elements", () => {
  expectTypeOf<DeepPartial<{ tags: string[] }>>().toEqualTypeOf<{ tags?: string[] }>();
});

test("an array of objects recurses into the objects", () => {
  expectTypeOf<DeepPartial<{ rows: { a: number }[] }>>().toEqualTypeOf<{
    rows?: { a?: number }[];
  }>();
});

test("a primitive is left alone", () => {
  expectTypeOf<DeepPartial<number>>().toEqualTypeOf<number>();
});
