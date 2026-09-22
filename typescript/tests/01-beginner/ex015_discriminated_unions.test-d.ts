import { expectTypeOf, test } from "vitest";
import type { AppEvent } from "@ex/01-beginner/ex015_discriminated_unions/index";

test("AppEvent is the three-member tagged union", () => {
  expectTypeOf<AppEvent>().toEqualTypeOf<
    | { type: "click"; x: number; y: number }
    | { type: "key"; key: string }
    | { type: "scroll"; delta: number }
  >();
});

// Extract only reaches one arm if the tag is a literal type. Declared as
// `type: string`, the union is undiscriminated and this resolves to never.
test("the tag discriminates, so one member can be selected by it", () => {
  expectTypeOf<Extract<AppEvent, { type: "key" }>>().toEqualTypeOf<{
    type: "key";
    key: string;
  }>();
});
