import { expectTypeOf, test } from "vitest";
import format, { DEFAULTS, slugify, truncate } from "@ex/04-expert/ex098_dts_authoring/legacy";

// These grade the declaration file, which the compiler takes entirely
// on trust — it never opens legacy.js. That is the row.
test("slugify is declared as it behaves", () => {
  expectTypeOf(slugify).toEqualTypeOf<(text: string, separator?: string) => string>();
});

// The separator has a default in the implementation, so it is optional
// at the call site — and an optional parameter carries `| undefined`
// in Parameters (ex009).
test("the optional separator really is optional", () => {
  expectTypeOf<Parameters<typeof slugify>>().toEqualTypeOf<
    [text: string, separator?: string | undefined]
  >();
  expectTypeOf(slugify("a")).toEqualTypeOf<string>();
});

test("truncate is declared as it behaves", () => {
  expectTypeOf(truncate).toEqualTypeOf<(text: string, limit: number) => string>();
});

// Mutable, because the module exports a plain object. Declaring it
// readonly would be a promise legacy.js does not keep.
test("DEFAULTS is the object it is, and is not readonly", () => {
  expectTypeOf<typeof DEFAULTS>().toEqualTypeOf<{ separator: string; limit: number }>();
});

test("the default export is declared", () => {
  expectTypeOf(format).toEqualTypeOf<(text: string) => string>();
});
