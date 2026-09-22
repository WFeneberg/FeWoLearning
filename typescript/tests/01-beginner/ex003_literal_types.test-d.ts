import { expectTypeOf, test } from "vitest";
import type { LogLevel } from "@ex/01-beginner/ex003_literal_types/index";
import { LEVELS } from "@ex/01-beginner/ex003_literal_types/index";

// This is the fact that grades `as const` itself: without it the declaration
// widens to string[], which is neither readonly nor a tuple of literals.
test("LEVELS keeps its exact literal tuple type", () => {
  expectTypeOf<typeof LEVELS>().toEqualTypeOf<
    readonly ["debug", "info", "warn", "error"]
  >();
});

test("LogLevel is the union of the four levels", () => {
  expectTypeOf<LogLevel>().toEqualTypeOf<"debug" | "info" | "warn" | "error">();
});
