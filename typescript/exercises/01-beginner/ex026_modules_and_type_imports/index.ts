// Exercise 026 — modules: exports, type imports, and module state (beginner).
// Goal:   author a module that re-exports a sibling, carries a default, and
//         keeps state of its own.
// Drills: named and default exports, `import type`, module-level state.
// Passes: mmFrom converts through ./units, formatLength is reachable both
//         ways, and the id counter is shared by every importer.
//
// WHAT THIS ROW CANNOT GRADE, and why the stub already declares every export:
// a missing export is an error at the IMPORT site, so a test importing a name
// the stub does not have would put a type error in the test file instead of
// producing a failing fact. Every export below therefore exists already and
// merely throws. The choice between `export { x as y } from "./units"` and a
// wrapper function, or between `export default f` and a separate declaration,
// leaves no evidence a test can read — so what is graded is the behaviour.
// This row therefore has no type-level facts at all: every candidate turned
// out to be green on the untouched stub, because the stub has to declare
// every export for the tests to compile in the first place.
//
// Two things worth internalising anyway. `import type` and `export type` are
// erased entirely: they emit nothing, cannot be used as values, and exist to
// tell the bundler that no runtime dependency is implied. And a module-level
// binding is a SINGLETON — evaluated once, no matter how many modules import
// it. That is the closest thing here to a C# static, except the unit of
// sharing is the module, not the type, and there is no way to get a second
// copy.

import type { Unit } from "./units";

export type { Unit };

/** TODO: the length in millimetres. Delegate to ./units rather than redoing
 *  the arithmetic — the idiomatic form is a renamed re-export. */
export function mmFrom(_value: number, _unit: Unit): number {
  throw new Error("TODO: implement mmFrom");
}

/** TODO: `<value><unit>`, e.g. `2.5cm`. Trailing zeros are dropped: 3 is `3cm`. */
export function formatLength(_value: number, _unit: Unit): string {
  throw new Error("TODO: implement formatLength");
}

/** TODO: "id-1" on the first call, "id-2" on the next, and so on — from
 *  state held by this module, shared by every importer. */
export function nextId(): string {
  throw new Error("TODO: implement nextId");
}

/** TODO: send the counter back to the start, so the next id is "id-1". */
export function resetIds(): void {
  throw new Error("TODO: implement resetIds");
}

// Given: the module's default export is formatLength itself.
export default formatLength;
