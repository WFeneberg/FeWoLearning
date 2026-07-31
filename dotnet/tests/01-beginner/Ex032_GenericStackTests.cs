using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex032_GenericStackTests
{
    [Fact]
    public void Push_And_Pop_ReturnItems_InLifoOrder_ForInt()
    {
        var stack = new GenericStack<int>();

        stack.Push(1);
        stack.Push(2);
        stack.Push(3);

        Assert.Equal(3, stack.Count);
        Assert.Equal(3, stack.Peek());
        Assert.Equal(3, stack.Pop());
        Assert.Equal(2, stack.Pop());
        Assert.Equal(1, stack.Pop());
        Assert.Equal(0, stack.Count);
    }

    [Fact]
    public void Push_And_Pop_ReturnItems_InLifoOrder_ForString()
    {
        var stack = new GenericStack<string>();

        stack.Push("first");
        stack.Push("second");
        stack.Push("third");

        Assert.Equal("third", stack.Peek());
        Assert.Equal("third", stack.Pop());
        Assert.Equal("second", stack.Pop());
        Assert.Equal("first", stack.Pop());
        Assert.Equal(0, stack.Count);
    }
}
