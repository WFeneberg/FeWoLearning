// Exercise 067 — Intl (intermediate).
// Goal:   format for a locale you name, never for the machine's.
// Drills: NumberFormat, DateTimeFormat with an explicit time zone,
//         RelativeTimeFormat, PluralRules, and formatToParts.
// Passes: the tests assert PARTS rather than whole strings, because the
//         spaces and separators ICU uses change between versions — and
//         because the ambient locale here is de-CH, not English.

/**
 * The parts of a currency-formatted number, as an array of
 * { type, value } — exactly what Intl.NumberFormat#formatToParts returns.
 *
 * TODO: implement.
 */
export function currencyParts(_value, _locale, _currency) {
  throw new Error("TODO: implement currencyParts");
}

/**
 * The group separator a locale uses for thousands: "," for en-US, "." for
 * de-DE.
 *
 * TODO: implement — format a big number to parts and pick the "group" one.
 */
export function groupSeparator(_locale) {
  throw new Error("TODO: implement groupSeparator");
}

/**
 * A date formatted as { year, month, day } strings in the given locale and
 * TIME ZONE — the time zone is not optional here, or the answer depends on
 * where the test runs.
 *
 * TODO: implement with DateTimeFormat + formatToParts.
 */
export function dateFields(_date, _locale, _timeZone) {
  throw new Error("TODO: implement dateFields");
}

/**
 * "in 3 days" / "3 days ago" in the given locale.
 *
 * TODO: implement with Intl.RelativeTimeFormat, numeric: "always".
 */
export function relative(_value, _unit, _locale) {
  throw new Error("TODO: implement relative");
}

/**
 * The plural category of a number in a locale: "one", "other", … — what a
 * translation file keys its messages on.
 *
 * TODO: implement with Intl.PluralRules.
 */
export function pluralCategory(_value, _locale) {
  throw new Error("TODO: implement pluralCategory");
}
