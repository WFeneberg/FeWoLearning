using System.Linq;
using System.Threading.Tasks;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Expert;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Tests.Expert;

public class Ex100_EndToEndMvvmFeatureTests
{
    private static Ex100_EndToEndMvvmFeature WithName(string name) =>
        new() { Name = name };

    private static async Task<bool> CanSave(Ex100_EndToEndMvvmFeature feature) =>
        await feature.Save.CanExecute.FirstAsync();

    // Awaiting the execution, the way ex041 established for this track. Two measured
    // dead ends before settling on it: draining the dispatcher after the fact is not
    // enough, and waiting on IsExecuting does not work either, because it is a
    // behaviour whose CURRENT false comes back immediately - before the command has
    // even started. Deciding the gateway's outcome up front and awaiting the
    // execution needs no sleep and no pumping.
    private static Task Save(Ex100_EndToEndMvvmFeature feature) =>
        feature.Save.Execute().ToTask(TestContext.Current.CancellationToken);

    // Validation, on its own first: one message, the right one, and nothing at all
    // when the form is fine.
    [AvaloniaTheory]
    [InlineData("", Ex100_Messages.Required)]
    [InlineData("a perfectly reasonable but far too long name", Ex100_Messages.TooLong)]
    public void An_Invalid_Name_Reports_Exactly_One_Message(string name, string expected)
    {
        var feature = WithName(name);

        Assert.True(feature.HasErrors);
        Assert.Equal([expected], feature.GetErrors(nameof(Ex100_EndToEndMvvmFeature.Name)).Cast<string>());
    }

    [AvaloniaFact]
    public void A_Valid_Name_Reports_Nothing()
    {
        var feature = WithName("Ada");

        Assert.False(feature.HasErrors);
        Assert.Empty(feature.GetErrors(nameof(Ex100_EndToEndMvvmFeature.Name)).Cast<string>());
    }

    [AvaloniaFact]
    public void Errors_Are_Reported_Per_Property()
    {
        Assert.Empty(WithName("").GetErrors("SomethingElse").Cast<string>());
    }

    // The first seam: the command must report itself unavailable, rather than being
    // invokable and then declining. An implementation that checks HasErrors inside
    // the task body passes everything else in this file and fails here.
    [AvaloniaFact]
    public async Task Save_Is_Not_Executable_While_The_Form_Is_Invalid()
    {
        Assert.False(await CanSave(WithName("")));
        Assert.True(await CanSave(WithName("Ada")));
    }

    // ...and it follows the field, so fixing the form enables it without anything
    // else being poked.
    [AvaloniaFact]
    public async Task Making_The_Form_Valid_Enables_Save()
    {
        var feature = WithName("");

        feature.Name = "Grace";

        Assert.True(await CanSave(feature));
    }

    [AvaloniaFact]
    public async Task Breaking_The_Form_Again_Disables_Save()
    {
        var feature = WithName("Grace");

        feature.Name = string.Empty;

        Assert.False(await CanSave(feature));
    }

    [AvaloniaFact]
    public async Task Saving_Hands_The_Current_Name_To_The_Gateway()
    {
        var feature = WithName("Ada");

        await Save(feature);

        Assert.Equal(["Ada"], feature.Gateway.Requests);
    }

    // The second seam, and the one that matters most: while the save is still in
    // flight nobody has moved. The gateway stalls, so the execution genuinely has
    // not finished when this asserts.
    [AvaloniaFact]
    public void Nobody_Navigates_While_The_Save_Is_In_Flight()
    {
        var feature = WithName("Ada");
        feature.Gateway.Stall = true;

        _ = feature.Save.Execute().Subscribe(_ => { }, _ => { });

        Assert.Equal(["Ada"], feature.Gateway.Requests);
        Assert.Empty(feature.Router.NavigationStack);
    }

    [AvaloniaFact]
    public async Task A_Successful_Save_Navigates_Onwards()
    {
        var feature = WithName("Ada");

        await Save(feature);

        Assert.Single(feature.Router.NavigationStack);
        Assert.Equal("done", feature.Router.NavigationStack[0].UrlPathSegment);
        Assert.Empty(feature.SurfacedErrors);
    }

    // The failing half of the same seam: an implementation that navigates when the
    // command is INVOKED rather than when it succeeds passes the test above and
    // strands the user here.
    [AvaloniaFact]
    public async Task A_Failed_Save_Moves_Nobody()
    {
        var feature = WithName("Ada");
        feature.Gateway.FailWith = "the server said no";

        await Record.ExceptionAsync(() => Save(feature));

        Assert.Empty(feature.Router.NavigationStack);
    }

    // The third seam: the failure has to be visible. Swallowing it is worse than
    // crashing, because the form then looks saved. Note that the execution itself
    // must still fault - a solution that catches inside the task body and returns
    // normally leaves this exception null.
    [AvaloniaFact]
    public async Task A_Failed_Save_Faults_And_Surfaces_Its_Message()
    {
        var feature = WithName("Ada");
        feature.Gateway.FailWith = "the server said no";

        var thrown = await Record.ExceptionAsync(() => Save(feature));

        Assert.NotNull(thrown);
        Assert.Equal("the server said no", thrown!.Message);
        Assert.Equal(["the server said no"], feature.SurfacedErrors);
    }

    // And the feature survives it: a second attempt goes through, so a failure is
    // recoverable rather than terminal.
    [AvaloniaFact]
    public async Task A_Retry_After_A_Failure_Still_Works()
    {
        var feature = WithName("Ada");
        feature.Gateway.FailWith = "the server said no";

        await Record.ExceptionAsync(() => Save(feature));

        feature.Gateway.FailWith = null;
        await Save(feature);

        Assert.Equal(["Ada", "Ada"], feature.Gateway.Requests);
        Assert.Single(feature.SurfacedErrors);
        Assert.Single(feature.Router.NavigationStack);
    }
}
