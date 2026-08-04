package fewolearning.exercises.intermediate.ex059_regex_named_groups

/*
Exercise 059 - Regex named groups (intermediate).

Goal:   Extract year/month/day from an ISO date using named capture groups.
Drills: named groups, match extraction.
*/
data class IsoDateParts(val year: String, val month: String, val day: String)

fun extractDateParts(isoDate: String): IsoDateParts? {
    TODO()
}
