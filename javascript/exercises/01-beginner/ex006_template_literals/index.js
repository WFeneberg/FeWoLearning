// Exercise 006 — template literals (beginner).
// Goal:   build strings with `${}` instead of concatenation.
// Drills: interpolation, nested expressions, multiline literals, the
//         String() conversion every `${}` performs.
// Passes: label() pluralizes inside the literal, card() is one multiline
//         template, and interpolate() shows what `${}` does to a value.

/**
 * `label("apple", 1)` -> "apple: 1 item"
 * `label("apple", 3)` -> "apple: 3 items"
 *
 * TODO: implement as ONE template literal — the plural `s` comes from a
 * ternary inside the `${}`.
 */
export function label(_name, _count) {
  throw new Error("TODO: implement label");
}

/**
 * Renders a card, exactly:
 *
 *   Title
 *   -----        <- as many dashes as the title is long
 *   - line one
 *   - line two
 *
 * with no trailing newline. `lines` may be empty, in which case the card is
 * the title and the dashes only.
 *
 * TODO: implement as a multiline template literal.
 */
export function card(_title, _lines) {
  throw new Error("TODO: implement card");
}

/**
 * Returns `value: ${value}` for whatever it is handed.
 *
 * Every `${}` runs the same conversion an implicit String() would, so an
 * object contributes its toString(), null prints as "null", and a symbol —
 * which has no string conversion — throws a TypeError. Do not defend
 * against that: letting it out is the point.
 *
 * TODO: implement.
 */
export function interpolate(_value) {
  throw new Error("TODO: implement interpolate");
}
