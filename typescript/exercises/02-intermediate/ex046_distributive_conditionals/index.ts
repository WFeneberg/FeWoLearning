// Exercise 046 — distribution, and how to switch it off (intermediate).
// Goal:   control whether a conditional asks about a union member by member
//         or all at once.
// Drills: the naked type parameter rule, the `[T] extends [U]` brackets,
//         and what both do to `never`.
// Passes: the two classifiers disagree exactly where they should.
//
// ex045 relied on distribution without naming it. The rule: when the
// CHECKED type of a conditional is a bare type parameter — `T extends U`,
// with nothing wrapped around T — and the argument is a union, the
// conditional is applied to each member separately and the results are
// unioned back together.
//
// Wrapping either side in a one-element tuple — `[T] extends [U]` — stops
// it, because T is no longer naked. The conditional then asks one question
// about the whole union. Both forms are correct; they answer different
// questions, and choosing the wrong one is a silent bug.
//
// THE TRAP, and it catches everyone once: a distributive conditional over
// `never` produces `never`, not the false branch. `never` is the EMPTY
// union, so distributing over it means running the conditional zero times
// and unioning nothing. The bracketed form has no such problem.

/** TODO: ask member by member. */
export type IsStringEach<T> = unknown;

/** TODO: ask once, about the whole thing. */
export type IsStringWhole<T> = unknown;

/** TODO: the members of T that are arrays. Distribution is what makes this
 *  a filter rather than an all-or-nothing answer. */
export type ArrayMembers<T> = unknown;
