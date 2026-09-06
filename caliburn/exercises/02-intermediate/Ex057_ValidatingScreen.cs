// Exercise 057 - Validating Screen (intermediate).
// Goal:   A screen's CanCloseAsync does not have to come from an externally-toggled flag (ex033,
//         ex034, ex052 all set RefuseClose directly from the test) - it can just as well be
//         DERIVED from the screen's own validation state, recomputed fresh every time it is
//         asked. No framework surprise here: this composes what earlier exercises already
//         established about CanCloseAsync (it is a plain virtual Task<bool> method, asked
//         whenever something wants to close the screen) with ordinary validation logic you write
//         yourself. Measured on this machine (Caliburn.Micro 5.0.258): a Screen with no Parent and
//         no attached view never has its CanCloseAsync invoked by TryCloseAsync at all (there is
//         nothing there to close it INTO) - so this exercise asks CanCloseAsync directly, and
//         through a close-request method that must consult it, rather than relying on
//         TryCloseAsync's own plumbing, which needs a conductor or a window neither exercise here
//         has.
// Drills: writing a computed HasValidationErrors that inspects the screen's own state (not a
//         flag the test flips), overriding CanCloseAsync to gate on it, and writing a
//         RequestCloseAsync that ACTS on the guard's answer instead of merely returning it -
//         a close attempt while invalid must be refused in fact, not just reported as refused.
// Passes: dotnet test --filter FullyQualifiedName~Ex057_

using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

/// <summary>A screen that edits Age and refuses to close while Age is not a plausible human
/// age - validation gates CanCloseAsync, and CanCloseAsync in turn gates RequestCloseAsync.</summary>
public class Ex057_ValidatingScreen : Screen
{
    int _age;

    /// <summary>The value being edited. Plausible range: 1..130 inclusive.</summary>
    public int Age { get => _age; set => Set(ref _age, value); }

    /// <summary>How many times RequestCloseAsync actually succeeded in closing.</summary>
    public int ClosedCount { get; private set; }

    /// <summary>True when Age is outside the plausible 1..130 range.</summary>
    public bool HasValidationErrors =>
        throw new NotImplementedException("TODO: Ex057 - report whether Age is outside the plausible 1..130 range");

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException("TODO: Ex057 - refuse to close while HasValidationErrors is true");

    /// <summary>Simulates a Close command/handler: asks CanCloseAsync first, and only counts
    /// itself as closed when that guard actually permits it.</summary>
    public Task<bool> RequestCloseAsync() =>
        throw new NotImplementedException("TODO: Ex057 - ask CanCloseAsync; only bump ClosedCount and return true when it allows closing");
}
