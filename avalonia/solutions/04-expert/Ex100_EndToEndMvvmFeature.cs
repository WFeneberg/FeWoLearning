using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Concurrency;

namespace FeWoLearning.Avalonia.Exercises.Expert;

// Passes: dotnet test --filter FullyQualifiedName~Ex100_
public class Ex100_EndToEndMvvmFeature : ReactiveObject, INotifyDataErrorInfo, IScreen
{
    private const int MaxLength = 20;

    /// <summary>Given. Do not change.</summary>
    public RoutingState Router { get; } = new();

    /// <summary>Given. Do not change.</summary>
    public Ex100_Gateway Gateway { get; } = new();

    /// <summary>Given. Do not change.</summary>
    public List<string> SurfacedErrors { get; } = [];

    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        set
        {
            this.RaiseAndSetIfChanged(ref _name, value);
            RaiseErrorsChanged();
        }
    }

    /// <summary>Given. Do not change.</summary>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>Given. Do not change.</summary>
    protected void RaiseErrorsChanged() =>
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Name)));

    public bool HasErrors => Message is not null;

    private string? Message => Name.Length switch
    {
        0 => Ex100_Messages.Required,
        > MaxLength => Ex100_Messages.TooLong,
        _ => null,
    };

    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName == nameof(Name) && Message is { } message)
        {
            yield return message;
        }
    }

    public ReactiveCommand<RxVoid, RxVoid> Save { get; private set; } = null!;

    private void Wire()
    {
        // Gated on the validity, so the command is never executable for an invalid
        // form rather than starting and then giving up.
        var canSave = this.WhenAnyValue(x => x.Name).Select(_ => !HasErrors);

        // Sequencer.CurrentThread is not optional: without it the result lands on
        // another scheduler and neither the navigation nor the error surface is
        // observable inline. ex041 records the same finding.
        Save = ReactiveCommand.CreateFromTask(() => Gateway.SaveAsync(Name), canSave, Sequencer.CurrentThread);

        // Navigation hangs off the RESULT, so a failed save moves nobody.
        Save.Subscribe(_ => Router.Navigate
            .Execute(new Ex100_DoneViewModel(this))
            .Subscribe(_ => { }, _ => { }));

        // Without this the exception goes unobserved and the form looks saved.
        Save.ThrownExceptions.Subscribe(error => SurfacedErrors.Add(error.Message));
    }

    public Ex100_EndToEndMvvmFeature() => Wire();
}

/// <summary>Given. Do not change.</summary>
public static class Ex100_Messages
{
    public const string Required = "a name is required";

    public const string TooLong = "a name may be at most 20 characters";
}

/// <summary>
/// Given. Do not change. Configured BEFORE a save rather than settled afterwards,
/// so every outcome is already decided when the command runs and nothing has to be
/// waited on.
/// </summary>
public class Ex100_Gateway
{
    /// <summary>What each request was asked to save, in order.</summary>
    public List<string> Requests { get; } = [];

    /// <summary>Null to succeed; a message to fail with it.</summary>
    public string? FailWith { get; set; }

    /// <summary>When true a save never finishes, for testing the in-flight state.</summary>
    public bool Stall { get; set; }

    public Task SaveAsync(string name)
    {
        Requests.Add(name);

        if (Stall)
        {
            return new TaskCompletionSource().Task;
        }

        return FailWith is null
            ? Task.CompletedTask
            : Task.FromException(new InvalidOperationException(FailWith));
    }
}

/// <summary>Given. Do not change. Where a successful save lands.</summary>
public class Ex100_DoneViewModel(IScreen hostScreen) : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => "done";

    public IScreen HostScreen { get; } = hostScreen;
}
