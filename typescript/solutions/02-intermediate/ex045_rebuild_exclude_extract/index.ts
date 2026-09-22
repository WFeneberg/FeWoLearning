// Reference solution — exercise 045.
// `T` is bare in the checked position, so each of these distributes over a
// union argument rather than testing it as a whole.
export type MyExclude<T, U> = T extends U ? never : T;

export type MyExtract<T, U> = T extends U ? T : never;

export type MyNonNullable<T> = MyExclude<T, null | undefined>;
