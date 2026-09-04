// Exercise 098 - Diagnostics Tracing (expert).
// Goal:   Make layout work observable without a debugger and without a print statement.
// Drills: a structured sink instead of strings, a scope that records its own nesting, and
//         tracing that costs nothing when nobody is listening.
// Passes: dotnet test --filter FullyQualifiedName~Ex098_
//
// "Why did this measure four times" is unanswerable from a breakpoint - the interesting
// part is the sequence, not any single stop. A structured trace answers it: one record per
// pass, with the element and the nesting depth, queryable afterwards.
//
// The cost matters as much as the content. A tracer that formats a string per pass is one
// nobody dares leave enabled, so the check comes first and the record is a value type's
// worth of fields rather than a message.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace FeWoLearning.Uno.Exercises.Expert;

/// <summary>One thing that happened.</summary>
/// <param name="Category">What kind of work - "measure", "arrange".</param>
/// <param name="Subject">Which element, by name.</param>
/// <param name="Depth">How deeply nested the scope was, starting at 0.</param>
public sealed record Ex098_TraceRecord(string Category, string Subject, int Depth);

/// <summary>Collects trace records, and can be switched off.</summary>
public sealed class Ex098_Tracer
{
    private readonly List<Ex098_TraceRecord> _records = [];
    private int _depth;

    /// <summary>Whether anything is being recorded.</summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>What has been recorded, in order.</summary>
    public IReadOnlyList<Ex098_TraceRecord> Records => _records;

    /// <summary>
    /// Opens a scope: records it at the current depth and returns something whose disposal
    /// closes it. When tracing is off, records nothing and allocates nothing worth counting.
    /// </summary>
    public IDisposable Scope(string category, string subject)
    {
        if (!IsEnabled)
        {
            // The shared token. A new object per scope is exactly the cost that gets
            // tracing switched off in production and then never switched back on.
            return Disabled;
        }

        _records.Add(new Ex098_TraceRecord(category, subject, _depth));
        _depth++;

        return new OpenScope(this);
    }

    private sealed class OpenScope(Ex098_Tracer tracer) : IDisposable
    {
        private bool _closed;

        public void Dispose()
        {
            if (_closed)
            {
                return;
            }

            _closed = true;
            tracer._depth--;
        }
    }

    /// <summary>Forgets everything recorded so far.</summary>
    public void Clear() => _records.Clear();

    /// <summary>How many records there are of one category.</summary>
    public int CountOf(string category) => _records.Count(record => record.Category == category);

    /// <summary>The do-nothing token used while tracing is off.</summary>
    public static IDisposable Disabled { get; } = new NullScope();

    private sealed class NullScope : IDisposable
    {
        public void Dispose()
        {
        }
    }
}

/// <summary>
/// A panel that traces its own passes, so a test can see the sequence a real app would log.
/// </summary>
public partial class Ex098_TracedPanel : Panel
{
    /// <summary>Where this panel reports to.</summary>
    public Ex098_Tracer? Tracer { get; set; }

    /// <summary>What this panel calls itself in the trace.</summary>
    public string TraceName { get; set; } = "panel";

    protected override Size MeasureOverride(Size availableSize)
    {
        // A null tracer is the normal case, so the scope is optional rather than the
        // tracer being mandatory.
        using var scope = Tracer?.Scope("measure", TraceName) ?? Ex098_Tracer.Disabled;

        var width = 0d;
        var height = 0d;

        foreach (var child in Children)
        {
            child.Measure(availableSize);
            width = Math.Max(width, child.DesiredSize.Width);
            height += child.DesiredSize.Height;
        }

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        using var scope = Tracer?.Scope("arrange", TraceName) ?? Ex098_Tracer.Disabled;

        var y = 0d;

        foreach (var child in Children)
        {
            child.Arrange(new Rect(0, y, finalSize.Width, child.DesiredSize.Height));
            y += child.DesiredSize.Height;
        }

        return finalSize;
    }
}
