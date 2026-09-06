using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex082_
public class Ex082_ClipboardRoundTrip : Control
{
    public IClipboard? Clipboard => TopLevel.GetTopLevel(this)?.Clipboard;

    public async Task CopyAsync(string text)
    {
        if (Clipboard is not { } clipboard)
        {
            return;
        }

        var transfer = new DataTransfer();
        transfer.Add(DataTransferItem.CreateText(text));
        await clipboard.SetDataAsync(transfer);
    }

    public async Task<string?> PasteAsync()
    {
        if (Clipboard is not { } clipboard)
        {
            return null;
        }

        // An empty clipboard hands back null, not an empty transfer.
        using var transfer = await clipboard.TryGetDataAsync();

        return transfer is null ? null : await transfer.TryGetTextAsync();
    }

    public async Task ClearAsync()
    {
        if (Clipboard is { } clipboard)
        {
            await clipboard.ClearAsync();
        }
    }
}
