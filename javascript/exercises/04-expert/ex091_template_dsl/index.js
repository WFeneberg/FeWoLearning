// Exercise 091 — a tagged-template query builder (expert).
// Goal:   a DSL where the values CANNOT become syntax.
// Drills: a tag returning a structure instead of a string, parameter
//         placeholders, composing fragments with renumbering, and one
//         explicit, ugly escape hatch.
// Passes: a value containing "; DROP TABLE users --" ends up in `values`
//         and never in `text` — that is the whole point of the row.

/**
 * sql`SELECT * FROM t WHERE id = ${id}` ->
 *   { text: "SELECT * FROM t WHERE id = $1", values: [id] }
 *
 * Placeholders are numbered from $1 in order of appearance.
 *
 * A value that is itself a fragment (something sql`` produced) is INLINED:
 * its text is spliced in and its values appended, with every placeholder
 * renumbered to match its new position.
 *
 * A value produced by raw() is spliced in as literal text with no
 * placeholder at all.
 *
 * TODO: implement.
 */
export function sql(_strings, ..._values) {
  throw new Error("TODO: implement sql");
}

/**
 * Marks a string as literal SQL text rather than a value. The deliberate
 * escape hatch: everything it is given goes straight into the query, so a
 * caller has to opt in visibly.
 *
 * TODO: implement — return a tagged wrapper sql() can recognise.
 */
export function raw(_text) {
  throw new Error("TODO: implement raw");
}

/**
 * Joins fragments with `separator` into one fragment, renumbering as it
 * goes. join([], ", ") is an empty fragment.
 *
 * TODO: implement.
 */
export function join(_fragments, _separator) {
  throw new Error("TODO: implement join");
}
