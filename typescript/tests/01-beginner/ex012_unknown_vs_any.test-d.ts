import { expectTypeOf, test } from "vitest";
import { parseJson } from "@ex/01-beginner/ex012_unknown_vs_any/index";

// The whole row in one fact. toEqualTypeOf distinguishes any from unknown, so
// `return JSON.parse(text)` — which infers any — fails here, and only an
// explicit widening to unknown passes.
test("parseJson reports unknown, not any", () => {
  expectTypeOf(parseJson("{}")).toEqualTypeOf<unknown>();
});
