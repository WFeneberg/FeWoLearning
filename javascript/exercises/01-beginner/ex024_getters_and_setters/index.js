// Exercise 024 — getters and setters (beginner).
// Goal:   expose a computed value that reads like a field.
// Drills: `get`/`set` in an object literal, validation in a setter,
//         what a property descriptor of an accessor looks like.
// Passes: `fahrenheit` is a real accessor — its descriptor has a `get`, not
//         a `value` — and writing to it moves `celsius`.

/**
 * Returns a temperature object:
 *   - `celsius`: readable and writable
 *   - `fahrenheit`: computed from celsius, readable and writable
 *   - setting either below absolute zero (-273.15 °C) — including at
 *     construction — throws a RangeError with the message
 *     "below absolute zero"
 *
 * TODO: implement with `get fahrenheit()` / `set fahrenheit(value)` and a
 * `set celsius(value)` that validates. Keep the number behind the scenes so
 * the setter cannot be bypassed.
 */
export function makeTemperature(_celsius) {
  throw new Error("TODO: implement makeTemperature");
}

/**
 * Returns an object with a `callCount` property and a `value` accessor that
 * increments `callCount` every time it is READ. Proof that a getter is a
 * function call wearing a property's clothes.
 *
 * TODO: implement.
 */
export function makeProbe() {
  throw new Error("TODO: implement makeProbe");
}
