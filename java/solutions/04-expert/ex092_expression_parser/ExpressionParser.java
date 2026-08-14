package fewolearning.exercises.expert.ex092_expression_parser;

/*
Exercise 092 - Expression parser (reference solution).

Recursive-descent parser over the grammar:
  expression -> term (('+' | '-') term)*
  term       -> factor (('*' | '/') factor)*
  factor     -> ('-' | '+')? factor | '(' expression ')' | number
*/
public final class ExpressionParser {
    private ExpressionParser() {
    }

    public static double evaluate(String expression) {
        Parser parser = new Parser(expression);
        double result = parser.parseExpression();
        parser.expectEnd();
        return result;
    }

    private static final class Parser {
        private final String text;
        private int position;

        private Parser(String text) {
            this.text = text;
            this.position = 0;
        }

        private double parseExpression() {
            double value = parseTerm();
            while (true) {
                char operator = peek();
                if (operator == '+') {
                    position++;
                    value += parseTerm();
                } else if (operator == '-') {
                    position++;
                    value -= parseTerm();
                } else {
                    break;
                }
            }
            return value;
        }

        private double parseTerm() {
            double value = parseFactor();
            while (true) {
                char operator = peek();
                if (operator == '*') {
                    position++;
                    value *= parseFactor();
                } else if (operator == '/') {
                    position++;
                    value /= parseFactor();
                } else {
                    break;
                }
            }
            return value;
        }

        private double parseFactor() {
            char current = peek();
            if (current == '(') {
                position++;
                double value = parseExpression();
                if (peek() != ')') {
                    throw new IllegalArgumentException("Expected ')' at position " + position);
                }
                position++;
                return value;
            }
            if (current == '-') {
                position++;
                return -parseFactor();
            }
            if (current == '+') {
                position++;
                return parseFactor();
            }
            return parseNumber();
        }

        private double parseNumber() {
            int start = position;
            while (position < text.length() && (Character.isDigit(text.charAt(position)) || text.charAt(position) == '.')) {
                position++;
            }
            if (position == start) {
                throw new IllegalArgumentException("Expected a number at position " + position);
            }
            return Double.parseDouble(text.substring(start, position));
        }

        private char peek() {
            skipWhitespace();
            return position < text.length() ? text.charAt(position) : '\0';
        }

        private void skipWhitespace() {
            while (position < text.length() && Character.isWhitespace(text.charAt(position))) {
                position++;
            }
        }

        private void expectEnd() {
            skipWhitespace();
            if (position != text.length()) {
                throw new IllegalArgumentException("Unexpected trailing input at position " + position);
            }
        }
    }
}
