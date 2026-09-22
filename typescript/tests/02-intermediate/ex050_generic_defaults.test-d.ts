import { expectTypeOf, test } from "vitest";
import type { Outcome, Page } from "@ex/02-intermediate/ex050_generic_defaults/index";
import { makeList } from "@ex/02-intermediate/ex050_generic_defaults/index";

test("Outcome uses Error when the second argument is left out", () => {
  expectTypeOf<Outcome<number>>().toEqualTypeOf<
    { ok: true; value: number } | { ok: false; error: Error }
  >();
});

test("Outcome steps aside when an error type is given", () => {
  expectTypeOf<Outcome<number, string>>().toEqualTypeOf<
    { ok: true; value: number } | { ok: false; error: string }
  >();
});

test("Page defaults its item type", () => {
  expectTypeOf<Page>().toEqualTypeOf<{ items: unknown[]; total: number }>();
});

test("Page takes an item type when given one", () => {
  expectTypeOf<Page<string>>().toEqualTypeOf<{ items: string[]; total: number }>();
});

// For a function the default applies only where inference has nothing to
// work from — so both halves belong in one fact. Split apart, the
// inference half is green before the default is written (inference needs no
// default) and grades nothing.
test("the default applies only where inference has nothing to work from", () => {
  const fromNothing = makeList();
  const fromNumbers = makeList(1, 2);
  const fromBooleans = makeList(true);
  expectTypeOf<[typeof fromNothing, typeof fromNumbers, typeof fromBooleans]>().toEqualTypeOf<
    [string[], number[], boolean[]]
  >();
});
