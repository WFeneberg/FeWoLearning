// Exercise 086 — dynamic import and module cycles (advanced).
// Goal:   load a module on demand, and know what a cycle does.
// Drills: import() as an expression, the namespace object it resolves to,
//         one-evaluation-per-module, and what a half-initialised module
//         exposes to the other half of a cycle.
// Passes: two dynamic imports of the same module give the SAME namespace
//         object, and the cycle report shows a hoisted function surviving
//         where a const does not.

/**
 * Loads ./registry.js on demand and returns its namespace object.
 *
 * TODO: implement with `await import(...)`. A static import at the top of
 * this file would defeat the purpose.
 */
export async function loadRegistry() {
  throw new Error("TODO: implement loadRegistry");
}

/**
 * Loads ./registry.js twice and returns
 *   { same, firstCall, secondCall }
 * where `same` is whether the two namespace objects are identical, and the
 * two calls are the results of calling bump() once through each.
 *
 * A module is evaluated once per URL, so the second call continues the
 * first one's state: 1 then 2.
 *
 * TODO: implement.
 */
export async function loadTwice() {
  throw new Error("TODO: implement loadTwice");
}

/**
 * Loads the module named by `key` from a registry of loaders:
 *   "registry" -> ./registry.js
 *   "cycle"    -> ./cycle-a.js
 * and returns its namespace object. An unknown key rejects with a
 * RangeError.
 *
 * TODO: implement with an object of `() => import(...)` functions —
 * a bare `import(variable)` is not statically analysable, and bundlers
 * treat it differently from a literal path.
 */
export async function loadByKey(_key) {
  throw new Error("TODO: implement loadByKey");
}

/**
 * The `report` object from ./cycle-a.js.
 *
 * TODO: implement.
 */
export async function cycleReport() {
  throw new Error("TODO: implement cycleReport");
}
