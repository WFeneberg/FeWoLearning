import { expectTypeOf, test } from "vitest";
import { addTo, detach } from "@ex/02-intermediate/ex063_bind_call_apply/index";
import type { Store } from "@ex/02-intermediate/ex063_bind_call_apply/index";

// Paired with detach's result for the same reason as ex062: without a
// `this` parameter, OmitThisParameter<typeof addTo> is ALREADY
// `(amount: number) => number`, so that half alone is green on the
// untouched stub. Only the receiver half moves.
test("addTo declares a receiver, and detach hands back the version without it", () => {
  const store: Store = { total: 0 };
  expectTypeOf<
    [ThisParameterType<typeof addTo>, ReturnType<typeof detach>]
  >().toEqualTypeOf<[Store, (amount: number) => number]>();
  void store;
});

// strictBindCallApply. Without a `this` parameter on addTo this compiles,
// which is what starts the fact red.
test("call checks the receiver", () => {
  // @ts-expect-error — {} is not a Store
  addTo.call({}, 1);
});
