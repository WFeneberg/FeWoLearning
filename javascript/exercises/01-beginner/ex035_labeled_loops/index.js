// Exercise 035 — labeled break and continue (beginner).
// Goal:   leave a nested loop without a flag variable.
// Drills: `outer: for (…) { … break outer; }`, labeled continue, stopping
//         as early as the answer allows.
// Passes: findInGrid() stops visiting cells the moment it has the answer,
//         which a flag-checked loop would not.

/**
 * Finds `target` in a 2-D array and returns { row, col }, or null.
 * Must stop as soon as it is found — the tests count the visits.
 *
 * TODO: implement with a labeled break (no early `return` from inside the
 * loops, so that the label is what does the work).
 */
export function findInGrid(_grid, _target) {
  throw new Error("TODO: implement findInGrid");
}

/**
 * Sums every row that contains no negative number, skipping the rest
 * entirely — as soon as a negative shows up, move on to the next row.
 *
 * TODO: implement with a labeled continue.
 */
export function sumCleanRows(_grid) {
  throw new Error("TODO: implement sumCleanRows");
}

/**
 * The first value that appears in EVERY one of the given lists, or null.
 * Compare with ===, and scan `lists[0]` in order.
 *
 * TODO: implement with a labeled continue that abandons a candidate as soon
 * as one list is missing it.
 */
export function firstCommonValue(_lists) {
  throw new Error("TODO: implement firstCommonValue");
}
