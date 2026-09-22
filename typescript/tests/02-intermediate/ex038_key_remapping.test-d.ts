import { expectTypeOf, test } from "vitest";
import type { Api, DataOf, MethodsOf, Prefixed } from "@ex/02-intermediate/ex038_key_remapping/index";

test("MethodsOf keeps only the callable members", () => {
  expectTypeOf<MethodsOf<Api>>().toEqualTypeOf<{
    getUser(): string;
    getPost(): string;
  }>();
});

// The complementary filter, so that a MethodsOf which simply copied
// everything could not satisfy both facts.
test("DataOf keeps only the rest", () => {
  expectTypeOf<DataOf<Api>>().toEqualTypeOf<{ id: number; label: string }>();
});

test("Prefixed renames every key and leaves the values alone", () => {
  expectTypeOf<Prefixed<{ id: number; label: string }>>().toEqualTypeOf<{
    raw_id: number;
    raw_label: string;
  }>();
});

test("a type with no methods maps to an empty MethodsOf", () => {
  expectTypeOf<MethodsOf<{ a: number }>>().toEqualTypeOf<{}>();
});
