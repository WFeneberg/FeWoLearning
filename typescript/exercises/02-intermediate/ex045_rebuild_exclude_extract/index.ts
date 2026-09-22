// Exercise 045 — rebuilding Exclude and Extract (intermediate).
// Goal:   filter a union member by member.
// Drills: the distributive conditional type, and NonNullable as a
//         two-word application of it.
// Passes: all three filter unions correctly, including down to never.
//
// Exclude is `T extends U ? never : T` and Extract is the same with the
// arms swapped. One line each, and both depend entirely on a behaviour
// that has not been named yet.
//
// When the checked type of a conditional is a BARE type parameter and the
// argument is a union, the conditional DISTRIBUTES: it is applied to each
// member separately and the results are unioned back together. So
// `MyExclude<"a" | "b", "b">` is not one question about the whole union —
// it is `("a" extends "b" ? never : "a") | ("b" extends "b" ? never : "b")`,
// which collapses to `"a"`, because a union with `never` in it drops the
// never.
//
// Without distribution these would be useless: the whole union would be
// tested against U once and the answer would be all-or-nothing. ex046
// covers how to turn it OFF, and why you sometimes must.
//
// Type-level only; there is nothing to run.

/** TODO: the members of T that are NOT assignable to U. */
export type MyExclude<T, U> = unknown;

/** TODO: the members of T that ARE assignable to U. */
export type MyExtract<T, U> = unknown;

/** TODO: T without null and undefined. Build it from MyExclude. */
export type MyNonNullable<T> = unknown;
