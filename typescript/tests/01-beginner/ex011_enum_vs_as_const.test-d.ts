import { expectTypeOf, test } from "vitest";
import type { CompassPoint } from "@ex/01-beginner/ex011_enum_vs_as_const/index";
import { Compass } from "@ex/01-beginner/ex011_enum_vs_as_const/index";

// Without `as const` this object's values widen to string, which would make
// CompassPoint plain string and the whole idiom pointless.
test("Compass keeps its literal values and is readonly", () => {
  expectTypeOf<typeof Compass>().toEqualTypeOf<{
    readonly Up: "up";
    readonly Down: "down";
    readonly Left: "left";
    readonly Right: "right";
  }>();
});

test("CompassPoint is the union of the values", () => {
  expectTypeOf<CompassPoint>().toEqualTypeOf<"up" | "down" | "left" | "right">();
});
