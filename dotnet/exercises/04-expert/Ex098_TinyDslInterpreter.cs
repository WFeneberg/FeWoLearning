namespace FeWoLearning.Exercises.Expert;

// Exercise 098 — Tiny DSL interpreter (expert).
// Goal:   Parse and evaluate arithmetic expression strings with correct operator
//         precedence and associativity: unary minus, '^' (right-assoc), '*', '/'
//         (left-assoc), '+', '-' (left-assoc), parentheses, and named variables.
// Drills: tokenizing, recursive-descent parsing, precedence climbing, evaluation.
public static class TinyDslInterpreter
{
    // Evaluates an arithmetic expression such as "2 + 3 * (4 - 1) ^ 2".
    public static double Evaluate(string expression) => throw new NotImplementedException();

    // Evaluates an expression, resolving identifiers against the supplied variables.
    public static double Evaluate(string expression, IReadOnlyDictionary<string, double> variables)
        => throw new NotImplementedException();
}
