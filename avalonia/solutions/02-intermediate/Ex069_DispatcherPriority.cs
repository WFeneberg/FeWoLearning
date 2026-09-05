using System.Collections.Generic;
using Avalonia.Threading;

namespace FeWoLearning.Avalonia.Exercises.Intermediate;

// Passes: dotnet test --filter FullyQualifiedName~Ex069_
public class Ex069_DispatcherPriority
{
    /// <summary>Given. Do not change.</summary>
    public List<string> Log { get; } = [];

    public void PostAll()
    {
        // Posted deliberately in the wrong order, so that the run order below is
        // the dispatcher's doing and not this method's.
        Dispatcher.UIThread.Post(() => Log.Add("background"), DispatcherPriority.Background);
        Dispatcher.UIThread.Post(() => Log.Add("normal"), DispatcherPriority.Normal);
        Dispatcher.UIThread.Post(() => Log.Add("render"), DispatcherPriority.Render);
        Dispatcher.UIThread.Post(() => Log.Add("send"), DispatcherPriority.Send);
    }
}
