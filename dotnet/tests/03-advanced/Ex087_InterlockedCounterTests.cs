using System;
using System.Threading.Tasks;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex087_InterlockedCounterTests
{
    [Fact]
    public void Increment_StartsAtZeroByDefault()
    {
        var counter = new InterlockedCounter();
        Assert.Equal(0, counter.Value);
        Assert.Equal(1, counter.Increment());
        Assert.Equal(1, counter.Value);
    }

    [Fact]
    public async Task Increment_FromManyConcurrentTasks_LosesNoUpdates()
    {
        const int taskCount = 50;
        const int incrementsPerTask = 2000;
        const long expectedTotal = taskCount * incrementsPerTask; // 100_000

        var counter = new InterlockedCounter();
        var tasks = new Task[taskCount];
        for (int t = 0; t < taskCount; t++)
        {
            tasks[t] = Task.Run(() =>
            {
                for (int i = 0; i < incrementsPerTask; i++)
                    counter.Increment();
            });
        }

        await Task.WhenAll(tasks);

        // With a naive (non-atomic) "read, add 1, write" counter this races and
        // reliably ends up below expectedTotal; Interlocked.Increment must not.
        Assert.Equal(expectedTotal, counter.Value);
    }

    [Fact]
    public async Task Add_FromManyConcurrentTasks_SumsExactly()
    {
        const int taskCount = 20;
        var counter = new InterlockedCounter(initialValue: 100);
        var tasks = new Task[taskCount];
        long expectedTotal = 100;

        for (int t = 0; t < taskCount; t++)
        {
            int amount = t + 1; // 1..20
            expectedTotal += amount;
            tasks[t] = Task.Run(() => counter.Add(amount));
        }

        await Task.WhenAll(tasks);

        Assert.Equal(expectedTotal, counter.Value);
    }

    [Fact]
    public void Reset_ReturnsPreviousValueAndZeroesCounter()
    {
        var counter = new InterlockedCounter();
        counter.Increment();
        counter.Increment();
        counter.Increment();

        long previous = counter.Reset();

        Assert.Equal(3, previous);
        Assert.Equal(0, counter.Value);
    }
}
