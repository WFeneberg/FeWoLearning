// Exercise 087 — AsyncLocalStorage (advanced).
// Goal:   carry a request id through an async call chain without a parameter.
// Drills: node:async_hooks, run(), getStore(), nesting, and the fact that
//         the context survives an await but not a bare setTimeout escape.
// Passes: two concurrent "requests" never see each other's context, which
//         is the only thing that makes this pattern usable at all.

/**
 * Runs `fn` with `context` as the ambient store and returns its result.
 *
 * TODO: implement with an AsyncLocalStorage instance shared by this module.
 */
export function withContext(_context, _fn) {
  throw new Error("TODO: implement withContext");
}

/**
 * The current context, or undefined outside any withContext() call.
 *
 * TODO: implement.
 */
export function currentContext() {
  throw new Error("TODO: implement currentContext");
}

/**
 * Awaits a microtask and then returns currentContext()?.id — proof that
 * the context follows the continuation rather than the call stack.
 *
 * TODO: implement.
 */
export async function idAfterAwait() {
  throw new Error("TODO: implement idAfterAwait");
}

/**
 * Runs `fn` with the current context extended by `extra`, without
 * disturbing the outer one.
 *
 * TODO: implement.
 */
export function withExtra(_extra, _fn) {
  throw new Error("TODO: implement withExtra");
}
