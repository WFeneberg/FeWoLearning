// Exercise 004 - Dependent Properties (beginner).
// Goal:   Announce exactly the computed properties a setter actually moved - no more.
// Drills: chains of computed properties, announcing dependents from a setter, and not
//         announcing the ones that did not change.
// Passes: dotnet test --filter FullyQualifiedName~Ex004_
//
// Subtotal <- Quantity, UnitPrice
// Discount <- Subtotal, DiscountPercent
// Total    <- Subtotal, Discount

using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Beginner;

public class Ex004_DependentProperties : PropertyChangedBase
{
    private int _quantity = 1;
    private decimal _unitPrice;
    private decimal _discountPercent;

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (Set(ref _quantity, value)) NotifySubtotalChanged();
        }
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            if (Set(ref _unitPrice, value)) NotifySubtotalChanged();
        }
    }

    public decimal DiscountPercent
    {
        get => _discountPercent;
        set
        {
            // Deliberately not NotifySubtotalChanged: a discount does not move Subtotal.
            if (Set(ref _discountPercent, value))
            {
                NotifyOfPropertyChange(nameof(Discount));
                NotifyOfPropertyChange(nameof(Total));
            }
        }
    }

    public decimal Subtotal => Quantity * UnitPrice;

    public decimal Discount => Subtotal * DiscountPercent / 100m;

    public decimal Total => Subtotal - Discount;

    /// <summary>Subtotal moved, so everything downstream of it moved with it.</summary>
    private void NotifySubtotalChanged()
    {
        NotifyOfPropertyChange(nameof(Subtotal));
        NotifyOfPropertyChange(nameof(Discount));
        NotifyOfPropertyChange(nameof(Total));
    }
}
