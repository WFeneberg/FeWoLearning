import { expectTypeOf, test } from "vitest";
import { getIn, pluck, setIn } from "@ex/02-intermediate/ex049_constrained_key_generics/index";

const row = { id: "r-1", label: "first", count: 2 };

test("getIn reports the type of the key it was given", () => {
  expectTypeOf(getIn(row, "label")).toEqualTypeOf<string>();
  expectTypeOf(getIn(row, "count")).toEqualTypeOf<number>();
});

test("getIn rejects a key the object does not have", () => {
  // @ts-expect-error — "nope" is not a key of row
  getIn(row, "nope");
});

test("setIn gives back the object type it was handed", () => {
  expectTypeOf(setIn(row, "count", 9)).toEqualTypeOf<typeof row>();
});

// T[K] in a parameter position is what makes this an error. Typed against
// the union of all value types, a string would be perfectly acceptable
// here — so this fact is what separates the two designs.
test("setIn checks the value against that key, not against every key", () => {
  // @ts-expect-error — count is a number, however many string fields row has
  setIn(row, "count", "nope");
});

test("setIn accepts a value that does match", () => {
  expectTypeOf(setIn(row, "label", "second")).toEqualTypeOf<typeof row>();
});

test("pluck reports an array of that property's type", () => {
  const rows = [{ id: "a", count: 1 }];
  expectTypeOf(pluck(rows, "id")).toEqualTypeOf<string[]>();
  expectTypeOf(pluck(rows, "count")).toEqualTypeOf<number[]>();
});
