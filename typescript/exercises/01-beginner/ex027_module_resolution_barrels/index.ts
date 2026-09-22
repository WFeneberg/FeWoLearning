// Exercise 027 — barrels, re-export collisions, and import cycles
// (beginner).
// Goal:   publish one entry point over several modules, and survive a cycle.
// Drills: `export *`, an explicit renamed re-export, deferring a read across
//         a circular import.
// Passes: `describe` resolves to the shapes one, `describeColour` to the
//         colours one, `isWarm` comes through, and the catalogue/pricing
//         pair loads at all.
//
// A barrel is a module that exists only to re-export others, so consumers
// write one import instead of five. Two things go wrong with them, and this
// row is both.
//
// The first is a NAME COLLISION. ./shapes and ./colours both export
// `describe`, so the obvious second `export * from "./colours"` does not
// work — try it and see. Measured: TS2308, "Module ./shapes has already
// exported a member named 'describe'". Two `export *` lines cannot both
// contribute the same name.
//
// Worth knowing what that error is protecting you from, also measured: the
// bundler running these tests resolves the clash at runtime by keeping the
// first one and saying nothing at all. The behaviour looks fine, the
// ambiguity is real, and TS2308 is the only thing between you and a barrel
// whose meaning depends on the order of two lines.
//
// TODO: keep `export *` for ./shapes, and re-export ./colours EXPLICITLY,
// renaming the member that clashes to `describeColour` and letting `isWarm`
// and the `Colour` type through under their own names.
//
// The second is in ./pricing: see the TODO there. Nothing in this file needs
// changing for it.

export * from "./shapes";
