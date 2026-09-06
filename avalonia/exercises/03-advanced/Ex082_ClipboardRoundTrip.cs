using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 082 - ClipboardRoundTrip (advanced).
/// Goal:   Put text on the clipboard and read it back, through the only route
///         Avalonia offers: the clipboard belongs to the TOP LEVEL, not to the
///         application, so a control has to find its own window first.
/// Drills: TopLevel.GetTopLevel, TopLevel.Clipboard, IClipboard.SetDataAsync,
///         TryGetDataAsync and ClearAsync, DataTransfer with a text item.
/// Passes: dotnet test --filter FullyQualifiedName~Ex082_
///
/// THE API IS NOT THE ONE IN THE SAMPLES. There is no SetTextAsync or GetTextAsync
/// any more. IClipboard in Avalonia 12.1.1 is five members - ClearAsync,
/// FlushAsync, SetDataAsync(IAsyncDataTransfer), TryGetDataAsync() and
/// TryGetInProcessDataAsync() - so text goes on as a DataTransfer holding a
/// DataTransferItem, exactly as a drag payload does in ex081, and comes back as
/// something you have to ask for a format - or, more conveniently, hand to the
/// TryGetTextAsync extension in AsyncDataTransferExtensions, which is the shortest
/// honest route and the one to prefer. Note the async split: what
/// TryGetDataAsync hands back is an IAsyncDataTransfer, whose items expose
/// TryGetRawAsync rather than the synchronous TryGetRaw a drop's IDataTransfer
/// has.
///
/// Measured in this headless harness, so all three of these are real assertions
/// rather than hopeful ones: a set/get round trip returns the text; the read-back
/// transfer reports exactly one format, Text; and after ClearAsync,
/// TryGetDataAsync returns NULL rather than an empty transfer. Remember the null:
/// an implementation that assumes a transfer is always there throws on an empty
/// clipboard, which is the state it is in most of the time.
///
/// The clipboard is process-global state, and this track's suite runs serially -
/// so anything you leave on it is still there for the next test. Clear up after
/// yourself in real code for the same reason.
public class Ex082_ClipboardRoundTrip : Control
{
    /// <summary>
    /// This control's clipboard, or null when it is not in a window yet - which is
    /// the case for every control before it is shown, so callers have to cope.
    /// </summary>
    public IClipboard? Clipboard =>
        throw new NotImplementedException(
            "TODO: Ex082 - reach the clipboard through TopLevel.GetTopLevel(this), " +
            "returning null when there is no TopLevel");

    /// <summary>Puts <paramref name="text"/> on the clipboard as a text payload.</summary>
    public Task CopyAsync(string text) =>
        throw new NotImplementedException(
            "TODO: Ex082 - build a DataTransfer, Add a DataTransferItem carrying " +
            "the text, and hand it to the clipboard's SetDataAsync");

    /// <summary>
    /// The clipboard's text, or null when the clipboard is empty or holds
    /// something that is not text.
    /// </summary>
    public Task<string?> PasteAsync() =>
        throw new NotImplementedException(
            "TODO: Ex082 - TryGetDataAsync, and mind that it returns null on an " +
            "empty clipboard. Otherwise ask the transfer for its text with the " +
            "TryGetTextAsync extension. Dispose the transfer when you are done with " +
            "it - it is IDisposable, and holding one open holds the payload alive");

    public Task ClearAsync() =>
        throw new NotImplementedException("TODO: Ex082 - clear the clipboard");
}
