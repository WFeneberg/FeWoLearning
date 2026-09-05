// Exercise 012 - Legacy event to INotifyPropertyChanged (beginner).
// Goal:   Migrate a screen off a bespoke "XChanged" event - the kind every WinForms-era
//         model has - onto the interface WPF's binding engine actually understands.
// Drills: replacing (or rather, sitting alongside) a hand-rolled XChanged event with
//         INotifyPropertyChanged, and the fact that a plain custom event, no matter
//         how symmetrical it looks, is invisible to a real Binding - only
//         PropertyChanged makes one refresh.
// Passes: dotnet test --filter FullyQualifiedName~Ex012_

using System.ComponentModel;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public class Ex012_VolumeControl : INotifyPropertyChanged
{
    // Explicit "= 0" initializer: the setter below throws before it ever assigns this
    // field, and without an initializer that makes the compiler warn CS0649 ("field is
    // never assigned"). Same value it would have had anyway - this is a warning
    // workaround, not part of the exercise, same as ex010's _celsius.
    private int _volume = 0;

    /// <summary>
    /// The legacy notification old, pre-binding code still subscribes to. Ready to use -
    /// keep raising it, do not delete it. It is not what makes a WPF <c>Binding</c> work;
    /// that is the whole point of this exercise.
    /// </summary>
    public event EventHandler? VolumeChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int Volume
    {
        get => _volume;
        set
        {
            // TODO: if value equals the current _volume, do nothing at all - no field
            // write, no events, exactly like ex003/ex010. Otherwise assign _volume,
            // then call BOTH RaiseVolumeChanged() - for the legacy subscriber that
            // still exists - AND RaisePropertyChanged(nameof(Volume)). A real WPF
            // Binding only ever looks at PropertyChanged; VolumeChanged firing on its
            // own would leave any bound target stale.
            throw new NotImplementedException("TODO: Ex012 - raise INotifyPropertyChanged alongside the legacy VolumeChanged event");
        }
    }

    /// <summary>Raises the legacy <see cref="VolumeChanged"/> event. Ready to use.</summary>
    protected void RaiseVolumeChanged() => VolumeChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>Raises <see cref="PropertyChanged"/>. Ready to use.</summary>
    protected void RaisePropertyChanged(string? propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
