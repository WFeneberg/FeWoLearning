// Exercise 020 — JSON (beginner).
// Goal:   know what survives a JSON round trip and what quietly does not.
// Drills: stringify/parse, a replacer function, a reviver, stable key order.
// Passes: redact() removes keys during serialization rather than after, and
//         parseWithDates() rebuilds Date objects on the way back in.

/** JSON.parse(JSON.stringify(value)). TODO: implement. */
export function roundTrip(_value) {
  throw new Error("TODO: implement roundTrip");
}

/**
 * Serializes `value` but drops every property whose key is in `secretKeys`,
 * at ANY depth, replacing nothing (the key is simply absent).
 *
 * TODO: implement with a replacer FUNCTION — returning undefined from it
 * omits the property. Deleting keys from a copy afterwards is the thing
 * this row exists to avoid.
 */
export function redact(_value, _secretKeys) {
  throw new Error("TODO: implement redact");
}

/**
 * Parses `json` and turns every string that looks like an ISO timestamp
 * ("2024-01-31T12:00:00.000Z") into a Date, at any depth.
 *
 * TODO: implement with a reviver.
 */
export function parseWithDates(_json) {
  throw new Error("TODO: implement parseWithDates");
}

/**
 * Serializes an object with its keys in sorted order, so two objects with
 * the same content produce the same string.
 *
 * TODO: implement. JSON.stringify takes an array of key names as its second
 * argument — and applies it at every level.
 */
export function stringifyStable(_object) {
  throw new Error("TODO: implement stringifyStable");
}
