namespace FeWoLearning.Exercises.Expert;

// Exercise 097 — Roslyn-style syntax analyzer (expert).
// Goal:   Parse C# method declarations into a small syntax tree and walk each
//         method body's control-flow to report a diagnostic wherever a non-void
//         method has a path that falls through without a return/throw — the same
//         family of check Roslyn/csc performs for CS0161 ("not all code paths
//         return a value"). This exercise builds the tree and reachability
//         analysis by hand rather than depending on the Microsoft.CodeAnalysis
//         (Roslyn) package, which is not referenced by this project.
// Drills: recursive-descent parsing, syntax tree modeling, reachability
//         analysis, diagnostics reporting.
public static class RoslynAnalyzer
{
    // One reported issue: a method whose declared return type is not guaranteed
    // to be satisfied on every path through its body.
    public readonly record struct Diagnostic(string MethodName, int Line, string Message);

    // Analyzes the given C# source text and returns one diagnostic per method
    // whose non-void return type is not guaranteed on every code path.
    // Supports blocks, if/else (including "else if" chains), return, throw, and
    // simple semicolon-terminated statements; loops/switch/try are treated
    // conservatively as opaque, non-terminating statements.
    public static IReadOnlyList<Diagnostic> AnalyzeMissingReturns(string sourceCode)
        => throw new NotImplementedException();
}
