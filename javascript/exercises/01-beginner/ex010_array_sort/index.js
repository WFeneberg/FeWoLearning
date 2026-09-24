// Exercise 010 — sorting (beginner).
// Goal:   never be surprised by [1, 10, 9] again.
// Drills: sort's default string comparison, the comparator contract,
//         stability, toSorted as the copying twin of sort.
// Passes: numbers sort numerically, ties keep their original order, and no
//         function here reorders its input.
//
// Coming from C#: `sort` with no comparator converts every element to a
// string. `[10, 9, 1].sort()` is `[1, 10, 9]`, and it is not a bug.

/**
 * Ascending numeric order, as a NEW array.
 *
 * TODO: implement.
 */
export function sortNumbers(_values) {
  throw new Error("TODO: implement sortNumbers");
}

/**
 * Sorts people by `dept` (A–Z, case-sensitive is fine) and, within a
 * department, by descending `score`. People with the same dept AND score
 * must keep their original relative order — sort has been required to be
 * stable since ES2019, so this needs no tiebreaker of its own.
 *
 * Returns a NEW array.
 *
 * TODO: implement with one comparator.
 */
export function sortStaff(_people) {
  throw new Error("TODO: implement sortStaff");
}

/**
 * Sorts strings with the caller's locale rules — "ä" belongs next to "a" in
 * German, and the default `<` comparison puts it after "z".
 *
 * TODO: implement with localeCompare and the given locale.
 */
export function sortNames(_names, _locale) {
  throw new Error("TODO: implement sortNames");
}
