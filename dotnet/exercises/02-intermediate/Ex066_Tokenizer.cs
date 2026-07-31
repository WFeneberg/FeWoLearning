using System.Collections.Generic;

namespace FeWoLearning.Exercises.Intermediate;

// Exercise 066 — Tokenizer (intermediate).
// Goal:   Split an arithmetic expression string into a flat list of tokens:
//         numbers (integers or decimals, e.g. "12" or "3.5"), the operators
//         + - * /, and the parentheses ( and ). Whitespace between tokens is
//         ignored. Any other character is invalid input.
// Drills: string parsing/tokenizing, character classification, StringBuilder,
//         input validation via exceptions.
public static class Tokenizer
{
    // Returns the tokens of `expression` in order, e.g. "3 + 4 * 2" ->
    // ["3", "+", "4", "*", "2"]. Throws System.FormatException if the
    // expression contains a character that cannot start or continue a
    // valid token (i.e. anything other than a digit, '.', one of +-*/(),
    // or whitespace).
    public static List<string> Tokenize(string expression) => throw new System.NotImplementedException();
}
