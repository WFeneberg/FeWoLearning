using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Tests;

/// <summary>
/// Fixture for the harness smoke test only. Never an exercise. The property is UserName,
/// not Name: an element named "Name" generates a field that hides FrameworkElement.Name
/// and the build warns CS0108.
/// </summary>
public class HarnessProbeViewModel : PropertyChangedBase
{
    string _userName = "Ada";

    public string UserName
    {
        get => _userName;
        set
        {
            if (Set(ref _userName, value)) NotifyOfPropertyChange(nameof(CanSayHello));
        }
    }

    /// <summary>Caliburn's guard convention: gates the IsEnabled of the SayHello button.</summary>
    public bool CanSayHello => UserName.Length > 3;

    public int Greetings { get; private set; }

    public void SayHello() => Greetings++;
}
