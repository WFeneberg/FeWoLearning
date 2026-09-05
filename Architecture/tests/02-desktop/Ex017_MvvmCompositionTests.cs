using System.ComponentModel;
using FeWoLearning.Architecture.Exercises.Desktop.Ex017;

namespace FeWoLearning.Architecture.Tests.Desktop;

public class Ex017_MvvmCompositionTests
{
    private static (OrderViewModel ViewModel, List<string> Raised) Build()
    {
        var viewModel = new OrderViewModel();
        var raised = new List<string>();
        viewModel.PropertyChanged += (_, e) => raised.Add(e.PropertyName ?? "");
        return (viewModel, raised);
    }

    [Fact]
    public void A_Changed_Property_Raises_Once_Naming_Itself()
    {
        var (viewModel, raised) = Build();

        viewModel.CustomerName = "Ada";

        Assert.Equal([nameof(OrderViewModel.CustomerName)], raised);
    }

    [Fact]
    public void Adversarial_Setting_The_Same_Value_Raises_Nothing()
    {
        // Raising unconditionally is the easy implementation and it passes the fact
        // above. It also makes every two-way binding a potential feedback loop, and
        // turns "the user typed nothing" into a change a dirty-tracking flag believes.
        var (viewModel, raised) = Build();

        viewModel.CustomerName = "Ada";
        raised.Clear();

        viewModel.CustomerName = "Ada";

        Assert.Empty(raised);
    }

    [Fact]
    public void Mechanism_Setting_A_Source_Property_Also_Announces_The_Derived_One()
    {
        // The bug every MVVM codebase has at least once. Total is correct the entire
        // time; it is simply never re-read, because nobody told the binding it changed.
        // It presents as a rendering bug and it is a notification bug.
        var (viewModel, raised) = Build();

        viewModel.Quantity = 3;

        Assert.Contains(nameof(OrderViewModel.Quantity), raised);
        Assert.Contains(nameof(OrderViewModel.Total), raised);
    }

    [Fact]
    public void The_Derived_Property_Actually_Recomputes()
    {
        // Pairs with the notification fact: announcing a change to a value that never
        // changes is its own kind of lie.
        var (viewModel, _) = Build();

        viewModel.Quantity = 3;
        viewModel.UnitPrice = 2.5m;

        Assert.Equal(7.5m, viewModel.Total);
    }

    [Fact]
    public void Adversarial_A_No_Op_Set_Stays_Silent_About_The_Derived_Property_Too()
    {
        // Catches raising Total unconditionally from the setter while guarding only the
        // source property - an easy half-fix that passes both facts above.
        var (viewModel, raised) = Build();

        viewModel.Quantity = 3;
        raised.Clear();

        viewModel.Quantity = 3;

        Assert.Empty(raised);
    }

    [Fact]
    public void The_View_Model_Needs_Nothing_But_System_ComponentModel()
    {
        // Documents the constraint the whole desktop block is built on: this is a
        // desktop architecture exercise with no desktop framework anywhere in it.
        var (viewModel, _) = Build();

        // The assignment is not decoration. Everything below it asserts type metadata,
        // which is true of the STUB as well - so without a call into the exercise this
        // fact would pass on the untouched tree and grade nothing.
        viewModel.CustomerName = "Ada";

        Assert.IsAssignableFrom<INotifyPropertyChanged>(viewModel);
        Assert.All(
            viewModel.GetType().Assembly.GetReferencedAssemblies(),
            a => Assert.DoesNotContain("PresentationFramework", a.Name ?? ""));
    }
}
