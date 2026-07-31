using System.Text.RegularExpressions;

namespace FeWoLearning.Exercises.Expert;

// Exercise 097 — Roslyn-style syntax analyzer (reference solution).
//
// This is a hand-rolled analogue of a Roslyn CSharpSyntaxWalker: it lexes just
// enough of a method body to build a tiny statement tree (blocks, if/else,
// return, throw, and opaque "other" statements), then walks that tree with a
// recursive reachability check equivalent to the compiler's CS0161 analysis
// ("not all code paths return a value"). Loops/switch/try are recognized only
// well enough not to break parsing; they are treated conservatively as opaque
// statements that never guarantee a return.
public static class RoslynAnalyzer
{
    public readonly record struct Diagnostic(string MethodName, int Line, string Message);

    private abstract record StatementNode;
    private sealed record BlockNode(IReadOnlyList<StatementNode> Statements) : StatementNode;
    private sealed record IfStatementNode(BlockNode Then, BlockNode? Else) : StatementNode;
    private sealed record ReturnStatementNode : StatementNode;
    private sealed record ThrowStatementNode : StatementNode;
    private sealed record OtherStatementNode : StatementNode;

    // Matches a method signature: one or more modifiers, a return type, a name,
    // a parameter list, and the opening brace of a block body. Constructors
    // (no separate return-type token) and expression-bodied / abstract members
    // (no opening brace) do not match and are left unanalyzed.
    private static readonly Regex MethodSignatureRegex = new(
        @"(?:(?:public|private|protected|internal|static|virtual|override|sealed|async|extern|unsafe|new|partial)\s+)+([\w<>\[\],\.\?]+)\s+(\w+)\s*\(([^)]*)\)\s*\{",
        RegexOptions.Compiled);

    public static IReadOnlyList<Diagnostic> AnalyzeMissingReturns(string sourceCode)
    {
        ArgumentNullException.ThrowIfNull(sourceCode);

        var diagnostics = new List<Diagnostic>();
        foreach (var method in FindMethods(sourceCode))
        {
            if (method.ReturnType == "void")
                continue;

            if (!AlwaysReturns(method.Body.Statements))
            {
                diagnostics.Add(new Diagnostic(
                    method.Name,
                    method.Line,
                    $"Method '{method.Name}' does not return a value on all code paths."));
            }
        }

        return diagnostics;
    }

    private static IEnumerable<(string Name, string ReturnType, int Line, BlockNode Body)> FindMethods(string source)
    {
        foreach (Match match in MethodSignatureRegex.Matches(source))
        {
            var returnType = match.Groups[1].Value;
            var name = match.Groups[2].Value;
            var line = CountLine(source, match.Index);

            var bracePos = match.Index + match.Length - 1; // position of the '{'
            var body = ParseBlock(source, ref bracePos);

            yield return (name, returnType, line, body);
        }
    }

    private static int CountLine(string source, int index)
    {
        var line = 1;
        for (var i = 0; i < index; i++)
        {
            if (source[i] == '\n')
                line++;
        }
        return line;
    }

    // --- Control-flow reachability -----------------------------------------

    private static bool AlwaysReturns(IReadOnlyList<StatementNode> statements)
    {
        foreach (var statement in statements)
        {
            if (StatementAlwaysReturns(statement))
                return true;
        }
        return false;
    }

    private static bool StatementAlwaysReturns(StatementNode statement) => statement switch
    {
        ReturnStatementNode => true,
        ThrowStatementNode => true,
        BlockNode block => AlwaysReturns(block.Statements),
        IfStatementNode ifStatement => ifStatement.Else is not null
            && AlwaysReturns(ifStatement.Then.Statements)
            && AlwaysReturns(ifStatement.Else.Statements),
        _ => false,
    };

    // --- Minimal recursive-descent statement parser -------------------------

    private static BlockNode ParseBlock(string src, ref int pos)
    {
        // Assumes src[pos] == '{'.
        pos++;
        var statements = new List<StatementNode>();
        SkipTrivia(src, ref pos);
        while (pos < src.Length && src[pos] != '}')
        {
            statements.Add(ParseStatement(src, ref pos));
            SkipTrivia(src, ref pos);
        }
        if (pos < src.Length)
            pos++; // consume '}'
        return new BlockNode(statements);
    }

    private static BlockNode ParseEmbeddedStatement(string src, ref int pos)
    {
        SkipTrivia(src, ref pos);
        if (pos < src.Length && src[pos] == '{')
            return ParseBlock(src, ref pos);

        var statement = ParseStatement(src, ref pos);
        return new BlockNode(new List<StatementNode> { statement });
    }

    private static StatementNode ParseStatement(string src, ref int pos)
    {
        SkipTrivia(src, ref pos);

        if (MatchKeyword(src, pos, "if"))
        {
            pos += 2;
            SkipTrivia(src, ref pos);
            SkipParenGroup(src, ref pos);
            SkipTrivia(src, ref pos);
            var thenBlock = ParseEmbeddedStatement(src, ref pos);

            SkipTrivia(src, ref pos);
            BlockNode? elseBlock = null;
            if (MatchKeyword(src, pos, "else"))
            {
                pos += 4;
                SkipTrivia(src, ref pos);
                elseBlock = ParseEmbeddedStatement(src, ref pos);
            }

            return new IfStatementNode(thenBlock, elseBlock);
        }

        if (MatchKeyword(src, pos, "return"))
        {
            SkipStatementTail(src, ref pos);
            return new ReturnStatementNode();
        }

        if (MatchKeyword(src, pos, "throw"))
        {
            SkipStatementTail(src, ref pos);
            return new ThrowStatementNode();
        }

        if (pos < src.Length && src[pos] == '{')
            return ParseBlock(src, ref pos);

        // Any other simple statement, or a compound construct this lightweight
        // analyzer does not model in detail (for/while/switch/try/using/...).
        // Conservatively assumed not to guarantee a return.
        SkipOpaqueStatement(src, ref pos);
        return new OtherStatementNode();
    }

    // --- Low-level scanning helpers -----------------------------------------

    private static void SkipTrivia(string src, ref int pos)
    {
        while (pos < src.Length)
        {
            var c = src[pos];
            if (char.IsWhiteSpace(c))
            {
                pos++;
            }
            else if (c == '/' && pos + 1 < src.Length && src[pos + 1] == '/')
            {
                while (pos < src.Length && src[pos] != '\n')
                    pos++;
            }
            else if (c == '/' && pos + 1 < src.Length && src[pos + 1] == '*')
            {
                pos += 2;
                while (pos + 1 < src.Length && !(src[pos] == '*' && src[pos + 1] == '/'))
                    pos++;
                pos = Math.Min(pos + 2, src.Length);
            }
            else
            {
                break;
            }
        }
    }

    private static bool MatchKeyword(string src, int pos, string keyword)
    {
        if (pos + keyword.Length > src.Length)
            return false;
        if (string.CompareOrdinal(src, pos, keyword, 0, keyword.Length) != 0)
            return false;
        var after = pos + keyword.Length;
        return after >= src.Length || !(char.IsLetterOrDigit(src[after]) || src[after] == '_');
    }

    private static void SkipParenGroup(string src, ref int pos)
    {
        if (pos >= src.Length || src[pos] != '(')
            return;
        var depth = 0;
        while (pos < src.Length)
        {
            if (src[pos] == '(') depth++;
            else if (src[pos] == ')') depth--;
            pos++;
            if (depth == 0)
                return;
        }
    }

    private static void SkipStringOrChar(string src, ref int pos)
    {
        var quote = src[pos];
        pos++;
        while (pos < src.Length && src[pos] != quote)
        {
            if (src[pos] == '\\')
                pos++;
            pos++;
        }
        if (pos < src.Length)
            pos++; // consume closing quote
    }

    // Consumes a return/throw expression up to its terminating top-level ';',
    // treating (), [], {} as nesting so object/collection initializers and
    // string literals with those characters don't confuse the scan.
    private static void SkipStatementTail(string src, ref int pos)
    {
        var depth = 0;
        while (pos < src.Length)
        {
            var c = src[pos];
            if (c is '"' or '\'') { SkipStringOrChar(src, ref pos); continue; }
            if (c is '(' or '[' or '{') { depth++; pos++; continue; }
            if (c is ')' or ']' or '}') { depth--; pos++; continue; }
            if (c == ';' && depth == 0) { pos++; return; }
            pos++;
        }
    }

    // Consumes an unmodeled statement: either a simple "...;" statement or a
    // brace-delimited construct (optionally chained, e.g. try/catch/finally).
    private static void SkipOpaqueStatement(string src, ref int pos)
    {
        var parenDepth = 0;
        while (pos < src.Length)
        {
            var c = src[pos];
            if (c is '"' or '\'') { SkipStringOrChar(src, ref pos); continue; }
            if (c == '(') { parenDepth++; pos++; continue; }
            if (c == ')') { parenDepth--; pos++; continue; }
            if (parenDepth == 0 && c == '{')
            {
                ParseBlock(src, ref pos); // consume nested block, discard structure
                SkipTrivia(src, ref pos);
                if (MatchKeyword(src, pos, "catch") || MatchKeyword(src, pos, "finally") || MatchKeyword(src, pos, "else"))
                    continue; // consume the following clause too
                return;
            }
            if (parenDepth == 0 && c == ';') { pos++; return; }
            pos++;
        }
    }
}
