using System.Threading.Tasks;
using Avalonia.Headless.XUnit;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex068_AsyncImageLoadingTests
{
    [AvaloniaFact]
    public void Before_Any_Load_The_Placeholder_Is_What_A_View_Would_Show()
    {
        var vm = new Ex068_AsyncImageLoading();

        Assert.False(vm.IsLoading);
        Assert.Null(vm.Loaded);
        Assert.Same(Ex068_AsyncImageLoading.Placeholder, vm.Current);
    }

    // IsLoading has to be set before the first await, or a view binding to it
    // never sees the loading state at all: by the time control returns here the
    // request is already outstanding.
    [AvaloniaFact]
    public void While_A_Request_Is_Outstanding_The_Placeholder_Stays_Up()
    {
        var vm = new Ex068_AsyncImageLoading();

        _ = vm.LoadAsync("portrait");

        Assert.True(vm.IsLoading);
        Assert.Null(vm.Loaded);
        Assert.Same(Ex068_AsyncImageLoading.Placeholder, vm.Current);
        Assert.Equal("portrait", Assert.Single(vm.Feed.Requests).Key);
    }

    [AvaloniaFact]
    public async Task A_Completed_Request_Replaces_The_Placeholder()
    {
        var vm = new Ex068_AsyncImageLoading();

        var load = vm.LoadAsync("portrait");
        vm.Feed.Complete(0, widthMarker: 7);
        await load;

        Assert.False(vm.IsLoading);
        Assert.NotNull(vm.Loaded);
        Assert.Equal(7, vm.Loaded!.PixelSize.Width);
        Assert.Same(vm.Loaded, vm.Current);
    }

    // Starting a second load must cancel the first one's token. Without this the
    // only thing stopping a stale result is luck about which task finishes
    // first.
    [AvaloniaFact]
    public void A_Second_Load_Cancels_The_First_Requests_Token()
    {
        var vm = new Ex068_AsyncImageLoading();

        _ = vm.LoadAsync("first");
        _ = vm.LoadAsync("second");

        Assert.Equal(2, vm.Feed.Requests.Count);
        Assert.True(vm.Feed.Requests[0].Token.IsCancellationRequested);
        Assert.False(vm.Feed.Requests[1].Token.IsCancellationRequested);
    }

    // The whole point of the row: the slow first request lands LAST, and must
    // not win. An implementation that simply assigns whatever its await returned
    // passes every other test in this file and fails this one, which is the
    // exact bug the exercise is about.
    [AvaloniaFact]
    public async Task A_Superseded_Request_Landing_Late_Does_Not_Overwrite_The_Newer_Image()
    {
        var vm = new Ex068_AsyncImageLoading();

        var first = vm.LoadAsync("first");
        var second = vm.LoadAsync("second");

        vm.Feed.Complete(1, widthMarker: 3);
        await second;
        await first;

        // The first request's own result arrives only now.
        vm.Feed.Complete(0, widthMarker: 9);
        await first;

        Assert.Equal(3, vm.Loaded!.PixelSize.Width);
        Assert.False(vm.IsLoading);
    }
}
