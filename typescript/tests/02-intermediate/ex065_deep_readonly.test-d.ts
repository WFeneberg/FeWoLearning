import { expectTypeOf, test } from "vitest";
import type { DeepReadonly } from "@ex/02-intermediate/ex065_deep_readonly/index";

test("DeepReadonly freezes every level", () => {
  expectTypeOf<DeepReadonly<{ a: { b: number } }>>().toEqualTypeOf<{
    readonly a: { readonly b: number };
  }>();
});

test("an array becomes a readonly array of readonly elements", () => {
  expectTypeOf<DeepReadonly<{ xs: { n: number }[] }>>().toEqualTypeOf<{
    readonly xs: readonly { readonly n: number }[];
  }>();
});

// The discriminating fact. A function is an object to `keyof`, so a
// recursive mapper that does not check for one FIRST maps it to {} and
// silently loses the signature.
test("a function comes through untouched", () => {
  expectTypeOf<DeepReadonly<{ run: (n: number) => string }>>().toEqualTypeOf<{
    readonly run: (n: number) => string;
  }>();
});

test("a primitive is left alone", () => {
  expectTypeOf<DeepReadonly<number>>().toEqualTypeOf<number>();
  expectTypeOf<DeepReadonly<string>>().toEqualTypeOf<string>();
});

test("recursion goes through arrays into objects and back", () => {
  expectTypeOf<DeepReadonly<{ rows: { cells: string[] }[] }>>().toEqualTypeOf<{
    readonly rows: readonly { readonly cells: readonly string[] }[];
  }>();
});
