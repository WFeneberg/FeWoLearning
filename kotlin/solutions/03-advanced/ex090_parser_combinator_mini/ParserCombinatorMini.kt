package fewolearning.exercises.advanced.ex090_parser_combinator_mini

typealias Parser<T> = (String) -> Pair<T, String>?

/** Matches [expected] as the input's first character, returning it paired with the remainder. */
fun charParser(expected: Char): Parser<Char> = { input ->
    if (input.isNotEmpty() && input[0] == expected) {
        expected to input.substring(1)
    } else {
        null
    }
}

/** Applies [parser] repeatedly, collecting every success; always succeeds, even with zero matches. */
fun <T> repeatParser(parser: Parser<T>): Parser<List<T>> = { input ->
    val results = mutableListOf<T>()
    var remaining = input
    while (true) {
        val next = parser(remaining) ?: break
        results.add(next.first)
        remaining = next.second
    }
    results.toList() to remaining
}
