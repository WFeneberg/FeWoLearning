import { expectTypeOf, test } from "vitest";
import type { Json } from "@ex/04-expert/ex099_json_serializable/index";

test("plain values come through unchanged", () => {
  expectTypeOf<Json<{ a: number; b: string; c: boolean; d: null }>>().toEqualTypeOf<{
    a: number;
    b: string;
    c: boolean;
    d: null;
  }>();
});

// The key disappears, not just its value — which is why the mapped
// type needs an `as` clause (ex038).
test("a function-valued key is dropped entirely", () => {
  expectTypeOf<Json<{ a: number; run: () => void }>>().toEqualTypeOf<{ a: number }>();
});

test("an undefined-valued key is dropped entirely", () => {
  expectTypeOf<Json<{ a: number; b: undefined }>>().toEqualTypeOf<{ a: number }>();
});

// Matched before the object arm, since a Date is an object too.
test("a Date becomes a string", () => {
  expectTypeOf<Json<{ at: Date }>>().toEqualTypeOf<{ at: string }>();
});

test("a Map or a Set becomes an empty object", () => {
  expectTypeOf<Json<{ m: Map<string, number> }>>().toEqualTypeOf<{
    m: Record<string, never>;
  }>();
});

test("recursion reaches into arrays and nested objects", () => {
  expectTypeOf<Json<{ rows: { a: number; run: () => void }[] }>>().toEqualTypeOf<{
    rows: { a: number }[];
  }>();
});

test("a primitive is itself", () => {
  expectTypeOf<Json<string>>().toEqualTypeOf<string>();
  expectTypeOf<Json<number>>().toEqualTypeOf<number>();
});
