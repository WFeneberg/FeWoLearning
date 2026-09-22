import { expectTypeOf, test } from "vitest";
import { countTo, runningTotal } from "@ex/02-intermediate/ex059_generators/index";

// All three parameters asserted, because only the first is exercised by
// a for…of and the other two are the row.
test("countTo yields numbers, returns a number, receives nothing", () => {
  expectTypeOf(countTo(3)).toEqualTypeOf<Generator<number, number, void>>();
});

test("runningTotal yields numbers, never returns, receives numbers", () => {
  expectTypeOf(runningTotal(0)).toEqualTypeOf<Generator<number, void, number>>();
});

test("the value sent into next is checked against N", () => {
  const gen = runningTotal(0);
  gen.next();
  // @ts-expect-error — N is number, not string
  gen.next("five");
});
