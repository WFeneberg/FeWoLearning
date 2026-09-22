// Exercise 068 — `satisfies`: check it, but do not widen it (intermediate).
// Goal:   validate a literal against a type without losing what the
//         literal actually was.
// Drills: `satisfies` versus a type annotation, and what each does to
//         `keyof typeof`.
// Passes: the palette's keys stay literal and each entry keeps its own
//         value type.
//
// An annotation does two jobs at once, and you usually only want one of
// them. `const palette: Palette = { … }` CHECKS the literal and then
// REPLACES its type with Palette — so `keyof typeof palette` becomes
// `string`, every entry becomes the whole union of allowed value types,
// and `palette.green.toUpperCase()` stops compiling even though green is
// plainly a string.
//
// `const palette = { … } satisfies Palette` does the checking and keeps
// the inferred type. Same errors for a bad entry, none of the loss.
//
// The stub below is the annotated form — which compiles, which is the
// point: nothing is wrong with it except what it throws away. Switching
// it to `satisfies` is the work.
//
// Worth knowing against `as const`, which also preserves: `as const` does
// no checking at all, and makes everything readonly. `satisfies` checks
// and changes nothing. The two compose — `{ … } as const satisfies T` —
// when you want both.

export type Palette = Record<string, string | readonly [number, number, number]>;

/** TODO: keep the checking, lose the widening. */
export const palette: Palette = {
  red: [255, 0, 0],
  green: "#00ff00",
};

/** TODO: render one entry as CSS — a string entry as-is, a triple as
 *  `rgb(r, g, b)`. */
export function toCss(_name: keyof typeof palette): string {
  throw new Error("TODO: implement toCss");
}
