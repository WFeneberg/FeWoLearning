// Exercise 074 — Proxy.revocable (advanced).
// Goal:   hand out access you can take back.
// Drills: Proxy.revocable, what a revoked proxy still allows, and scoping
//         a capability to one call.
// Passes: after revocation every operation throws a TypeError, while the
//         underlying object is untouched and still usable by its owner.

/**
 * { view, revoke } over `target`.
 *
 * TODO: implement with Proxy.revocable.
 */
export function share(_target) {
  throw new Error("TODO: implement share");
}

/**
 * Calls `fn(view)` with a revocable view of `resource`, revokes it
 * afterwards — even if `fn` throws — and returns fn's result.
 *
 * TODO: implement.
 */
export function withTemporaryAccess(_resource, _fn) {
  throw new Error("TODO: implement withTemporaryAccess");
}

/**
 * Revokes a view and then reports what each operation does, as "ok" or the
 * caught error's name:
 *   { read, write, typeOf, isProxyEqual }
 * where typeOf is `typeof view` (a string, and never an error) and
 * isProxyEqual is whether the revoked view is still === itself.
 *
 * TODO: implement — identity and typeof survive revocation; nothing else
 * does.
 */
export function afterRevocation() {
  throw new Error("TODO: implement afterRevocation");
}
