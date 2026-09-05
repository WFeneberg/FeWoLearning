using Bunit;
using FeWoLearning.Blazor.Exercises.Intermediate;
using FeWoLearning.Blazor.Support;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Intermediate;

public class Ex067_ErrorBoundaryLoggingHandlerTests : BunitContext
{
    // Registered over bUnit's own default, which is how "did you call base?" below
    // becomes observable at all: base.OnErrorAsync is what reaches this.
    private sealed class RecordingBoundaryLogger : IErrorBoundaryLogger
    {
        public List<Exception> Logged { get; } = [];

        public ValueTask LogErrorAsync(Exception exception)
        {
            Logged.Add(exception);
            return ValueTask.CompletedTask;
        }
    }

    private readonly ErrorLog _log = new();
    private readonly RecordingBoundaryLogger _frameworkLogger = new();

    public Ex067_ErrorBoundaryLoggingHandlerTests()
    {
        Services.AddSingleton(_log);
        Services.AddSingleton<IErrorBoundaryLogger>(_frameworkLogger);
    }

    // The "nothing logged yet" half is deliberately inside this fact rather than a
    // fact of its own: on the untouched stub the override never runs, so a fact that
    // only asserted an empty log would pass against it and prove nothing.
    [Fact]
    public void The_Override_Receives_The_Caught_Exception_And_Only_Then()
    {
        var cut = Render<Ex067_ErrorBoundaryLoggingHandler>(p => p.Add(c => c.Explode, false));
        Assert.Empty(_log.Entries);

        cut.Render(p => p.Add(c => c.Explode, true));

        var logged = Assert.Single(_log.Entries);
        Assert.Equal(ExplodingChild.Message, logged.Message);
    }

    // Non-vacuity for base.OnErrorAsync: an override that logs and stops passes the
    // fact above and leaves the framework's own logger untouched.
    [Fact]
    public void The_Override_Still_Lets_The_Framework_Log_It()
    {
        var cut = Render<Ex067_ErrorBoundaryLoggingHandler>(p => p.Add(c => c.Explode, false));

        cut.Render(p => p.Add(c => c.Explode, true));

        var logged = Assert.Single(_frameworkLogger.Logged);
        Assert.Equal(ExplodingChild.Message, logged.Message);
    }

    // Handling an error must not be a one-shot: after recovering, the next one is
    // logged as well.
    [Fact]
    public void A_Second_Error_After_Recovery_Is_Logged_Again()
    {
        var cut = Render<Ex067_ErrorBoundaryLoggingHandler>(p => p.Add(c => c.Explode, true));
        cut.Render(p => p.Add(c => c.Explode, false));
        cut.Find("#recover").Click();
        cut.WaitForAssertion(() => Assert.Equal("ok", cut.Find("#child").TextContent));

        cut.Render(p => p.Add(c => c.Explode, true));

        Assert.Equal(2, _log.Entries.Count);
    }
}
