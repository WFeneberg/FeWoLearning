using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex068_
public class Ex068_AsyncImageLoading : ReactiveObject
{
    /// <summary>Given. Do not change.</summary>
    public static readonly Bitmap Placeholder =
        new WriteableBitmap(new PixelSize(1, 1), new Vector(96, 96));

    private CancellationTokenSource? _inFlight;
    private bool _isLoading;
    private Bitmap? _loaded;

    /// <summary>Given. Do not change.</summary>
    public Ex068_ImageFeed Feed { get; } = new();

    public bool IsLoading
    {
        get => _isLoading;
        private set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public Bitmap? Loaded
    {
        get => _loaded;
        private set
        {
            this.RaiseAndSetIfChanged(ref _loaded, value);
            this.RaisePropertyChanged(nameof(Current));
        }
    }

    public IImage Current => Loaded ?? Placeholder;

    public async Task LoadAsync(string key)
    {
        _inFlight?.Cancel();
        var cts = new CancellationTokenSource();
        _inFlight = cts;

        IsLoading = true;

        try
        {
            var bitmap = await Feed.RequestAsync(key, cts.Token);

            // Only the request that is still current may publish its result: a
            // superseded one has to land silently, or it overwrites the newer image.
            if (!cts.Token.IsCancellationRequested)
            {
                Loaded = bitmap;
                IsLoading = false;
            }
        }
        catch (OperationCanceledException)
        {
            // Superseded. IsLoading stays as it is, because the request that
            // replaced this one is still in flight.
        }
    }
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
