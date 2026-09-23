// Exercise 076 — nominal typing, built by hand (advanced).
// Goal:   make two types that are structurally identical refuse to be
//         swapped.
// Drills: a phantom property, `unique symbol` as a brand key, a reusable
//         Brand helper.
// Passes: a branded id is still usable as a string, a plain string is not
//         usable as an id, and two differently branded ids do not mix.
//
// ex001 established that TypeScript compares shapes. Usually that is the
// good news. It stops being good news when `UserId` and `OrderId` are
// both `string`: every function taking one accepts the other, and the
// bug that follows is silent and expensive.
//
// The fix is to add something to the type that nothing else has. The
// property is PHANTOM — it is declared, never assigned, and does not
// exist at runtime. Its only job is to make the shapes differ.
//
// Two details make the difference between a brand that works and one that
// leaks. The key should be a `unique symbol`, declared but never defined,
// so no object literal can accidentally supply it and no other module can
// name it. And the brand should INTERSECT with the base type rather than
// wrap it, so a UserId is still a string everywhere a string will do —
// `.toUpperCase()` keeps working, and only the direction that matters is
// blocked.
//
// This is the one place where TypeScript asks for a trick where C# has a
// keyword. The consolation is that it costs nothing at runtime: a branded
// string IS a string, with no wrapper and no allocation.

declare const brand: unique symbol;

/** TODO: T, plus a phantom marker carrying B. */
export type Brand<T, B extends string> = unknown;

/** TODO: a string branded "UserId". */
export type UserId = unknown;

/** TODO: a string branded "OrderId". */
export type OrderId = unknown;

/** TODO: the deliberate way in — the one cast the design allows. */
export function asUserId(_value: string): UserId {
  throw new Error("TODO: implement asUserId");
}

/** TODO: `user:<id>`. Takes a UserId and nothing else. */
export function describeUser(_id: UserId): string {
  throw new Error("TODO: implement describeUser");
}
