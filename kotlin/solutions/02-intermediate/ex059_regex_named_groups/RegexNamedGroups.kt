package fewolearning.exercises.intermediate.ex059_regex_named_groups

data class IsoDateParts(val year: String, val month: String, val day: String)

private val isoDateRegex = Regex("""(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})""")

/** Extracts year/month/day from an ISO date using named capture groups. */
fun extractDateParts(isoDate: String): IsoDateParts? {
    val match = isoDateRegex.matchEntire(isoDate) ?: return null
    return IsoDateParts(
        match.groups["year"]!!.value,
        match.groups["month"]!!.value,
        match.groups["day"]!!.value
    )
}
