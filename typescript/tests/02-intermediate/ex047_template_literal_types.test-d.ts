import { expectTypeOf, test } from "vitest";
import type { ClassName, EventName, Px } from "@ex/02-intermediate/ex047_template_literal_types/index";
import { className } from "@ex/02-intermediate/ex047_template_literal_types/index";

test("EventName prefixes a literal", () => {
  expectTypeOf<EventName<"click">>().toEqualTypeOf<"onclick">();
});

// Interpolating a union produces every combination, not a pairing.
test("EventName over a union produces one literal per member", () => {
  expectTypeOf<EventName<"click" | "focus">>().toEqualTypeOf<"onclick" | "onfocus">();
});

test("ClassName produces the cross product of both unions", () => {
  expectTypeOf<ClassName<"a" | "b", "x" | "y">>().toEqualTypeOf<
    "a-x" | "a-y" | "b-x" | "b-y"
  >();
});

// `${number}` is a pattern, so this is a set of strings rather than one.
//
// Accepted and rejected cases are asserted as ONE tuple deliberately. While
// Px is still `unknown` everything extends it, so an acceptance-only fact
// would be green on the untouched stub and would grade nothing.
test("Px is a pattern: digits then px, and nothing else", () => {
  expectTypeOf<
    [
      "4px" extends Px ? true : false,
      "0px" extends Px ? true : false,
      "4em" extends Px ? true : false,
      "4" extends Px ? true : false,
    ]
  >().toEqualTypeOf<[true, true, false, false]>();
});

test("className reports the exact literal it will return", () => {
  expectTypeOf(className("primary", "sm")).toEqualTypeOf<"primary-sm">();
});
