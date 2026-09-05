using System.Collections.Generic;
using System.Collections.Specialized;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex070_ObservableCollectionSyncTests
{
    private sealed class Recorder
    {
        private readonly List<NotifyCollectionChangedEventArgs> _events = [];

        public Recorder(Ex070_ObservableCollectionSync subject) =>
            subject.Target.CollectionChanged += (_, e) => _events.Add(e);

        public IReadOnlyList<NotifyCollectionChangedEventArgs> Events => _events;

        public void Clear() => _events.Clear();
    }

    private static Ex070_ObservableCollectionSync Seeded(params string[] items)
    {
        var subject = new Ex070_ObservableCollectionSync();
        subject.SyncTo(items);
        return subject;
    }

    [AvaloniaFact]
    public void Filling_An_Empty_Target_Adds_Each_Item()
    {
        var subject = new Ex070_ObservableCollectionSync();
        var recorder = new Recorder(subject);

        subject.SyncTo(["a", "b", "c"]);

        Assert.Equal(["a", "b", "c"], subject.Target);
        Assert.All(recorder.Events, e => Assert.Equal(NotifyCollectionChangedAction.Add, e.Action));
        Assert.Equal(3, recorder.Events.Count);
    }

    // The strongest simplicity check in the file, and the one a Clear-and-refill
    // fails hardest: nothing changed, so nothing at all may be reported.
    [AvaloniaFact]
    public void Syncing_To_An_Identical_List_Raises_Nothing()
    {
        var subject = Seeded("a", "b", "c");
        var recorder = new Recorder(subject);

        subject.SyncTo(["a", "b", "c"]);

        Assert.Empty(recorder.Events);
        Assert.Equal(["a", "b", "c"], subject.Target);
    }

    [AvaloniaFact]
    public void Dropping_A_Middle_Item_Raises_Exactly_One_Remove()
    {
        var subject = Seeded("a", "b", "c");
        var recorder = new Recorder(subject);

        subject.SyncTo(["a", "c"]);

        var single = Assert.Single(recorder.Events);
        Assert.Equal(NotifyCollectionChangedAction.Remove, single.Action);
        Assert.Equal(1, single.OldStartingIndex);
        Assert.Equal("b", Assert.Single(single.OldItems!.Cast<string>()));
        Assert.Equal(["a", "c"], subject.Target);
    }

    [AvaloniaFact]
    public void Appending_One_Item_Raises_Exactly_One_Add()
    {
        var subject = Seeded("a", "c");
        var recorder = new Recorder(subject);

        subject.SyncTo(["a", "c", "d"]);

        var single = Assert.Single(recorder.Events);
        Assert.Equal(NotifyCollectionChangedAction.Add, single.Action);
        Assert.Equal(2, single.NewStartingIndex);
        Assert.Equal("d", Assert.Single(single.NewItems!.Cast<string>()));
    }

    // A reversal is the case where a lazy answer is most tempting. The event
    // count is bounded rather than pinned exactly, so a different but still
    // incremental algorithm is not punished - six events would mean every item
    // was torn out and re-added, which is a rebuild wearing a diff's clothes.
    [AvaloniaFact]
    public void Reversing_The_Order_Is_Done_Incrementally()
    {
        var subject = Seeded("a", "b", "c");
        var recorder = new Recorder(subject);

        subject.SyncTo(["c", "b", "a"]);

        Assert.Equal(["c", "b", "a"], subject.Target);
        Assert.InRange(recorder.Events.Count, 1, 4);
    }

    // Reset is the whole reason this exercise is not "just call Clear": it tells
    // a bound control that everything it knew is gone, so selection, scroll
    // position and item containers are all discarded.
    [AvaloniaFact]
    public void No_Sync_Ever_Raises_A_Reset_And_The_Instance_Is_Never_Replaced()
    {
        var subject = new Ex070_ObservableCollectionSync();
        var instance = subject.Target;
        var recorder = new Recorder(subject);

        subject.SyncTo(["a", "b", "c"]);
        subject.SyncTo(["c", "b", "a"]);
        subject.SyncTo(["b"]);
        subject.SyncTo([]);
        subject.SyncTo(["x", "y"]);

        Assert.DoesNotContain(
            recorder.Events,
            e => e.Action == NotifyCollectionChangedAction.Reset);
        Assert.Same(instance, subject.Target);
        Assert.Equal(["x", "y"], subject.Target);
    }
}
