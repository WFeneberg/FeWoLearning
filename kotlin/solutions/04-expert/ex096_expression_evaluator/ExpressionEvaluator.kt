package fewolearning.exercises.expert.ex096_expression_evaluator

/**
 * Recursive-descent evaluator over the grammar:
 *   expr   := term (('+' | '-') term)*
 *   term   := factor (('*' | '/') factor)*
 *   factor := number | '(' expr ')' | ('+' | '-') factor
 */
fun evaluate(expression: String): Double {
    val parser = Parser(tokenize(expression))
    val result = parser.parseExpression()
    require(parser.isAtEnd()) { "Unexpected trailing input in '$expression'" }
    return result
}

private sealed class Token {
    data class Number(val value: Double) : Token()
    data class Op(val symbol: Char) : Token()
    object LParen : Token()
    object RParen : Token()
}

private fun tokenize(expression: String): List<Token> {
    val tokens = mutableListOf<Token>()
    var i = 0
    while (i < expression.length) {
        val c = expression[i]
        when {
            c.isWhitespace() -> i++
            c == '(' -> { tokens.add(Token.LParen); i++ }
            c == ')' -> { tokens.add(Token.RParen); i++ }
            c == '+' || c == '-' || c == '*' || c == '/' -> { tokens.add(Token.Op(c)); i++ }
            c.isDigit() || c == '.' -> {
                val start = i
                while (i < expression.length && (expression[i].isDigit() || expression[i] == '.')) i++
                tokens.add(Token.Number(expression.substring(start, i).toDouble()))
            }
            else -> throw IllegalArgumentException("Unexpected character '$c' in '$expression'")
        }
    }
    return tokens
}

private class Parser(private val tokens: List<Token>) {
    private var pos = 0

    fun isAtEnd(): Boolean = pos >= tokens.size

    fun parseExpression(): Double {
        var value = parseTerm()
        while (currentSymbolIn('+', '-')) {
            val op = (tokens[pos] as Token.Op).symbol
            pos++
            val rhs = parseTerm()
            value = if (op == '+') value + rhs else value - rhs
        }
        return value
    }

    private fun parseTerm(): Double {
        var value = parseFactor()
        while (currentSymbolIn('*', '/')) {
            val op = (tokens[pos] as Token.Op).symbol
            pos++
            val rhs = parseFactor()
            value = if (op == '*') value * rhs else value / rhs
        }
        return value
    }

    private fun parseFactor(): Double {
        val token = tokens.getOrElse(pos) { throw IllegalArgumentException("Unexpected end of expression") }
        return when (token) {
            is Token.Number -> {
                pos++
                token.value
            }
            is Token.Op -> when (token.symbol) {
                '-' -> { pos++; -parseFactor() }
                '+' -> { pos++; parseFactor() }
                else -> throw IllegalArgumentException("Unexpected operator '${token.symbol}'")
            }
            Token.LParen -> {
                pos++
                val value = parseExpression()
                val closing = tokens.getOrElse(pos) { throw IllegalArgumentException("Expected ')'") }
                require(closing is Token.RParen) { "Expected ')'" }
                pos++
                value
            }
            Token.RParen -> throw IllegalArgumentException("Unexpected ')'")
        }
    }

    private fun currentSymbolIn(vararg symbols: Char): Boolean {
        val token = tokens.getOrNull(pos)
        return token is Token.Op && token.symbol in symbols
    }
}
