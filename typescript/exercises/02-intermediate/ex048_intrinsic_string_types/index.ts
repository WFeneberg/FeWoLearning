// Exercise 048 — the four string intrinsics (intermediate).
// Goal:   change the case of a string type, and use it to derive a whole
//         object type.
// Drills: Capitalize, Uppercase, and composing them with ex047's templates
//         and ex038's key remapping.
// Passes: the two name-builders produce the right literals, and Handlers
//         renames a whole type's keys.
//
// Uppercase, Lowercase, Capitalize and Uncapitalize look like the library
// types of ex041-ex045, but they are not: they are INTRINSICS, implemented
// inside the compiler. lib.es5.d.ts declares them as
// `type Uppercase<S extends string> = intrinsic`, and there is no way to
// write them yourself — string manipulation at the type level otherwise
// stops at concatenation and pattern matching.
//
// (Which is why that keyword exists at all. Nothing else in the standard
// library uses it.)
//
// Composing them is where they earn their place: `` `on${Capitalize<K>}` ``
// over `keyof T` turns a data type into its handler type, with no chance
// of the two drifting apart.

/** TODO: HandlerName<"click"> is "onClick". */
export type HandlerName<K extends string> = unknown;

/** TODO: EnvVar<"port"> is "APP_PORT". */
export type EnvVar<K extends string> = unknown;

/** TODO: one handler per key of T, each taking that key's value type and
 *  returning void. For { click: MouseEvent } that is
 *  { onClick: (value: MouseEvent) => void }. */
export type Handlers<T> = unknown;

/** The same name transformation at runtime. */
export function handlerName<K extends string>(_key: K): HandlerName<K> {
  throw new Error("TODO: implement handlerName");
}
