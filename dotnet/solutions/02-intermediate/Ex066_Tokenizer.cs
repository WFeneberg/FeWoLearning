using System;
using System.Collections.Generic;
using System.Text;

namespace FeWoLearning.Exercises.Intermediate;

// Exercise 066 — Tokenizer (reference solution).
public static class Tokenizer
{
    private const string Operators = "+-*/()";

    public static List<string> Tokenize(string expression)
    {
        var tokens = new List<string>();
        var i = 0;

        while (i < expression.Length)
        {
            var c = expression[i];

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            if (Operators.IndexOf(c) >= 0)
            {
                tokens.Add(c.ToString());
                i++;
                continue;
            }

            if (char.IsDigit(c) || c == '.')
            {
                var start = i;
                var sb = new StringBuilder();
                var sawDot = false;

                while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                {
                    if (expression[i] == '.')
                    {
                        if (sawDot)
                        {
                            throw new FormatException(
                                $"Invalid number '{sb}.' at position {start}: multiple decimal points.");
                        }

                        sawDot = true;
                    }

                    sb.Append(expression[i]);
                    i++;
                }

                var number = sb.ToString();
                if (number == "." || number.Length == 0)
                {
                    throw new FormatException($"Invalid number token at position {start}.");
                }

                tokens.Add(number);
                continue;
            }

            throw new FormatException($"Unexpected character '{c}' at position {i}.");
        }

        return tokens;
    }
}
