using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Exercises.Advanced;

/// Exercise 081 - DragAndDropPayload (advanced).
/// Goal:   Make a control a drop target: opt in, inspect what is being dragged
///         BEFORE it lands, answer with the effect you will apply, and read the
///         payload out on the drop itself.
/// Drills: DragDrop.AllowDrop, the DragEnter/DragOver/Drop attached routed events,
///         IDataTransfer and IDataTransferItem.TryGetRaw, DataFormat, DragEffects.
/// Passes: dotnet test --filter FullyQualifiedName~Ex081_
///
/// USE DataTransfer, NOT DataObject. Avalonia 12 replaced the whole payload model:
/// DataObject and DataFormats are both marked [Obsolete] ("Use DataTransfer
/// instead", "Use DataFormat instead"), and IDataObject is gone entirely. The
/// current shape is a DataTransfer holding DataTransferItems, each carrying values
/// under a DataFormat - DataFormat.Text for plain text, or one you mint yourself
/// with DataFormat.CreateStringApplicationFormat. Reaching for the obsolete types
/// would put warnings in a build this track keeps at zero.
///
/// DragOver is where the answer to "may I drop here?" belongs, and DragEffects is
/// how you give it: set it to None for a payload you cannot use and the platform
/// shows the user a no-entry cursor rather than letting them find out by dropping.
///
/// Measured, and the reason the AllowDrop half is graded separately: with AllowDrop
/// left off, a synthesised drop produced NOTHING - no enter, no over, no drop. The
/// opt-in is not advisory.
public class Ex081_DragAndDropPayload : Border
{
    /// <summary>Given. Do not change. Records one line per event, in order.</summary>
    public List<string> Log { get; } = [];

    /// <summary>Given. Do not change. The effect answered for the last DragOver.</summary>
    public DragDropEffects LastOfferedEffect { get; private set; } = DragDropEffects.None;

    /// <summary>Given. Do not change. Call this from your DragOver handler.</summary>
    protected void RecordOfferedEffect(DragDropEffects effect) => LastOfferedEffect = effect;

    /// <summary>
    /// Opt in to drops and wire the three handlers. Called from the constructor,
    /// which is given.
    ///
    ///   DragEnter  appends "enter"
    ///   DragOver   appends "over", and sets e.DragEffects to Copy when the
    ///              payload carries DataFormat.Text, or to None when it does not -
    ///              then calls RecordOfferedEffect with whatever it decided
    ///   Drop       appends "drop:" followed by the dragged text, or "drop:none"
    ///              when there is no text, and marks the event Handled
    ///
    /// Handlers for these go on with AddHandler(DragDrop.DropEvent, ...) and
    /// friends: they are attached routed events, not properties of this class, so
    /// there is nothing to override.
    /// </summary>
    private void Wire() =>
        throw new NotImplementedException(
            "TODO: Ex081 - DragDrop.SetAllowDrop(this, true), then AddHandler for " +
            "DragDrop.DragEnterEvent, DragOverEvent and DropEvent per the contract " +
            "above. Read the payload with " +
            "e.DataTransfer.Items[..].TryGetRaw(DataFormat.Text)");

    public Ex081_DragAndDropPayload()
    {
        // Given. Do not change: a control with no Background is invisible to hit
        // testing, and a drop has to hit something.
        Background = Brushes.Transparent;
        Wire();
    }
}
