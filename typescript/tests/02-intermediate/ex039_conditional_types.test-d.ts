import { expectTypeOf, test } from "vitest";
import type { Classify, HasLength } from "@ex/02-intermediate/ex039_conditional_types/index";

// Arrays and functions are objects too, so an arm order that asks about
// `object` first collapses all three into "object" and fails here.
test("Classify puts arrays before objects", () => {
  expectTypeOf<Classify<string[]>>().toEqualTypeOf<"array">();
  expectTypeOf<Classify<readonly number[]>>().toEqualTypeOf<"array">();
});

test("Classify puts functions before objects", () => {
  expectTypeOf<Classify<() => void>>().toEqualTypeOf<"function">();
});

test("Classify recognises a plain object", () => {
  expectTypeOf<Classify<{ a: number }>>().toEqualTypeOf<"object">();
});

test("Classify falls through to primitive", () => {
  expectTypeOf<Classify<number>>().toEqualTypeOf<"primitive">();
  expectTypeOf<Classify<string>>().toEqualTypeOf<"primitive">();
});

// Assignability by shape, not by name: nothing declares a `length`
// interface anywhere.
test("HasLength answers by shape", () => {
  expectTypeOf<HasLength<string>>().toEqualTypeOf<true>();
  expectTypeOf<HasLength<unknown[]>>().toEqualTypeOf<true>();
  expectTypeOf<HasLength<{ length: number; other: boolean }>>().toEqualTypeOf<true>();
});

test("HasLength rejects what has none, and a length of the wrong type", () => {
  expectTypeOf<HasLength<number>>().toEqualTypeOf<false>();
  expectTypeOf<HasLength<{ length: string }>>().toEqualTypeOf<false>();
});
