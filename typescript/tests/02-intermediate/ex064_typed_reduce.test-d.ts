import { expectTypeOf, test } from "vitest";
import { fold, groupBy } from "@ex/02-intermediate/ex064_typed_reduce/index";

// A comes from the SEED. The callback is contextually typed from it and
// from the element type, so neither parameter is annotated below.
test("fold reports the seed's type", () => {
  expectTypeOf(fold([1, 2, 3], 0, (acc, n) => acc + n)).toEqualTypeOf<number>();
});

test("fold can fold into an unrelated type", () => {
  expectTypeOf(fold([1, 2], "", (acc, n) => acc + String(n))).toEqualTypeOf<string>();
});

// The seed carries an annotation here for the reason the header gives:
// a bare [] would infer never[] and the callback's first line would be
// the error, with no mention of the seed anywhere.
test("an annotated seed decides the element type of the result", () => {
  expectTypeOf(fold([1, 2], [] as number[], (acc, n) => [...acc, n])).toEqualTypeOf<number[]>();
});

test("groupBy is keyed by whatever the selector returns", () => {
  expectTypeOf(groupBy(["a", "bb"], (w) => w.length)).toEqualTypeOf<Record<number, string[]>>();
});

test("groupBy keeps the element type in the groups", () => {
  expectTypeOf(groupBy([{ id: 1 }], (item) => (item.id > 0 ? "pos" : "neg"))).toEqualTypeOf<
    Record<"pos" | "neg", { id: number }[]>
  >();
});
