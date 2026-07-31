using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex005_ListOperationsTests
{
    [Fact]
    public void AddRemoveDeduplicate_SequenceProducesExpectedContentAndOrder()
    {
        var numbers = new List<int> { 1, 2, 3 };

        ListOperations.AddValue(numbers, 2);
        ListOperations.AddValue(numbers, 4);
        // numbers: [1, 2, 3, 2, 4]

        ListOperations.RemoveFirst(numbers, 2);
        // numbers: [1, 3, 2, 4]

        var result = ListOperations.Deduplicate(numbers);

        Assert.Equal(new List<int> { 1, 3, 2, 4 }, result);
    }

    [Fact]
    public void RemoveFirst_ValueNotPresent_LeavesListUnchanged()
    {
        var numbers = new List<int> { 5, 6, 7 };

        ListOperations.RemoveFirst(numbers, 42);

        Assert.Equal(new List<int> { 5, 6, 7 }, numbers);
    }

    [Fact]
    public void Deduplicate_KeepsFirstOccurrenceOrder()
    {
        var numbers = new List<int> { 4, 1, 4, 2, 1, 3 };

        var result = ListOperations.Deduplicate(numbers);

        Assert.Equal(new List<int> { 4, 1, 2, 3 }, result);
    }
}
