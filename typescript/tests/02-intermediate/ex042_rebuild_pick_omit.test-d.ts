import { expectTypeOf, test } from "vitest";
import type { MyOmit, MyPick, Row } from "@ex/02-intermediate/ex042_rebuild_pick_omit/index";

test("MyPick keeps the named keys with their modifiers", () => {
  expectTypeOf<MyPick<Row, "id" | "count">>().toEqualTypeOf<{
    readonly id: string;
    count: number;
  }>();
});

test("MyPick of one key is a one-property type", () => {
  expectTypeOf<MyPick<Row, "label">>().toEqualTypeOf<{ label?: string }>();
});

test("MyOmit removes the named keys and keeps the rest intact", () => {
  expectTypeOf<MyOmit<Row, "label">>().toEqualTypeOf<{
    readonly id: string;
    count: number;
  }>();
});

test("MyOmit of nothing is the whole type", () => {
  expectTypeOf<MyOmit<Row, never>>().toEqualTypeOf<{
    readonly id: string;
    label?: string;
    count: number;
  }>();
});

// The row's real subject. The BUILT-IN Omit accepts this happily and
// removes nothing — `Omit<Row, "coutn">` is a silent no-op, which is a
// genuine bug factory. The stub's constraint is the loose one, so this
// fact is red until it is tightened to `keyof T`.
test("MyOmit rejects a key the source does not have", () => {
  // @ts-expect-error — "coutn" is a typo, not a key of Row
  type _Typo = MyOmit<Row, "coutn">;
});
