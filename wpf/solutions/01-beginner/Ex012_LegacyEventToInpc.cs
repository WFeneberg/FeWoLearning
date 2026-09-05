// Exercise 012 - Legacy event to INotifyPropertyChanged (beginner). REFERENCE SOLUTION.
// Goal:   Migrate a screen off a bespoke "XChanged" event - the kind every WinForms-era
//         model has - onto the interface WPF's binding engine actually understands.
// Drills: replacing (or rather, sitting alongside) a hand-rolled XChanged event with
//         INotifyPropertyChanged, and the fact that a plain custom event, no matter
//         how symmetrical it looks, is invisible to a real Binding - only
//         PropertyChanged makes one refresh.

using System.ComponentModel;

namespace FeWoLearning.Wpf.Exercises.Beginner;

public class Ex012_VolumeControl : INotifyPropertyChanged
{
    private int _volume = 0;

    /// <summary>
    /// The legacy notification old, pre-binding code still subscribes to.
    /// </summary>
    public event EventHandler? VolumeChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int Volume
    {
        get => _volume;
        set
        {
            if (value == _volume)
            {
                return;
            }

            _volume = value;

            // Legacy subscribers still get their event...
            RaiseVolumeChanged();

            // ...but only this one is what a WPF Binding actually listens for.
            RaisePropertyChanged(nameof(Volume));
        }
    }

    /// <summary>Raises the legacy <see cref="VolumeChanged"/> event.</summary>
    protected void RaiseVolumeChanged() => VolumeChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>Raises <see cref="PropertyChanged"/>.</summary>
    protected void RaisePropertyChanged(string? propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
