// Exercise 082 — explicit resource management (advanced).
// Goal:   deterministic cleanup, in LIFO order, even when something throws.
// Drills: Symbol.dispose, DisposableStack, .use/.adopt/.defer, .move(),
//         and SuppressedError.
// Passes: the resources are released newest-first, and a throwing body
//         still releases every one of them.
//
// Note: the `using` DECLARATION is syntax this track does not rely on —
// the bundler in front of Vitest need not support it. Everything here is
// graded through explicit .dispose() calls, which is the same machinery.

/**
 * An object with a Symbol.dispose method that pushes `name` onto `log`.
 * Also exposes `disposed` (a boolean) so a test can read its state.
 *
 * TODO: implement.
 */
export function makeResource(_name, _log) {
  throw new Error("TODO: implement makeResource");
}

/**
 * Registers every resource in `resources` (each one disposable) on a
 * DisposableStack, calls `body(stack)`, and disposes the stack afterwards
 * — in reverse registration order, and even if `body` throws.
 *
 * Returns whatever `body` returned.
 *
 * TODO: implement with a DisposableStack and .use().
 */
export function withResources(_resources, _body) {
  throw new Error("TODO: implement withResources");
}

/**
 * Builds a DisposableStack that
 *   - adopts `value` with a disposer calling onRelease(value)
 *   - defers a callback pushing "deferred" onto `log`
 * and returns the stack WITHOUT disposing it.
 *
 * TODO: implement with .adopt() and .defer().
 */
export function buildStack(_value, _onRelease, _log) {
  throw new Error("TODO: implement buildStack");
}

/**
 * Moves a stack's registered disposers into a NEW stack, disposes the old
 * one, and returns the new one — the transfer-ownership pattern for a
 * constructor that may fail halfway.
 *
 * Returns { moved, oldDisposed } where `moved` is the new stack.
 *
 * TODO: implement with .move().
 */
export function transferOwnership(_stack) {
  throw new Error("TODO: implement transferOwnership");
}
