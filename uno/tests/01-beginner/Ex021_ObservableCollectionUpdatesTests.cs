using System.Collections.Specialized;
using FeWoLearning.Uno.Exercises.Beginner;
using Microsoft.UI.Xaml.Controls;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex021_ObservableCollectionUpdatesTests : UnoTestContext
{
    private static Ex021_ObservableCollectionUpdates Playlist(params string[] tracks)
    {
        var playlist = new Ex021_ObservableCollectionUpdates();
        foreach (var track in tracks)
        {
            playlist.Tracks.Add(track);
        }

        return playlist;
    }

    /// <summary>
    /// Reads the elements back by item index. The realised children sit in the visual tree
    /// in whatever order the repeater recycled them into, which is not the item order once
    /// anything has moved - TryGetElement is the mapping that stays true.
    /// </summary>
    private static List<string> Rendered(ItemsRepeater view, int count)
    {
        var texts = new List<string>();
        for (var i = 0; i < count; i++)
        {
            texts.Add(((TextBlock)view.TryGetElement(i)!).Text);
        }

        return texts;
    }

    [Fact]
    public void Renders_The_Tracks_It_Starts_With()
    {
        var playlist = Playlist("one", "two");

        var view = Layout(playlist.CreateView());

        Assert.Equal(["one", "two"], Rendered(view, 2));
    }

    [Fact]
    public void An_Added_Track_Appears_Without_Rebuilding_The_View()
    {
        var playlist = Playlist("one", "two");
        var view = Layout(playlist.CreateView());

        playlist.Tracks.Add("three");
        Layout(view);

        Assert.Equal(["one", "two", "three"], Rendered(view, 3));
    }

    [Fact]
    public void A_Removed_Track_Disappears()
    {
        var playlist = Playlist("one", "two", "three");
        var view = Layout(playlist.CreateView());

        playlist.Tracks.Remove("two");
        Layout(view);

        Assert.Equal(["one", "three"], Rendered(view, 2));
    }

    [Fact]
    public void Moving_Up_Reorders_The_Tracks()
    {
        var playlist = Playlist("one", "two", "three");

        playlist.MoveUp("three");

        Assert.Equal(["one", "three", "two"], playlist.Tracks);
    }

    [Fact]
    public void Moving_Up_Is_A_Single_Change()
    {
        var playlist = Playlist("one", "two", "three");
        var actions = new List<NotifyCollectionChangedAction>();
        playlist.Tracks.CollectionChanged += (_, e) => actions.Add(e.Action);

        playlist.MoveUp("three");

        // Remove-then-insert would be two events, and the UI would discard and rebuild
        // the element in between instead of keeping it.
        Assert.Equal([NotifyCollectionChangedAction.Move], actions);
    }

    [Fact]
    public void Moving_The_First_Track_Up_Does_Nothing()
    {
        var playlist = Playlist("one", "two");
        var changes = 0;
        playlist.Tracks.CollectionChanged += (_, _) => changes++;

        playlist.MoveUp("one");
        playlist.MoveUp("not in the list");

        Assert.Equal(["one", "two"], playlist.Tracks);
        Assert.Equal(0, changes);
    }

    [Fact]
    public void Replacing_Everything_Ends_Up_With_The_New_List()
    {
        var playlist = Playlist("one", "two", "three");

        playlist.ReplaceAll(["two", "four"]);

        Assert.Equal(["two", "four"], playlist.Tracks);
    }

    [Fact]
    public void Replacing_Everything_Never_Raises_A_Reset()
    {
        var playlist = Playlist("one", "two", "three");
        var actions = new List<NotifyCollectionChangedAction>();
        playlist.Tracks.CollectionChanged += (_, e) => actions.Add(e.Action);

        playlist.ReplaceAll(["two", "four"]);

        // Clear() is one line and raises Reset, which tells every listener to throw away
        // everything it has - including the element for "two", which did not change.
        Assert.DoesNotContain(NotifyCollectionChangedAction.Reset, actions);
        Assert.NotEmpty(actions);
    }

    [Fact]
    public void Replacing_Everything_Reaches_The_View()
    {
        var playlist = Playlist("one", "two", "three");
        var view = Layout(playlist.CreateView());

        playlist.ReplaceAll(["two", "four"]);
        Layout(view);

        Assert.Equal(["two", "four"], Rendered(view, 2));
    }

    [Fact]
    public void Replacing_With_The_Same_Contents_Changes_Nothing()
    {
        var playlist = Playlist("one", "two");
        var changes = 0;
        playlist.Tracks.CollectionChanged += (_, _) => changes++;

        playlist.ReplaceAll(["one", "two"]);

        Assert.Equal(0, changes);
    }
}
