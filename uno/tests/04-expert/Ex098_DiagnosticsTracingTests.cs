using FeWoLearning.Uno.Exercises.Expert;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Expert;

public class Ex098_DiagnosticsTracingTests : UnoTestContext
{
    private static Ex098_TracedPanel Panel(Ex098_Tracer tracer, string name = "panel")
    {
        var panel = new Ex098_TracedPanel { Tracer = tracer, TraceName = name };
        panel.Children.Add(new Border { Width = 10, Height = 10 });
        return panel;
    }

    [Fact]
    public void A_Scope_Records_Itself()
    {
        var tracer = new Ex098_Tracer();

        using (tracer.Scope("measure", "panel"))
        {
        }

        Assert.Equal(new Ex098_TraceRecord("measure", "panel", 0), Assert.Single(tracer.Records));
    }

    [Fact]
    public void Nested_Scopes_Record_Their_Depth()
    {
        var tracer = new Ex098_Tracer();

        using (tracer.Scope("measure", "outer"))
        {
            using (tracer.Scope("measure", "inner"))
            {
            }
        }

        Assert.Equal([0, 1], tracer.Records.Select(record => record.Depth));
    }

    [Fact]
    public void A_Closed_Scope_Releases_Its_Depth()
    {
        var tracer = new Ex098_Tracer();

        using (tracer.Scope("measure", "first"))
        {
        }

        using (tracer.Scope("measure", "second"))
        {
        }

        // Both at depth 0: a depth that only ever grows makes the trace unreadable after
        // the first pass.
        Assert.Equal([0, 0], tracer.Records.Select(record => record.Depth));
    }

    [Fact]
    public void Tracing_Off_Records_Nothing()
    {
        var tracer = new Ex098_Tracer { IsEnabled = false };

        using (tracer.Scope("measure", "panel"))
        {
        }

        Assert.Empty(tracer.Records);
    }

    [Fact]
    public void Tracing_Off_Allocates_Nothing_Per_Scope()
    {
        var tracer = new Ex098_Tracer { IsEnabled = false };

        var first = tracer.Scope("measure", "a");
        var second = tracer.Scope("measure", "b");

        // The same shared token. A new object per scope is exactly the cost that gets
        // tracing switched off in production and then never switched back on.
        Assert.Same(first, second);
        Assert.Same(Ex098_Tracer.Disabled, first);
    }

    [Fact]
    public void Counting_Filters_By_Category()
    {
        var tracer = new Ex098_Tracer();
        tracer.Scope("measure", "a").Dispose();
        tracer.Scope("arrange", "a").Dispose();
        tracer.Scope("measure", "b").Dispose();

        Assert.Equal(2, tracer.CountOf("measure"));
        Assert.Equal(1, tracer.CountOf("arrange"));
        Assert.Equal(0, tracer.CountOf("render"));
    }

    [Fact]
    public void Clearing_Forgets_The_Records()
    {
        var tracer = new Ex098_Tracer();
        tracer.Scope("measure", "a").Dispose();

        tracer.Clear();

        Assert.Empty(tracer.Records);
    }

    [Fact]
    public void A_Panel_Traces_Both_Passes()
    {
        var tracer = new Ex098_Tracer();

        Layout(Panel(tracer), width: 200, height: 200);

        Assert.Equal(1, tracer.CountOf("measure"));
        Assert.Equal(1, tracer.CountOf("arrange"));
    }

    [Fact]
    public void A_Panel_Traces_Under_Its_Own_Name()
    {
        var tracer = new Ex098_Tracer();

        Layout(Panel(tracer, "card"), width: 200, height: 200);

        Assert.All(tracer.Records, record => Assert.Equal("card", record.Subject));
    }

    [Fact]
    public void Nested_Panels_Show_The_Nesting()
    {
        var tracer = new Ex098_Tracer();
        var inner = Panel(tracer, "inner");
        var outer = new Ex098_TracedPanel { Tracer = tracer, TraceName = "outer" };
        outer.Children.Add(inner);

        Layout(outer, width: 200, height: 200);

        var measures = tracer.Records.Where(record => record.Category == "measure").ToList();

        // This is the answer to "why did this measure four times": the sequence, with the
        // nesting, rather than any single breakpoint.
        Assert.Equal([("outer", 0), ("inner", 1)], measures.Select(record => (record.Subject, record.Depth)));
    }

    [Fact]
    public void A_Panel_Without_A_Tracer_Still_Lays_Out()
    {
        var panel = new Ex098_TracedPanel();
        panel.Children.Add(new Border { Width = 10, Height = 10 });

        Layout(panel, width: 200, height: 200);

        // Tracing off is the normal case, and a null tracer is not an error.
        Assert.Equal(10, panel.DesiredSize.Height, 1);
    }
}
