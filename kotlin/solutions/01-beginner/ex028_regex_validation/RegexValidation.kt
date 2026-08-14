package fewolearning.exercises.beginner.ex028_regex_validation

/*
Exercise 028 - Regex validation (reference solution).
*/
private val EMAIL_REGEX = Regex("^([A-Za-z0-9._%+-]+)@([A-Za-z0-9.-]+\\.[A-Za-z]{2,})$")

fun isValidEmail(candidate: String): Boolean = EMAIL_REGEX.matches(candidate)

fun userAndDomain(email: String): Pair<String, String>? {
    val match = EMAIL_REGEX.matchEntire(email) ?: return null
    val (user, domain) = match.destructured
    return Pair(user, domain)
}
