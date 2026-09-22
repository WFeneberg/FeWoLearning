import { expectTypeOf, test } from "vitest";
import { css } from "@ex/01-beginner/ex009_params_defaults_rest/index";

// A default makes the parameter optional in the signature TypeScript exposes,
// and a rest parameter shows up as a rest element in the tuple.
test("css exposes an optional unit and a rest of extras", () => {
  expectTypeOf<Parameters<typeof css>>().toEqualTypeOf<
    [value: number, unit?: string | undefined, ...extras: string[]]
  >();
});
