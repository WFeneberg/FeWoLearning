using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex035_QueueStackSimulationTests
{
    [Fact]
    public void ProcessAll_ProcessesJobsInFifoOrder()
    {
        var printQueue = new Queue<string>(new[] { "job-A", "job-B", "job-C" });
        var undoHistory = new Stack<string>();

        List<string> processed = QueueStackSimulation.ProcessAll(printQueue, undoHistory);

        Assert.Equal(new[] { "job-A", "job-B", "job-C" }, processed);
        Assert.Empty(printQueue);
    }

    [Fact]
    public void Undo_ReversesLastProcessedJob()
    {
        var printQueue = new Queue<string>(new[] { "job-A", "job-B", "job-C" });
        var undoHistory = new Stack<string>();

        QueueStackSimulation.ProcessAll(printQueue, undoHistory);

        string? undone = QueueStackSimulation.Undo(undoHistory);

        Assert.Equal("job-C", undone);
        Assert.Equal(2, undoHistory.Count);
    }

    [Fact]
    public void Undo_ReturnsNullWhenHistoryIsEmpty()
    {
        var undoHistory = new Stack<string>();

        string? undone = QueueStackSimulation.Undo(undoHistory);

        Assert.Null(undone);
    }
}
