// Exercise 074 — every path into a type, as strings (advanced).
// Goal:   derive the set of legal dotted paths from an object type.
// Drills: recursion producing a union, template literals over a recursive
//         call, filtering keys to the string ones.
// Passes: Paths lists every leaf and every branch along the way, and
//         pathsOf produces the same set at runtime.
//
// This is the type behind every typed `get(obj, "a.b.c")` helper, and it
// is built from three pieces already seen: a mapped type indexed by its
// own keys to make a union (ex054), a template literal to join (ex047),
// and recursion (ex071).
//
// Two details decide whether it works.
//
// `keyof T & string` rather than `keyof T`. A key may be a symbol or a
// number, and neither interpolates into a path sensibly; intersecting
// with `string` keeps only what does.
//
// And BOTH the branch and the leaves belong in the union. `"a"` is a
// legal path in its own right, as well as being the prefix of `"a.b"` —
// so the object arm contributes `K | \`${K}.${Paths<T[K]>}\``, not just
// the second half.
//
// Scope, stated rather than discovered: this handles plain object trees.
// Arrays are treated as leaves — indexing into them by path needs
// `${number}` segments and is a different exercise — and so is anything
// with methods, for the reason ex072 gives about Date.

export interface Settings {
  id: string;
  display: {
    theme: string;
    layout: { columns: number; dense: boolean };
  };
  tags: string[];
}

/** TODO: every dotted path into T, branches included. */
export type Paths<T> = unknown;

/** TODO: the same set at runtime, in depth-first order: a branch before
 *  the paths beneath it. An array is a leaf. */
export function pathsOf(_value: object): string[] {
  throw new Error("TODO: implement pathsOf");
}
