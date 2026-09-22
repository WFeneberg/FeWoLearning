import { expectTypeOf, test } from "vitest";
import { isConfig } from "@ex/01-beginner/ex035_parsing_unknown_json/index";
import type { Config } from "@ex/01-beginner/ex035_parsing_unknown_json/index";

// A boolean-returning validator leaves the value `unknown` inside the
// branch, which is what makes the whole boundary pattern pointless.
test("isConfig narrows what it accepts", () => {
  const value: unknown = { name: "api", port: 1, tags: [] };
  if (isConfig(value)) {
    expectTypeOf(value).toEqualTypeOf<Config>();
  }
});
