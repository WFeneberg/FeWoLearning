import { expectTypeOf, test } from "vitest";
import type { Formatter, Settings } from "@ex/01-beginner/ex019_typeof_operator/index";

// Widened, not literal: defaultSettings carries no `as const`, so 3 is number.
// That is what lets withOverrides accept any retry count.
test("Settings is the shape of defaultSettings", () => {
  expectTypeOf<Settings>().toEqualTypeOf<{
    retries: number;
    timeoutMs: number;
    verbose: boolean;
  }>();
});

test("Formatter is the signature of formatDuration", () => {
  expectTypeOf<Formatter>().toEqualTypeOf<(ms: number) => string>();
});
