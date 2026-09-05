using Bunit;
using FeWoLearning.Blazor.Exercises.Expert;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Expert;

// There is no streaming here (README section 7). What is asserted is the whole
// component-side contract streaming SSR depends on: a first render before the data
// exists, a second one when it arrives, and a swap that replaces the subtree.
public class Ex100_StreamingSsrCapstoneTests : BunitContext
{
    private static TaskCompletionSource<IReadOnlyList<string>> Gate()
        => new(TaskCreationOptions.RunContinuationsAsynchronously);

    [Fact]
    public void The_First_Render_Happens_Before_The_Data_Does()
    {
        var gate = Gate();

        var cut = Render<Ex100_StreamingSsrCapstone>(p => p.Add(c => c.Load, () => gate.Task));

        Assert.Equal("loading…", cut.Find(".placeholder").TextContent);
        Assert.Empty(cut.FindAll(".items"));
        Assert.Equal("loading", cut.Find(".panel").GetAttribute("data-state"));
    }

    [Fact]
    public void The_Second_Render_Carries_The_Data()
    {
        var gate = Gate();
        var cut = Render<Ex100_StreamingSsrCapstone>(p => p.Add(c => c.Load, () => gate.Task));

        gate.SetResult(["alpha", "beta"]);

        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll(".items li").Count));
        Assert.Equal("alpha", cut.FindAll(".items li")[0].TextContent);
        Assert.Empty(cut.FindAll(".placeholder"));
        Assert.Equal("loaded", cut.Find(".panel").GetAttribute("data-state"));
    }

    // Ruling: the diffing half of the capstone. The panel is opened at the same
    // sequence number in both states, which on its own would keep one instance
    // across the switch (ex092) - the differing keys are what make it a replacement
    // instead, which is what streaming does to a subtree.
    [Fact]
    public void The_Panel_Is_Replaced_Rather_Than_Patched()
    {
        var gate = Gate();
        var cut = Render<Ex100_StreamingSsrCapstone>(p => p.Add(c => c.Load, () => gate.Task));
        var loading = cut.FindComponent<Ex100_StreamingSsrCapstone_Panel>().Instance;

        gate.SetResult(["alpha"]);
        cut.WaitForAssertion(() => Assert.NotEmpty(cut.FindAll(".items")));

        var loaded = cut.FindComponent<Ex100_StreamingSsrCapstone_Panel>().Instance;
        Assert.NotSame(loading, loaded);
    }

    // Metadata rather than behaviour: whether a host streams this component is the
    // host's business, and bUnit is not one. Same documented exception as ex069 -
    // this fact goes red on an assertion rather than on the exercise's own
    // NotImplementedException (README section 11).
    [Fact]
    public void The_Component_Is_Marked_For_Stream_Rendering()
    {
        Assert.NotNull(
            typeof(Ex100_StreamingSsrCapstone)
                .GetCustomAttributes(typeof(StreamRenderingAttribute), inherit: false)
                .FirstOrDefault());
    }

    [Fact]
    public void An_Already_Completed_Load_Goes_Straight_To_The_Data()
    {
        var cut = Render<Ex100_StreamingSsrCapstone>(p => p.Add(
            c => c.Load, () => Task.FromResult<IReadOnlyList<string>>(["only"])));

        Assert.Empty(cut.FindAll(".placeholder"));
        Assert.Equal("only", cut.Find(".items li").TextContent);
    }
}
