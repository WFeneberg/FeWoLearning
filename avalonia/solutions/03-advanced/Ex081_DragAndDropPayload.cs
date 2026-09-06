using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

// Passes: dotnet test --filter FullyQualifiedName~Ex081_
public class Ex081_DragAndDropPayload : Border
{
    /// <summary>Given. Do not change.</summary>
    public List<string> Log { get; } = [];

    /// <summary>Given. Do not change.</summary>
    public DragDropEffects LastOfferedEffect { get; private set; } = DragDropEffects.None;

    /// <summary>Given. Do not change.</summary>
    protected void RecordOfferedEffect(DragDropEffects effect) => LastOfferedEffect = effect;

    private static string? TextOf(DragEventArgs e) =>
        e.DataTransfer.Items
            .Select(item => item.TryGetRaw(DataFormat.Text) as string)
            .FirstOrDefault(text => text is not null);

    private void Wire()
    {
        DragDrop.SetAllowDrop(this, true);

        AddHandler(DragDrop.DragEnterEvent, (object? _, DragEventArgs _) => Log.Add("enter"));

        AddHandler(DragDrop.DragOverEvent, (object? _, DragEventArgs e) =>
        {
            Log.Add("over");

            // The answer belongs here rather than on the drop: this is what the
            // platform turns into the cursor the user sees.
            var effect = TextOf(e) is null ? DragDropEffects.None : DragDropEffects.Copy;
            e.DragEffects = effect;
            RecordOfferedEffect(effect);
        });

        AddHandler(DragDrop.DropEvent, (object? _, DragEventArgs e) =>
        {
            Log.Add($"drop:{TextOf(e) ?? "none"}");
            e.Handled = true;
        });
    }

    public Ex081_DragAndDropPayload()
    {
        // Given. Do not change.
        Background = Brushes.Transparent;
        Wire();
    }
}
