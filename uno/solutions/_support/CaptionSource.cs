using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Uno.Support;

/// <summary>Test fixture: a minimal observable source to bind against. Not an exercise.</summary>
public sealed class CaptionSource : INotifyPropertyChanged
{
    private string _caption = "";
    private int _count;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Caption
    {
        get => _caption;
        set => Set(ref _caption, value);
    }

    public int Count
    {
        get => _count;
        set => Set(ref _count, value);
    }

    private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
