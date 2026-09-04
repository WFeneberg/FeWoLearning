// Exercise 065 - Progress Reporting (intermediate).
// Goal:   Report progress out of a long operation without the operation knowing about a UI.
// Drills: IProgress<T> as the seam, Progress<T> against a synchronous implementation, and
//         clamping what a caller reports.
// Passes: dotnet test --filter FullyQualifiedName~Ex065_
//
// The operation takes an IProgress<T> and calls Report. That is the whole contract - and it
// is why the operation can be tested with a recorder and shipped against a UI. Note which
// side clamps: an operation that reports 110% is a bug in the operation, and a progress
// sink that renders 110% is a bug in the sink. Both check.

using System.ComponentModel;

namespace FeWoLearning.Uno.Exercises.Intermediate;

/// <summary>
/// A bindable progress sink: hand it to anything that takes an
/// <see cref="IProgress{T}"/> and bind a bar to <see cref="Percent"/>.
/// </summary>
public sealed class Ex065_ProgressReporting : IProgress<double>, INotifyPropertyChanged
{
    private double _percent;
    private bool _isRunning;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>The last reported value, clamped to 0..100.</summary>
    public double Percent
    {
        get => _percent;
        private set
        {
            if (_percent.Equals(value))
            {
                return;
            }

            _percent = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Percent)));
        }
    }

    /// <summary>True between <see cref="RunAsync"/> starting and finishing.</summary>
    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            if (_isRunning == value)
            {
                return;
            }

            _isRunning = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
        }
    }

    /// <summary>
    /// Records a report, clamped into 0..100. NaN is ignored - a division by zero upstream
    /// must not blank the bar.
    /// </summary>
    public void Report(double value)
    {
        if (double.IsNaN(value))
        {
            // "0 of 0 files" upstream. Clamping NaN would give 0 and blank a bar that was
            // making perfectly good progress.
            return;
        }

        Percent = Math.Clamp(value, 0, 100);
    }

    /// <summary>
    /// Runs <paramref name="work"/>, handing it this instance as the progress sink, and
    /// keeps <see cref="IsRunning"/> true for the duration - including when the work throws,
    /// in which case the exception is left to the caller.
    /// </summary>
    public async Task RunAsync(Func<IProgress<double>, Task> work)
    {
        IsRunning = true;

        try
        {
            // `this` is the sink: the operation only ever sees IProgress<double>, which is
            // what lets it be tested with a recorder and shipped against a UI.
            await work(this);
        }
        finally
        {
            // In a finally, and the exception is deliberately not caught: a progress sink
            // is not an error handler, but it must not leave a spinner running either.
            IsRunning = false;
        }
    }
}
