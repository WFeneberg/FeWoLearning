// Exercise 004 — number precision (beginner).
// Goal:   deal with the fact that every number here is a float64.
// Drills: 0.1 + 0.2, Number.EPSILON, toFixed, Number.isInteger vs
//         Number.isSafeInteger, summing money without drift.
// Passes: nearlyEqual() tolerates representation error, formatMoney() always
//         prints two decimals, and sumAmounts() gets 0.30 out of 0.1 + 0.2.
//
// Coming from C#: there is no `decimal`, and no `int` either — `1` and `1.0`
// are the same value. The integer guarantee runs out at 2^53 - 1, which is
// what Number.MAX_SAFE_INTEGER names.

/**
 * True when a and b differ by no more than `epsilon`, which defaults to
 * Number.EPSILON scaled by the larger magnitude (so it works away from 1.0
 * as well). NaN is never nearly equal to anything.
 *
 * TODO: implement.
 */
export function nearlyEqual(_a, _b, _epsilon) {
  throw new Error("TODO: implement nearlyEqual");
}

/**
 * Formats a number as a plain string with exactly two decimals: 1 -> "1.00",
 * 1.005 -> whatever toFixed does with it, -0 -> "0.00".
 *
 * TODO: implement.
 */
export function formatMoney(_value) {
  throw new Error("TODO: implement formatMoney");
}

/**
 * Sums amounts given in whole units (e.g. 0.1, 0.2) and returns a value that
 * compares exactly equal to the decimal answer: sumAmounts([0.1, 0.2]) === 0.3.
 * Amounts have at most two decimals.
 *
 * TODO: implement. The usual trick is to do the arithmetic in the smallest
 * unit — cents — and divide once at the end.
 */
export function sumAmounts(_amounts) {
  throw new Error("TODO: implement sumAmounts");
}

/**
 * True only for values that are integers AND within the range where float64
 * integers are exact.
 *
 * TODO: implement.
 */
export function isExactInteger(_value) {
  throw new Error("TODO: implement isExactInteger");
}
