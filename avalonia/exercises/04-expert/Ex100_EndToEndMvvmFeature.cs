using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace FeWoLearning.Avalonia.Exercises.Expert;

/// Exercise 100 - EndToEndMvvmFeature (expert).
/// Goal:   One feature that has to get all three right at once: a form that
///         VALIDATES, saves ASYNCHRONOUSLY, and NAVIGATES only when the save
///         actually succeeded. Each of those exists in an earlier row on its own;
///         what breaks in real applications is the wiring between them.
/// Drills: INotifyDataErrorInfo, ReactiveCommand.CreateFromTask with a canExecute,
///         RoutingState navigation gated on a result, error surfaces.
/// Passes: dotnet test --filter FullyQualifiedName~Ex100_
///
/// THE THREE SEAMS, WHICH ARE WHAT THIS ROW GRADES.
///
/// Validation gates the command, not the save. A Save that starts and then decides
/// the form was invalid has already put up a spinner and probably logged an
/// attempt; the command should never have been executable. So canExecute comes
/// from the validity, and the test checks the command reports itself unavailable
/// rather than merely doing nothing.
///
/// Navigation waits for the RESULT. Navigating when the command is invoked - not
/// when it completes successfully - is the classic bug: the user lands on the next
/// page while the save is still in flight, and stays there when it fails. The test
/// makes the save fail and checks nobody moved.
///
/// A failure has to leave a trace. Swallowing it is worse than crashing, because
/// the form then looks saved. ReactiveCommand's ThrownExceptions is the surface
/// for that, and a command whose task throws with nobody subscribed to
/// ThrownExceptions takes the exception somewhere nobody sees.
///
/// The gateway is given and is decided in advance by the test rather than by a
/// timer, so nothing is ever waited on.
///
/// ONE THING THAT WILL COST YOU AN HOUR OTHERWISE: pass Sequencer.CurrentThread to
/// CreateFromTask. Without an explicit sequencer the command delivers its result on
/// another scheduler, and neither the navigation nor the error surface has happened
/// by the time a test looks - measured here, and recorded in ex041 for the same
/// reason.
public class Ex100_EndToEndMvvmFeature : ReactiveObject, INotifyDataErrorInfo, IScreen
{
    /// <summary>Given. Do not change.</summary>
    public RoutingState Router { get; } = new();

    /// <summary>Given. Do not change. The save gateway the test drives.</summary>
    public Ex100_Gateway Gateway { get; } = new();

    /// <summary>Given. Do not change. One entry per error the command surfaced.</summary>
    public List<string> SurfacedErrors { get; } = [];

    private string _name = string.Empty;

    /// <summary>The field being validated. Must be non-empty and at most 20 characters.</summary>
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    /// <summary>Given. Do not change. Raised when validity changes.</summary>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>Given. Do not change. Call after Name changes.</summary>
    protected void RaiseErrorsChanged() =>
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Name)));

    /// <summary>
    /// True when Name is empty or longer than 20 characters.
    /// </summary>
    public bool HasErrors =>
        throw new NotImplementedException("TODO: Ex100 - report whether Name is invalid");

    /// <summary>
    /// The errors for <paramref name="propertyName"/>: for Name, exactly one
    /// message - Ex100_Messages.Required when it is empty, or Ex100_Messages.TooLong
    /// when it is over 20 characters. Nothing at all for any other property, and
    /// nothing when Name is valid.
    /// </summary>
    public IEnumerable GetErrors(string? propertyName) =>
        throw new NotImplementedException(
            "TODO: Ex100 - yield the one message that applies to Name, and nothing " +
            "for other properties or a valid Name");

    /// <summary>
    /// Saves through the Gateway and, ONLY on success, navigates to a fresh
    /// Ex100_DoneViewModel.
    ///
    /// It must not be executable while HasErrors is true - gate canExecute on the
    /// validity, observing it from Name changing - and any exception the save
    /// throws must end up as a message in SurfacedErrors rather than going
    /// unobserved.
    ///
    /// Called from the constructor, which is given.
    /// </summary>
    public ReactiveCommand<RxVoid, RxVoid> Save { get; private set; } = null!;

    private void Wire() =>
        throw new NotImplementedException(
            "TODO: Ex100 - build Save as ReactiveCommand.CreateFromTask awaiting " +
            "Gateway.SaveAsync(Name) on Sequencer.CurrentThread, with a canExecute " +
            "observable that is false " +
            "while HasErrors; subscribe to its output to navigate to a new " +
            "Ex100_DoneViewModel(this), and to its ThrownExceptions to append the " +
            "exception's Message to SurfacedErrors");

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
