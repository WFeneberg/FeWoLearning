import { expectTypeOf, test } from "vitest";
import type {
  MyExclude,
  MyExtract,
  MyNonNullable,
} from "@ex/02-intermediate/ex045_rebuild_exclude_extract/index";

// Only a DISTRIBUTIVE conditional can answer this: the union is taken
// apart, each member is asked separately, and the answers are unioned
// back. A non-distributive `[T] extends [U]` version tests the whole union
// at once and returns it unchanged.
test("MyExclude removes one member of a union", () => {
  expectTypeOf<MyExclude<"a" | "b" | "c", "b">>().toEqualTypeOf<"a" | "c">();
});

test("MyExclude removes several", () => {
  expectTypeOf<MyExclude<"a" | "b" | "c", "a" | "c">>().toEqualTypeOf<"b">();
});

test("MyExclude of everything is never", () => {
  expectTypeOf<MyExclude<"a", "a">>().toEqualTypeOf<never>();
});

test("MyExclude leaves a union alone when nothing matches", () => {
  expectTypeOf<MyExclude<"a" | "b", "z">>().toEqualTypeOf<"a" | "b">();
});

test("MyExtract keeps the members that match", () => {
  expectTypeOf<MyExtract<string | number | boolean, number | boolean>>().toEqualTypeOf<
    number | boolean
  >();
});

test("MyExtract of nothing is never", () => {
  expectTypeOf<MyExtract<"a" | "b", "z">>().toEqualTypeOf<never>();
});

test("MyNonNullable drops both null and undefined", () => {
  expectTypeOf<MyNonNullable<string | null | undefined>>().toEqualTypeOf<string>();
});

test("MyNonNullable leaves a clean type alone", () => {
  expectTypeOf<MyNonNullable<string | number>>().toEqualTypeOf<string | number>();
});
