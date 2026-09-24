// Support module for exercise 034 — NOT a TODO, and identical in
// exercises/ and solutions/. It exists so index.js has something to import.

/** A mutable module-level value. Importers see it CHANGE — imports are bindings, not copies. */
export let count = 0;

/** The only way to move `count`, since an importer may not assign to an imported binding. */
export function increment() {
  count += 1;
}

export function add(a, b) {
  return a + b;
}

// One default export per module. Its name here is local; importers choose
// their own.
export default function multiply(a, b) {
  return a * b;
}
