using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FeWoLearning.Architecture.Exercises.Desktop.Ex017;

// Exercise 017 — MvvmComposition (reference solution).
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Raise([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        // The equality check is not an optimisation. Raising unconditionally makes every
        // two-way binding a potential feedback loop, and turns "the user typed nothing"
        // into a change event that a dirty-tracking flag will happily believe.
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        Raise(propertyName);
        return true;
    }
}

public sealed class OrderViewModel : ObservableObject
{
    private string _customerName = "";
    private int _quantity;
    private decimal _unitPrice;

    public string CustomerName
    {
        get => _customerName;
        set => SetProperty(ref _customerName, value);
    }

    public int Quantity
    {
        get => _quantity;
        // Guarded by the return value, so a no-op set stays completely silent - Total
        // included.
        set { if (SetProperty(ref _quantity, value)) Raise(nameof(Total)); }
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set { if (SetProperty(ref _unitPrice, value)) Raise(nameof(Total)); }
    }

    public decimal Total => Quantity * UnitPrice;
}
