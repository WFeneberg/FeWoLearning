using System;
using System.Collections.Generic;
using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex059_ReadonlyCollectionWrapperTests
{
    [Fact]
    public void Items_ReflectsAddedElementsInOrder()
    {
        var wrapper = new ReadonlyCollectionWrapper();
        wrapper.Add("alpha");
        wrapper.Add("beta");

        Assert.Equal(new[] { "alpha", "beta" }, wrapper.Items);
        Assert.Equal(2, wrapper.Count);
    }

    [Fact]
    public void Items_CannotBeCastBackToAMutableList()
    {
        var wrapper = new ReadonlyCollectionWrapper();
        wrapper.Add("alpha");

        var view = wrapper.Items;

        Assert.Throws<InvalidCastException>(() => (List<string>)view);
    }

    [Fact]
    public void Items_ThrowsWhenMutatedThroughAnyImplementedInterface()
    {
        var wrapper = new ReadonlyCollectionWrapper();
        wrapper.Add("alpha");

        var mutableView = Assert.IsAssignableFrom<ICollection<string>>(wrapper.Items);

        Assert.Throws<NotSupportedException>(() => mutableView.Add("intruder"));
        Assert.Throws<NotSupportedException>(() => mutableView.Remove("alpha"));
        Assert.Throws<NotSupportedException>(() => mutableView.Clear());

        // The failed mutation attempts must not have changed anything.
        Assert.Equal(new[] { "alpha" }, wrapper.Items);
    }

    [Fact]
    public void Items_IsALiveViewThatSeesLaterInternalAdditions()
    {
        var wrapper = new ReadonlyCollectionWrapper();
        wrapper.Add("first");

        var view = wrapper.Items;
        Assert.Equal(1, view.Count);

        // Mutate the wrapper AFTER capturing the view reference: a defensive
        // copy/snapshot would now be stale, but a live view must reflect it.
        wrapper.Add("second");
        wrapper.Add("third");

        Assert.Equal(3, view.Count);
        Assert.Equal(new[] { "first", "second", "third" }, view);
        Assert.Equal("third", view[2]);
    }

    [Fact]
    public void Add_RejectsNullItem()
    {
        var wrapper = new ReadonlyCollectionWrapper();

        Assert.Throws<ArgumentNullException>(() => wrapper.Add(null!));
    }
}
