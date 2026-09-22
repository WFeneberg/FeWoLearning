// Exercise 055 — two ways to be exhaustive, and which one tells you
// earlier (intermediate).
// Goal:   make an unhandled union member a compile error, twice over.
// Drills: a `never` parameter in a switch default, and a total Record as
//         the same guarantee expressed in a declaration.
// Passes: both dispatchers cover every Kind, and an incomplete map is
//         rejected.
//
// ex013 built assertNever. This row is about WHERE the failure shows up
// when a union gains a member.
//
// With a switch, the error lands at the assertNever call inside the
// function — one error, in the code that forgot, which is exactly right
// when the handling differs per case.
//
// With a total map — `Record<Kind, V>` — the error lands at the
// DECLARATION of the map, before any dispatch is written. That is earlier
// and closer to the data, and it is the better shape whenever the cases
// are uniform. It also fails for a second reason worth knowing: Record
// rejects a key that is NOT in the union as well, so a stale entry left
// behind after a rename is caught too, which the switch happily ignores.
//
// CompleteMap's stub is `unknown`, which accepts any object, so the
// rejection fact below starts red.

export type Kind = "draft" | "published" | "archived";

/** TODO: an object with an entry for EVERY member of K and no others. */
export type CompleteMap<K extends string, V> = unknown;

/** TODO: "Draft", "Published", "Archived" — one entry per Kind. */
export const LABELS: CompleteMap<Kind, string> = {};

/** TODO: look the label up in LABELS. */
export function labelOf(_kind: Kind): string {
  throw new Error("TODO: implement labelOf");
}

/** TODO: accept only a value narrowed away to nothing. */
export function assertNever(_value: unknown): never {
  throw new Error("TODO: implement assertNever");
}

/**
 * TODO: the same answer via a switch, ending in an assertNever default —
 *   draft     -> "not yet"
 *   published -> "live"
 *   archived  -> "gone"
 */
export function statusOf(_kind: Kind): string {
  throw new Error("TODO: implement statusOf");
}
