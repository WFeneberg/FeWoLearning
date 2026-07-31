namespace FeWoLearning.Exercises.Expert;

// Exercise 098 — Tiny DSL interpreter (reference solution).
// A small recursive-descent parser/evaluator for arithmetic expressions.
//
// Grammar (highest precedence last to bind tightest):
//   expression := term (('+' | '-') term)*
//   term       := unary (('*' | '/') unary)*
//   unary      := '-' unary | power
//   power      := primary ('^' unary)?              // right-associative
//   primary    := number | identifier | '(' expression ')'
public static class TinyDslInterpreter
{
    public static double Evaluate(string expression)
        => Evaluate(expression, new Dictionary<string, double>());

    public static double Evaluate(string expression, IReadOnlyDictionary<string, double> variables)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(variables);

        var tokens = Tokenize(expression);
        var parser = new Parser(tokens, variables);
        var result = parser.ParseExpression();
        parser.ExpectEnd();
        return result;
    }

    // ---- Tokenizer ------------------------------------------------------

    private enum TokenKind
    {
        Number,
        Identifier,
        Plus,
        Minus,
        Star,
        Slash,
        Caret,
        LParen,
        RParen,
        End
    }

    private readonly record struct Token(TokenKind Kind, string Text, double Number = 0);

    private static List<Token> Tokenize(string expression)
    {
        var tokens = new List<Token>();
        var i = 0;
        while (i < expression.Length)
        {
            var c = expression[i];
            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            switch (c)
            {
                case '+': tokens.Add(new Token(TokenKind.Plus, "+")); i++; continue;
                case '-': tokens.Add(new Token(TokenKind.Minus, "-")); i++; continue;
                case '*': tokens.Add(new Token(TokenKind.Star, "*")); i++; continue;
                case '/': tokens.Add(new Token(TokenKind.Slash, "/")); i++; continue;
                case '^': tokens.Add(new Token(TokenKind.Caret, "^")); i++; continue;
                case '(': tokens.Add(new Token(TokenKind.LParen, "(")); i++; continue;
                case ')': tokens.Add(new Token(TokenKind.RParen, ")")); i++; continue;
            }

            if (char.IsDigit(c) || c == '.')
            {
                var start = i;
                var sawDot = false;
                while (i < expression.Length && (char.IsDigit(expression[i]) || (expression[i] == '.' && !sawDot)))
                {
                    if (expression[i] == '.') sawDot = true;
                    i++;
                }
                var text = expression[start..i];
                if (!double.TryParse(text, System.Globalization.CultureInfo.InvariantCulture, out var value))
                    throw new FormatException($"Invalid number literal '{text}'.");
                tokens.Add(new Token(TokenKind.Number, text, value));
                continue;
            }

            if (char.IsLetter(c) || c == '_')
            {
                var start = i;
                while (i < expression.Length && (char.IsLetterOrDigit(expression[i]) || expression[i] == '_'))
                    i++;
                tokens.Add(new Token(TokenKind.Identifier, expression[start..i]));
                continue;
            }

            throw new FormatException($"Unexpected character '{c}' at position {i}.");
        }

        tokens.Add(new Token(TokenKind.End, ""));
        return tokens;
    }

    // ---- Parser / evaluator ---------------------------------------------

    private sealed class Parser
    {
        private readonly List<Token> _tokens;
        private readonly IReadOnlyDictionary<string, double> _variables;
        private int _pos;

        public Parser(List<Token> tokens, IReadOnlyDictionary<string, double> variables)
        {
            _tokens = tokens;
            _variables = variables;
        }

        private Token Current => _tokens[_pos];

        public void ExpectEnd()
        {
            if (Current.Kind != TokenKind.End)
                throw new FormatException($"Unexpected trailing token '{Current.Text}'.");
        }

        public double ParseExpression()
        {
            var value = ParseTerm();
            while (Current.Kind is TokenKind.Plus or TokenKind.Minus)
            {
                var isPlus = Current.Kind == TokenKind.Plus;
                _pos++;
                var rhs = ParseTerm();
                value = isPlus ? value + rhs : value - rhs;
            }
            return value;
        }

        private double ParseTerm()
        {
            var value = ParseUnary();
            while (Current.Kind is TokenKind.Star or TokenKind.Slash)
            {
                var isMul = Current.Kind == TokenKind.Star;
                _pos++;
                var rhs = ParseUnary();
                if (!isMul && rhs == 0)
                    throw new DivideByZeroException("Division by zero in expression.");
                value = isMul ? value * rhs : value / rhs;
            }
            return value;
        }

        private double ParseUnary()
        {
            if (Current.Kind == TokenKind.Minus)
            {
                _pos++;
                return -ParseUnary();
            }
            if (Current.Kind == TokenKind.Plus)
            {
                _pos++;
                return ParseUnary();
            }
            return ParsePower();
        }

        private double ParsePower()
        {
            var value = ParsePrimary();
            if (Current.Kind == TokenKind.Caret)
            {
                _pos++;
                var exponent = ParseUnary(); // right-associative, allows "2 ^ -1"
                value = Math.Pow(value, exponent);
            }
            return value;
        }

        private double ParsePrimary()
        {
            switch (Current.Kind)
            {
                case TokenKind.Number:
                {
                    var v = Current.Number;
                    _pos++;
                    return v;
                }
                case TokenKind.Identifier:
                {
                    var name = Current.Text;
                    _pos++;
                    if (!_variables.TryGetValue(name, out var v))
                        throw new FormatException($"Unknown identifier '{name}'.");
                    return v;
                }
                case TokenKind.LParen:
                {
                    _pos++;
                    var v = ParseExpression();
                    if (Current.Kind != TokenKind.RParen)
                        throw new FormatException("Expected ')'.");
                    _pos++;
                    return v;
                }
                default:
                    throw new FormatException($"Unexpected token '{Current.Text}'.");
            }
        }
    }
}
