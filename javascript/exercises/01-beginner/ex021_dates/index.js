// Exercise 021 — dates (beginner).
// Goal:   do date arithmetic that does not depend on where the machine is.
// Drills: Date as a millisecond instant, UTC accessors, ISO parsing rules,
//         setDate() mutating in place.
// Passes: addDays() returns a new Date and leaves its argument alone, and
//         every result here is the same in every time zone.
//
// Coming from .NET: there is no DateOnly and no DateTimeOffset. A Date is a
// single number — milliseconds since the epoch — and the local time zone is
// only a rendering choice. Do the arithmetic in UTC and nothing drifts.

/** "YYYY-MM-DD" in UTC. TODO: implement. */
export function toIsoDate(_date) {
  throw new Error("TODO: implement toIsoDate");
}

/**
 * A NEW Date `days` later. Negative values go back.
 *
 * TODO: implement. Note that `date.setDate(...)` changes the date you were
 * given — copy first.
 */
export function addDays(_date, _days) {
  throw new Error("TODO: implement addDays");
}

/**
 * Whole days from `from` to `to`, measured between their UTC midnights, so
 * a daylight-saving jump cannot make it 0.999.
 *
 * TODO: implement.
 */
export function daysBetween(_from, _to) {
  throw new Error("TODO: implement daysBetween");
}

/** A NEW Date at 00:00:00.000 UTC of the same day. TODO: implement. */
export function startOfUtcDay(_date) {
  throw new Error("TODO: implement startOfUtcDay");
}
