using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex075_CancellableOperationTests
{
    [Fact]
    public async Task RunsToCompletion_WhenNeverCancelled()
    {
        var seen = new List<int>();

        var result = await CancellableOperation.RunAsync(5, i => seen.Add(i), CancellationToken.None);

        Assert.Equal(5, result);
        Assert.Equal(new[] { 0, 1, 2, 3, 4 }, seen);
    }

    [Fact]
    public async Task StopsBeforeCompletion_WhenCancelledMidway()
    {
        using var cts = new CancellationTokenSource();
        var seen = new List<int>();

        var ex = await Assert.ThrowsAsync<OperationCanceledException>(() =>
            CancellableOperation.RunAsync(10, i =>
            {
                seen.Add(i);
                if (i == 3)
                {
                    cts.Cancel();
                }
            }, cts.Token));

        // Cancellation must be observed before the next step's work runs, so
        // exactly the steps up to and including the one that requested
        // cancellation were performed — the loop never reaches completion.
        Assert.Equal(new[] { 0, 1, 2, 3 }, seen);
        Assert.Equal(cts.Token, ex.CancellationToken);
    }

    [Fact]
    public async Task ThrowsImmediately_WhenAlreadyCancelledBeforeStart()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var seen = new List<int>();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            CancellableOperation.RunAsync(10, i => seen.Add(i), cts.Token));

        Assert.Empty(seen);
    }
}
