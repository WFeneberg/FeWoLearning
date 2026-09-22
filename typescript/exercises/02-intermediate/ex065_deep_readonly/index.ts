// Exercise 065 — readonly all the way down (intermediate).
// Goal:   write the first recursive mapped type, and stop it in the right
//         places.
// Drills: recursion in a mapped type, a conditional guard on what to
//         recurse into, arrays as a separate case.
// Passes: nested objects and arrays are frozen, and functions and
//         primitives come through untouched.
//
// `Readonly<T>` is one level (ex037). Recursing is easy; knowing where to
// STOP is the row.
//
// Three cases, and the middle one is what a naive version gets wrong:
//
//   a FUNCTION is an object as far as `keyof` is concerned, so mapping
//   over it produces `{}` — the signature is not a property and does not
//   survive. Any recursive mapper has to check for a function FIRST and
//   pass it through, or every callback in the tree silently becomes an
//   empty object.
//
//   an ARRAY needs `readonly T[]`, not a mapped object: mapping over an
//   array type visits its numeric keys AND its methods, which is not what
//   anyone means.
//
//   a PRIMITIVE has no keys, so recursing into it is harmless but
//   pointless; stopping there keeps the type readable.
//
// deepFreeze is the runtime half, and Object.freeze is shallow — which is
// exactly the same lesson from the other side.

/** TODO: readonly at every level. Leave functions and primitives alone. */
export type DeepReadonly<T> = unknown;

/** TODO: freeze `value` and everything reachable from it, then return it.
 *  Object.freeze alone is shallow. */
export function deepFreeze<T>(_value: T): T {
  throw new Error("TODO: implement deepFreeze");
}
