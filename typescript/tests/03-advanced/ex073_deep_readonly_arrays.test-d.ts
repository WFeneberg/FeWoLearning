import { expectTypeOf, test } from "vitest";
import type { DeepFreeze } from "@ex/03-advanced/ex073_deep_readonly_arrays/index";

// The row. ex065's explicit array branch turns this into
// `readonly (string | number)[]` and the arity is gone; a homomorphic
// mapped type keeps it.
test("a tuple stays a tuple of the same length", () => {
  expectTypeOf<DeepFreeze<[string, number]>>().toEqualTypeOf<readonly [string, number]>();
});

test("a nested tuple keeps its positions", () => {
  expectTypeOf<DeepFreeze<{ pair: [string, { n: number }] }>>().toEqualTypeOf<{
    readonly pair: readonly [string, { readonly n: number }];
  }>();
});

test("an array is still an array", () => {
  expectTypeOf<DeepFreeze<string[]>>().toEqualTypeOf<readonly string[]>();
});

test("an object is still an object, frozen all the way down", () => {
  expectTypeOf<DeepFreeze<{ a: { b: number } }>>().toEqualTypeOf<{
    readonly a: { readonly b: number };
  }>();
});

test("a function comes through untouched", () => {
  expectTypeOf<DeepFreeze<{ run: (n: number) => string }>>().toEqualTypeOf<{
    readonly run: (n: number) => string;
  }>();
});

test("a primitive is left alone", () => {
  expectTypeOf<DeepFreeze<number>>().toEqualTypeOf<number>();
});
