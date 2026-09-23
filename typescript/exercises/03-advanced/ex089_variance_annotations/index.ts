// Exercise 089 — saying which way a type parameter goes (advanced).
// Goal:   build the three variance shapes deliberately, and annotate
//         them.
// Drills: `out` for a producer, `in` for a consumer, `in out` for
//         something that does both.
// Passes: the producer is covariant, the consumer contravariant, and
//         the box neither.
//
// A type parameter's variance is decided by WHERE it appears. Used only
// in output positions it is covariant, so `Producer<Dog>` is a
// `Producer<Animal>`: anything expecting to be handed an Animal is happy
// being handed a Dog. Used only in input positions it is contravariant,
// and the arrow reverses: a `Consumer<Animal>` can stand in for a
// `Consumer<Dog>`, because something able to swallow any Animal can
// certainly swallow a Dog. Used in both, it is invariant and neither
// direction is allowed.
//
// TypeScript works all of that out on its own. So be clear about what
// `in` and `out` actually do, because the answer is not "change the
// assignability":
//
//   they are ASSERTIONS. Annotating `out T` on a parameter that is in
//   fact used contravariantly is an ERROR, at the declaration. The
//   annotation is a claim the compiler checks, which is why it protects
//   a future edit — add a `set(value: T)` to a `Producer<out T>` and the
//   build breaks at the interface rather than at some call site in
//   another package;
//
//   and they are a HINT. A large, deeply generic type graph is cheaper
//   to check when the variance is declared rather than inferred.
//
// Because assignability is inferred either way, the facts below grade
// the SHAPES you write, not the annotations. Write the annotations
// anyway — they are the row, and getting one wrong is the error that
// proves you understood the shape.
//
// One trap carried over from ex066: `accept` must be written as a
// PROPERTY of function type, not with method syntax. Method syntax is
// bivariant, and a bivariant parameter makes Consumer assignable in both
// directions, which is not contravariance.

export interface Animal {
  name: string;
}

export interface Dog extends Animal {
  name: string;
  breed: string;
}

// The stubs share a BIVARIANT placeholder — method syntax, for the
// reason ex066 measured, and returning void so no covariance sneaks in
// through the result — so that none of the three facts is satisfied
// before the real members are written. A covariant placeholder would
// make the Producer fact green on the untouched tree.

/** TODO: something that only hands out a T. Annotate the variance. */
export interface Producer<T> {
  /** TODO: replace with the real member. */
  placeholder?(value: T): void;
}

/** TODO: something that only takes a T. Annotate the variance, and mind
 *  the property-versus-method trap above. */
export interface Consumer<T> {
  /** TODO: replace with the real member. */
  placeholder?(value: T): void;
}

/** TODO: something that does both. Annotate the variance. */
export interface Box<T> {
  /** TODO: replace with the real members. */
  placeholder?(value: T): void;
}
