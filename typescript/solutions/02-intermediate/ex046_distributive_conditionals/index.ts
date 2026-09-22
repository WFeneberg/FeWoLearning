// Reference solution — exercise 046.
// T is naked, so this distributes.
export type IsStringEach<T> = T extends string ? true : false;

// The brackets make T non-naked, so this asks once.
export type IsStringWhole<T> = [T] extends [string] ? true : false;

export type ArrayMembers<T> = T extends readonly unknown[] ? T : never;
