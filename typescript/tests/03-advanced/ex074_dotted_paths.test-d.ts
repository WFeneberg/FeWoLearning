import { expectTypeOf, test } from "vitest";
import type { Paths, Settings } from "@ex/03-advanced/ex074_dotted_paths/index";

test("a flat object contributes its keys", () => {
  expectTypeOf<Paths<{ a: number; b: string }>>().toEqualTypeOf<"a" | "b">();
});

// Both halves: "a" is a legal path in its own right as well as a prefix.
test("a branch appears as well as the paths beneath it", () => {
  expectTypeOf<Paths<{ a: { b: number } }>>().toEqualTypeOf<"a" | "a.b">();
});

test("the whole fixture, at three levels", () => {
  expectTypeOf<Paths<Settings>>().toEqualTypeOf<
    | "id"
    | "display"
    | "display.theme"
    | "display.layout"
    | "display.layout.columns"
    | "display.layout.dense"
    | "tags"
  >();
});

test("an array is a leaf", () => {
  expectTypeOf<Paths<{ tags: string[] }>>().toEqualTypeOf<"tags">();
});

test("a non-object has no paths", () => {
  expectTypeOf<Paths<number>>().toEqualTypeOf<never>();
});
