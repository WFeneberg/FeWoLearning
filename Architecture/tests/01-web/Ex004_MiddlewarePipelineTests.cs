using FeWoLearning.Architecture.Exercises.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex004_MiddlewarePipelineTests
{
    private static readonly IServiceProvider Services = new ServiceCollection().BuildServiceProvider();

    private static async Task<(List<string> Log, int Status)> Run(bool withShortCircuitHeader)
    {
        var log = new List<string>();
        var pipeline = Ex004_MiddlewarePipeline.Build(Services, log);

        var context = new DefaultHttpContext();
        if (withShortCircuitHeader)
            context.Request.Headers[Ex004_MiddlewarePipeline.ShortCircuitHeader] = "yes";

        await pipeline(context);

        return (log, context.Response.StatusCode);
    }

    [Fact]
    public async Task Ordering_The_Pipeline_Runs_In_Then_Out()
    {
        // Sequence equality, not Contains: the unwind half is exactly what a naive
        // "call every middleware in turn" implementation gets wrong, and Contains
        // would not notice.
        var (log, _) = await Run(withShortCircuitHeader: false);

        Assert.Equal(
            ["outer:in", "gate:in", "terminal", "gate:out", "outer:out"],
            log);
    }

    [Fact]
    public async Task Terminal_Middleware_Sets_The_Success_Status()
    {
        // 202, not 200, and that is not cosmetic: DefaultHttpContext starts life with
        // StatusCode 200, so a fact asserting 200 here passes against a pipeline that
        // does nothing at all. Measured - the degenerate probe for this batch caught
        // exactly that. Any fact about a response status has to assert a value the
        // default is not.
        var (_, status) = await Run(withShortCircuitHeader: false);

        Assert.Equal(StatusCodes.Status202Accepted, status);
    }

    [Fact]
    public async Task ShortCircuit_Stops_The_Pipeline_Before_The_Terminal_Middleware()
    {
        var (log, status) = await Run(withShortCircuitHeader: true);

        Assert.DoesNotContain("terminal", log);
        Assert.Equal(StatusCodes.Status403Forbidden, status);
    }

    [Fact]
    public async Task Adversarial_A_ShortCircuit_Still_Unwinds_The_Middleware_Above_It()
    {
        // The fact that separates a correct short-circuit from an early `return` in the
        // wrong place, or from throwing. "outer:out" is where a real pipeline commits a
        // transaction, stops a timer, writes an access log. Asserting only that the
        // terminal middleware was skipped would pass either way.
        var (log, _) = await Run(withShortCircuitHeader: true);

        Assert.Equal(["outer:in", "gate:short-circuit", "outer:out"], log);
    }

    [Fact]
    public async Task The_Pipeline_Is_Reusable_Across_Requests()
    {
        // A RequestDelegate is built once and serves every request. An implementation
        // that captured per-request state in the closure passes every fact above and
        // breaks here.
        var log = new List<string>();
        var pipeline = Ex004_MiddlewarePipeline.Build(Services, log);

        await pipeline(new DefaultHttpContext());
        await pipeline(new DefaultHttpContext());

        Assert.Equal(
            ["outer:in", "gate:in", "terminal", "gate:out", "outer:out",
             "outer:in", "gate:in", "terminal", "gate:out", "outer:out"],
            log);
    }
}
