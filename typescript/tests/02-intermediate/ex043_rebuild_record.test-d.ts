import { expectTypeOf, test } from "vitest";
import type { MyRecord } from "@ex/02-intermediate/ex043_rebuild_record/index";

test("a literal key union produces exactly those properties", () => {
  expectTypeOf<MyRecord<"a" | "b", number>>().toEqualTypeOf<{ a: number; b: number }>();
});

test("a single literal key produces one property", () => {
  expectTypeOf<MyRecord<"only", boolean>>().toEqualTypeOf<{ only: boolean }>();
});

// The same syntax, a very different contract: widen the key type and the
// mapped type becomes an index signature, where every read succeeds.
test("a wide key type produces an index signature instead", () => {
  expectTypeOf<MyRecord<string, number>>().toEqualTypeOf<{ [x: string]: number }>();
});

test("a numeric key type produces a numeric index signature", () => {
  expectTypeOf<MyRecord<number, string>>().toEqualTypeOf<{ [x: number]: string }>();
});

// The stub leaves K unconstrained, so this is red until PropertyKey is in
// place. A boolean cannot be a property key.
test("MyRecord rejects a key type that cannot be a key", () => {
  // @ts-expect-error — boolean is not a PropertyKey
  type _Bad = MyRecord<boolean, number>;
});
