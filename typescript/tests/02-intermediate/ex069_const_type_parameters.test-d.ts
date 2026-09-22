import { expectTypeOf, test } from "vitest";
import { defineConfig, route } from "@ex/02-intermediate/ex069_const_type_parameters/index";

// No `as const` at any call site below. Without the `const` modifier on
// the type parameter these all widen to string[] and string.
test("route keeps the caller's exact segments", () => {
  expectTypeOf(route(["users", "id"])).toEqualTypeOf<readonly ["users", "id"]>();
});

test("route keeps a single segment", () => {
  expectTypeOf(route(["health"])).toEqualTypeOf<readonly ["health"]>();
});

test("defineConfig keeps nested literals, not just top-level ones", () => {
  expectTypeOf(defineConfig({ name: "api", tags: ["x"] })).toEqualTypeOf<{
    readonly name: "api";
    readonly tags: readonly ["x"];
  }>();
});

// A fact for the limit in the header — a widened variable stays widened,
// because the widening happened before the call — was written and
// dropped: it holds with or without `const` on the parameter, so it was
// green on the untouched tree.
