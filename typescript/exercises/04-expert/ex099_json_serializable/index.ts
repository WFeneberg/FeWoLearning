// Exercise 099 — what survives JSON.stringify (expert).
// Goal:   describe, in the type system, what a value becomes after a
//         round trip through JSON.
// Drills: recursion with several leaf cases, dropping keys with `as`
//         (ex038), the types the serializer quietly changes.
// Passes: every rule below, including the three that surprise people.
//
// `JSON.stringify` does not preserve a value; it TRANSFORMS it, and the
// transformation is lossy in ways that are easy to forget until a field
// arrives as a string. A `Json<T>` type writes the rules down.
//
// What the serializer does, and what the type therefore has to do:
//
//   a function-valued property is DROPPED entirely, key and all
//   an undefined-valued property is DROPPED, key and all
//   a Date becomes a STRING, via its toJSON
//   a Map or a Set becomes an empty object, contents and all
//   everything else recurses
//
// The first two are why the result is not simply "the same shape with
// some things changed": keys disappear, so the mapped type needs an
// `as` clause to filter them out (ex038).
//
// The Date rule is the one that bites in practice. A DTO typed with a
// Date is a lie the moment it crosses a wire, and every codebase that
// has met this has a bug where `createdAt.getTime` was not a function.
// Matching it means checking for `toJSON` BEFORE the object arm, since
// a Date is an object too.
//
// Note that this types a round trip in ONE direction. `Json<T>` is what
// comes out of stringify; parsing back in is untyped (ex035), and the
// fact that the two are different types is the whole point.

/** TODO: the shape T takes after a round trip through JSON. */
export type Json<T> = unknown;

/** TODO: stringify and parse back, reporting the transformed type. */
export function roundTrip<T>(_value: T): Json<T> {
  throw new Error("TODO: implement roundTrip");
}
