import { expectTypeOf, test } from "vitest";
import { identity, longer } from "@ex/01-beginner/ex016_generics_and_constraints/index";

test("identity keeps the argument's type", () => {
  expectTypeOf(identity("text")).toEqualTypeOf<string>();
});

test("identity keeps an object type too", () => {
  expectTypeOf(identity({ a: 1 })).toEqualTypeOf<{ a: number }>();
});

test("longer keeps the shared argument type", () => {
  expectTypeOf(longer("ab", "c")).toEqualTypeOf<string>();
});

// The constraint is the row's subject, and only a rejection can show it: the
// stub's `unknown` parameters accept a number happily, which makes the
// expect-error unused and this fact red.
test("longer rejects arguments with no length", () => {
  // @ts-expect-error — a number has no length
  longer(1, 2);
});
