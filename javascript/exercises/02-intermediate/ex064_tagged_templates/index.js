// Exercise 064 — tagged templates (intermediate).
// Goal:   intercept a template literal before it becomes a string.
// Drills: the (strings, ...values) signature, strings.raw, interleaving,
//         escaping interpolated values, String.raw.
// Passes: html() escapes the VALUES and not the literal parts — which is
//         the whole reason a tag is safer than concatenation.

/**
 * Returns { strings, values, raw } for whatever template it tags:
 *   strings: the literal parts, as a plain array
 *   values: the interpolated values
 *   raw: the literal parts before escape processing
 *
 * TODO: implement.
 */
export function inspect(_strings, ..._values) {
  throw new Error("TODO: implement inspect");
}

/**
 * Joins the parts, escaping &, <, >, " and ' in every interpolated VALUE
 * while leaving the literal parts alone.
 *
 * html`<p>${"<script>"}</p>` -> "<p>&lt;script&gt;</p>"
 *
 * TODO: implement.
 */
export function html(_strings, ..._values) {
  throw new Error("TODO: implement html");
}

/**
 * Joins the parts using the RAW strings, so "\n" in the source stays two
 * characters — what String.raw does.
 *
 * TODO: implement without calling String.raw.
 */
export function keepRaw(_strings, ..._values) {
  throw new Error("TODO: implement keepRaw");
}
