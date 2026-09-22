// Exercise 069 — `const` type parameters (intermediate).
// Goal:   let a function keep its caller's literals without the caller
//         writing `as const`.
// Drills: `<const T>`, what it does to an array argument and to a nested
//         object literal.
// Passes: route remembers the exact segments it was handed, and
//         defineConfig remembers a nested list.
//
// Inference widens by default: `route(["users", "id"])` infers `string[]`,
// and the literals are gone before the function body starts. The old fix
// was to make every caller write `route(["users", "id"] as const)`, which
// works and which library authors cannot enforce.
//
// `<const T extends …>` moves that decision to the DECLARATION: the
// argument is inferred as if the caller had written `as const`, so the
// result is `readonly ["users", "id"]`. It reaches nested literals too,
// which is what makes it useful for configuration objects rather than
// just for tuples.
//
// Two things it does not do. It does not make the VALUE readonly at
// runtime — nothing is frozen, and `as const` did not do that either. And
// it has no effect on an argument that is already a variable of a widened
// type: `const parts = ["a", "b"]; route(parts)` still sees `string[]`,
// because the widening happened at the variable, before the call.
//
// The stubs have plain type parameters, so adding `const` is the work.

/** TODO: keep the caller's exact segments, then join them with "/". */
export function route<T extends readonly string[]>(_parts: T): T {
  throw new Error("TODO: implement route");
}

/** TODO: give back the configuration unchanged, keeping every literal. */
export function defineConfig<T>(_config: T): T {
  throw new Error("TODO: implement defineConfig");
}

/** The path for a set of segments. Given; it needs no work. */
export function pathOf(parts: readonly string[]): string {
  return `/${parts.join("/")}`;
}
