// Exercise 004 - Dependent Properties (beginner).
// Goal:   Announce exactly the computed properties a setter actually moved - no more.
// Drills: chains of computed properties, announcing dependents from a setter, and not
//         announcing the ones that did not change.
// Passes: dotnet test --filter FullyQualifiedName~Ex004_
//
// Subtotal <- Quantity, UnitPrice
// Discount <- Subtotal, DiscountPercent
// Total    <- Subtotal, Discount
//
// Over-announcing is not free: every announcement re-evaluates each binding on that
// property and re-runs its converters. Announce what moved, and only what moved.

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
        // TODO: set via Set(...), and when it moved announce Subtotal, Discount and Total.
        set => throw new NotImplementedException("TODO: Ex004 - set _quantity and announce its dependents");
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set => throw new NotImplementedException("TODO: Ex004 - set _unitPrice and announce its dependents");
    }

    public decimal DiscountPercent
    {
        get => _discountPercent;
        // TODO: this one does NOT move Subtotal. Announce only what it really changed.
        set => throw new NotImplementedException("TODO: Ex004 - set _discountPercent and announce its dependents");
    }

    public decimal Subtotal => Quantity * UnitPrice;

    public decimal Discount => Subtotal * DiscountPercent / 100m;

    public decimal Total => Subtotal - Discount;
}
