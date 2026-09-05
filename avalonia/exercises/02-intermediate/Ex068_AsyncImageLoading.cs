using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

/// Exercise 068 - AsyncImageLoading (intermediate).
/// Goal:   Load an image asynchronously behind a placeholder, and make a NEWER
///         request win. The interesting failure this guards against is not a
///         crash: it is a slow first request landing after a fast second one and
///         quietly replacing the right image with a stale one.
/// Drills: CancellationTokenSource per request, cancelling the previous one,
///         awaiting with a token, deriving displayed state from two properties.
/// Passes: dotnet test --filter FullyQualifiedName~Ex068_
///
/// ex042 already covered a CancellationToken inside a ReactiveCommand. This row is
/// the other half of the problem: you own the token source, you must cancel the
/// one still in flight before starting the next, and a cancelled load must leave
/// no trace - not in Current, not in IsLoading.
///
/// The feed is given and is driven by the test, not by a timer: RequestAsync hands
/// back a Task that completes only when the test says so. No delays, no sleeps, so
/// the ordering the test exercises is exact rather than likely.
public class Ex068_AsyncImageLoading : ReactiveObject
{
    /// <summary>Given. Do not change. Shown until a real image has arrived.</summary>
    public static readonly Bitmap Placeholder =
        new WriteableBitmap(new PixelSize(1, 1), new Vector(96, 96));

    /// <summary>Given. Do not change.</summary>
    public Ex068_ImageFeed Feed { get; } = new();

    public bool IsLoading
    {
        get => throw new NotImplementedException(
            "TODO: Ex068 - true from the moment LoadAsync starts until the request " +
            "that is still current has finished; a superseded request must not clear " +
            "it, because a newer one is by then in flight. Back it with " +
            "RaiseAndSetIfChanged so the view can bind");
        private set => throw new NotImplementedException("TODO: Ex068 - see the getter");
    }

    public Bitmap? Loaded
    {
        get => throw new NotImplementedException(
            "TODO: Ex068 - the bitmap the current request produced, null until one " +
            "has. Back it with RaiseAndSetIfChanged");
        private set => throw new NotImplementedException("TODO: Ex068 - see the getter");
    }

    /// <summary>What a view binds its Image.Source to.</summary>
    public IImage Current =>
        throw new NotImplementedException(
            "TODO: Ex068 - Loaded once there is one, Placeholder until then");

    public Task LoadAsync(string key) =>
        throw new NotImplementedException(
            "TODO: Ex068 - cancel the token source of any request still in flight, " +
            "make a fresh one, set IsLoading, await Feed.RequestAsync(key, token), " +
            "and write the result into Loaded ONLY IF this request is still the " +
            "current one. A cancelled await throws OperationCanceledException - " +
            "swallow it, and leave IsLoading alone in that case, because the request " +
            "that superseded this one is still running");
}

/// <summary>
/// Given. Do not change. A fake image source the test drives by hand: every
/// request records its key and hands back a pending Task, which the test completes
/// (or leaves hanging) in whatever order it wants to reproduce.
/// </summary>
public class Ex068_ImageFeed
{
    private readonly List<Pending> _pending = [];

    public IReadOnlyList<Pending> Requests => _pending;

    public Task<Bitmap> RequestAsync(string key, CancellationToken token)
    {
        var entry = new Pending(key, token);
        _pending.Add(entry);
        return entry.Task;
    }

    /// <summary>Completes the n-th request with a bitmap whose width identifies it.</summary>
    public void Complete(int index, int widthMarker) =>
        _pending[index].Source.TrySetResult(
            new WriteableBitmap(new PixelSize(widthMarker, 1), new Vector(96, 96)));

    public sealed class Pending
    {
        public Pending(string key, CancellationToken token)
        {
            Key = key;
            Token = token;
            // Honouring the token here is what lets an await on this task throw
            // OperationCanceledException the moment the caller cancels.
            token.Register(() => Source.TrySetCanceled(token));
        }

        public string Key { get; }

        public CancellationToken Token { get; }

        public TaskCompletionSource<Bitmap> Source { get; } = new();

        public Task<Bitmap> Task => Source.Task;
    }
}
