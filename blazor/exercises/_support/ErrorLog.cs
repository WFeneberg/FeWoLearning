namespace FeWoLearning.Blazor.Support;

/// <summary>
/// Test fixture for ex067: what a custom ErrorBoundary subclass logs into. Records
/// the exceptions themselves rather than a count, so a test asserts on what was
/// actually handed over. Not an exercise.
/// </summary>
public sealed class ErrorLog
{
    private readonly List<Exception> _entries = [];

    public IReadOnlyList<Exception> Entries => _entries;

    public void Record(Exception exception) => _entries.Add(exception);
}
