using System;
using System.Collections.Generic;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex083_MiddlewarePipelineTests
{
    [Fact]
    public void RunsMiddlewaresInRegistrationOrder()
    {
        var pipeline = new MiddlewarePipeline();
        pipeline
            .Use((ctx, next) => { ctx.Log.Add("first"); next(); })
            .Use((ctx, next) => { ctx.Log.Add("second"); next(); })
            .Use((ctx, next) => { ctx.Log.Add("third"); next(); });

        var context = new RequestContext();
        pipeline.Execute(context);

        Assert.Equal(new List<string> { "first", "second", "third" }, context.Log);
    }

    [Fact]
    public void ShortCircuitingMiddlewarePreventsLaterOnesFromRunning()
    {
        var pipeline = new MiddlewarePipeline();
        pipeline
            .Use((ctx, next) => { ctx.Log.Add("auth"); next(); })
            .Use((ctx, next) =>
            {
                ctx.Log.Add("guard");
                ctx.Handled = true;
                // Deliberately does NOT call next() -> chain stops here.
            })
            .Use((ctx, next) => { ctx.Log.Add("handler"); next(); });

        var context = new RequestContext();
        pipeline.Execute(context);

        Assert.Equal(new List<string> { "auth", "guard" }, context.Log);
        Assert.True(context.Handled);
        Assert.DoesNotContain("handler", context.Log);
    }

    [Fact]
    public void MiddlewareCanRunLogicAfterCallingNext()
    {
        var pipeline = new MiddlewarePipeline();
        pipeline
            .Use((ctx, next) =>
            {
                ctx.Log.Add("before-inner");
                next();
                ctx.Log.Add("after-inner");
            })
            .Use((ctx, next) => { ctx.Log.Add("inner"); next(); });

        var context = new RequestContext();
        pipeline.Execute(context);

        Assert.Equal(new List<string> { "before-inner", "inner", "after-inner" }, context.Log);
    }

    [Fact]
    public void UseReturnsSameInstanceForFluentChaining()
    {
        var pipeline = new MiddlewarePipeline();
        var returned = pipeline.Use((ctx, next) => next());

        Assert.Same(pipeline, returned);
    }

    [Fact]
    public void EmptyPipelineExecutesWithoutError()
    {
        var pipeline = new MiddlewarePipeline();
        var context = new RequestContext();

        pipeline.Execute(context);

        Assert.Empty(context.Log);
        Assert.False(context.Handled);
    }
}
