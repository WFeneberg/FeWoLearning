import { expectTypeOf, test } from "vitest";
import type { Bag, BagKeys } from "@ex/02-intermediate/ex067_index_signatures/index";
import { readFrom } from "@ex/02-intermediate/ex067_index_signatures/index";

// Not just string: numeric keys coerce to strings at runtime, so a string
// index signature accepts bag[0] and keyof has to admit it.
test("keyof a string index signature is string | number", () => {
  expectTypeOf<BagKeys>().toEqualTypeOf<string | number>();
});

// No return annotation on readFrom, so this grades the body.
test("a read admits that the key might not be there", () => {
  expectTypeOf(readFrom({} as Bag, "x")).toEqualTypeOf<number | undefined>();
});

// A fact contrasting Bag's open keys with a literal Record's closed ones
// was written and dropped: it asked about given code only and was green
// on the untouched tree. ex043 grades that contrast, from a stub.
