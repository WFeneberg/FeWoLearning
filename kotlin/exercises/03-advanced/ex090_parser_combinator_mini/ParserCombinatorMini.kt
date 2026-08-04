package fewolearning.exercises.advanced.ex090_parser_combinator_mini

/*
Exercise 090 - Mini parser combinator (advanced).

Goal:   Compose small parsing functions that consume a prefix and return the remainder.
Drills: higher-order parsers, composition.
*/
typealias Parser<T> = (String) -> Pair<T, String>?

fun charParser(expected: Char): Parser<Char> {
    TODO()
}

fun <T> repeatParser(parser: Parser<T>): Parser<List<T>> {
    TODO()
}
