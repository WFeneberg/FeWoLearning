import { test } from "vitest";
import { assertNever } from "@ex/01-beginner/ex013_never_exhaustiveness/index";
import type { ShapeV2 } from "@ex/01-beginner/ex013_never_exhaustiveness/index";

// The @ts-expect-error IS the assertion: it is itself an error when the call
// below compiles. While the parameter is still `unknown` the call is legal,
// so this fact is red — and it turns green only once the parameter is `never`,
// which is exactly what makes a forgotten case fail the build.
test("assertNever refuses a case that is still reachable", () => {
  const reachable = { kind: "triangle", base: 1, height: 2 } as ShapeV2;
  // @ts-expect-error — a reachable value must not be assignable to never
  assertNever(reachable);
});
