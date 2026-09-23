import { expectTypeOf, test } from "vitest";
import type { Join, Split } from "@ex/04-expert/ex091_split_string_type/index";
import { split } from "@ex/04-expert/ex091_split_string_type/index";

test("Split takes a string apart on its delimiter", () => {
  expectTypeOf<Split<"a,b,c", ",">>().toEqualTypeOf<["a", "b", "c"]>();
});

// Spread, not nested: a recursive call placed inside the tuple rather
// than spread into it builds a tree and fails here.
test("Split stays flat however many pieces there are", () => {
  expectTypeOf<Split<"a,b,c,d,e", ",">>().toEqualTypeOf<["a", "b", "c", "d", "e"]>();
});

test("Split keeps the whole string when nothing matches", () => {
  expectTypeOf<Split<"abc", ",">>().toEqualTypeOf<["abc"]>();
});

// The base case is [S], not []. Both of these follow from it.
test("Split keeps the empty pieces", () => {
  expectTypeOf<Split<"", ",">>().toEqualTypeOf<[""]>();
  expectTypeOf<Split<"a,,b", ",">>().toEqualTypeOf<["a", "", "b"]>();
  expectTypeOf<Split<"a,", ",">>().toEqualTypeOf<["a", ""]>();
});

test("a multi-character delimiter needs no extra care", () => {
  expectTypeOf<Split<"a::b", "::">>().toEqualTypeOf<["a", "b"]>();
});

test("Join puts them back", () => {
  expectTypeOf<Join<["a", "b", "c"], ",">>().toEqualTypeOf<"a,b,c">();
  expectTypeOf<Join<["only"], ",">>().toEqualTypeOf<"only">();
  expectTypeOf<Join<[], ",">>().toEqualTypeOf<"">();
});

test("Join undoes Split", () => {
  expectTypeOf<Join<Split<"a,b,c", ",">, ",">>().toEqualTypeOf<"a,b,c">();
});

test("split reports the tuple its arguments imply", () => {
  expectTypeOf(split("a,b", ",")).toEqualTypeOf<["a", "b"]>();
});
