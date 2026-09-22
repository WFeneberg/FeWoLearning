import { expectTypeOf, test } from "vitest";
import type { Loose, Row, Same, Solid } from "@ex/02-intermediate/ex037_mapped_modifiers/index";

// The row's surprise, and the reason Partial and Readonly are one-liners:
// a mapped type whose source is exactly `keyof T` is homomorphic and copies
// the modifiers across without being asked.
test("a plain mapped type preserves readonly and optional", () => {
  expectTypeOf<Same<Row>>().toEqualTypeOf<{
    readonly id: string;
    label?: string;
    count: number;
  }>();
});

test("Solid strips both modifiers", () => {
  expectTypeOf<Solid<Row>>().toEqualTypeOf<{
    id: string;
    label: string;
    count: number;
  }>();
});

test("Loose adds both", () => {
  expectTypeOf<Loose<Row>>().toEqualTypeOf<{
    readonly id?: string;
    readonly label?: string;
    readonly count?: number;
  }>();
});
