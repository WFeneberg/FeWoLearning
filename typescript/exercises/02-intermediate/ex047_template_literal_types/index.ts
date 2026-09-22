// Exercise 047 — template literal types (intermediate).
// Goal:   compute string types the way you compute string values.
// Drills: interpolation in a type, `${number}` as a pattern, and the
//         cross product a union interpolation produces.
// Passes: the three types build the right strings, and className produces
//         one at runtime.
//
// A template literal type has the same syntax as a template literal value
// and lives in type position: `` `on${K}` `` where K is a string literal
// type gives another string literal type. There is no C# equivalent — the
// nearest thing is a source generator, again.
//
// Two behaviours to notice. Interpolating `${number}` does not produce one
// type; it produces a PATTERN, so "4px" is assignable to `${number}px` and
// "4em" is not. And interpolating a UNION produces the cross product:
// `` `${"a"|"b"}-${"x"|"y"}` `` is four literals, not two. That multiplies
// fast, which is worth knowing before you write one over `keyof T`.

/** TODO: an event name — EventName<"click"> is "onclick". */
export type EventName<K extends string> = unknown;

/** TODO: a CSS pixel length — any number followed by "px". */
export type Px = unknown;

/** TODO: `<variant>-<size>`, for every combination of the two. */
export type ClassName<V extends string, S extends string> = unknown;

/** TODO: the class name for one variant and size. */
export function className<V extends string, S extends string>(
  _variant: V,
  _size: S,
): ClassName<V, S> {
  throw new Error("TODO: implement className");
}
