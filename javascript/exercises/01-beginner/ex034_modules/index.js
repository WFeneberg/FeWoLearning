// Exercise 034 — modules (beginner).
// Goal:   import the three ways, and see that an import is a live binding.
// Drills: named imports, a default import under a name of your choosing,
//         namespace imports, re-exporting, module state as a singleton.
// Passes: readCount() reports the CURRENT value rather than the one that
//         was there when this module was first evaluated.
//
// Coming from C#: there is no assembly and no namespace — a module is a
// file, its exports are its public surface, and it is evaluated once per
// process no matter how many files import it.

/**
 * The current value of `count` from ./deps.js.
 *
 * TODO: import it and return it. An imported binding is a live view of the
 * exporting module's variable, so this must not capture a snapshot at
 * module-evaluation time.
 */
export function readCount() {
  throw new Error("TODO: implement readCount");
}

/**
 * Calls deps' increment() twice and returns the new count.
 *
 * TODO: implement. Note that assigning to an imported binding is a
 * TypeError — the exporting module owns it.
 */
export function bumpTwice() {
  throw new Error("TODO: implement bumpTwice");
}

/**
 * add(a, b) then multiply by `factor`, using the NAMED export and the
 * DEFAULT export of ./deps.js.
 *
 * TODO: implement — import the default under the name `scale`.
 */
export function sumThenScale(_a, _b, _factor) {
  throw new Error("TODO: implement sumThenScale");
}

/**
 * The keys of a namespace import of ./deps.js, sorted. A default export
 * appears under the key "default".
 *
 * TODO: implement with `import * as deps`.
 */
export function namespaceKeys() {
  throw new Error("TODO: implement namespaceKeys");
}

// TODO: re-export deps' `add` from this module as well, so a consumer can
// import it from here without knowing about ./deps.js.
