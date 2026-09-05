using Bunit;
using FeWoLearning.Blazor.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

using OrderModel = Ex077_CustomValidatorComponent_Form.OrderModel;

public class Ex077_CustomValidatorComponentTests : BunitContext
{
    private readonly OrderModel _model = new() { Sku = "abc", Note = "far too long" };

    private IRenderedComponent<Ex077_CustomValidatorComponent_Form> RenderForm(bool attached = true)
        => Render<Ex077_CustomValidatorComponent_Form>(p => p
            .Add(c => c.Model, _model)
            .Add(c => c.ValidatorAttached, attached));

    private static string Messages(IRenderedComponent<Ex077_CustomValidatorComponent_Form> cut, string field)
        => cut.Find($"#{field}-messages").TextContent;

    [Fact]
    public void A_Full_Validation_Pass_Reports_Every_Broken_Rule()
    {
        var cut = RenderForm();

        cut.Find("#submit").Click();

        cut.WaitForAssertion(() => Assert.Equal(Ex077_CustomValidatorComponent.SkuError, Messages(cut, "sku")));
        Assert.Equal(Ex077_CustomValidatorComponent.NoteError, Messages(cut, "note"));
    }

    // The second event: a field that changes is re-checked there and then, without
    // anyone asking for a whole-form pass.
    [Fact]
    public void Editing_A_Field_Revalidates_It_Immediately()
    {
        var cut = RenderForm();

        cut.Find("#sku").Change("AB12");

        cut.WaitForAssertion(() => Assert.Equal("", Messages(cut, "sku")));
    }

    // Ruling: this is the row's whole point. Both messages live in one store, so a
    // handler that calls store.Clear() instead of store.Clear(field) takes the Note
    // message down as collateral - and nothing re-adds it until the next full pass,
    // so the form silently claims Note is fine when it is not.
    [Fact]
    public void Fixing_One_Field_Leaves_The_Other_Fields_Verdict_Alone()
    {
        var cut = RenderForm();
        cut.Find("#submit").Click();
        cut.WaitForAssertion(() => Assert.Equal(Ex077_CustomValidatorComponent.NoteError, Messages(cut, "note")));

        cut.Find("#sku").Change("AB12");

        cut.WaitForAssertion(() => Assert.Equal("", Messages(cut, "sku")));
        Assert.Equal(Ex077_CustomValidatorComponent.NoteError, Messages(cut, "note"));
    }

    // Removing the validator from the tree disposes it; its verdicts must go with it,
    // and its handlers must stop running. Without the unsubscribe the store is still
    // wired to the context and the message comes straight back on the next change.
    [Fact]
    public void Removing_The_Validator_Takes_Its_Messages_With_It()
    {
        var cut = RenderForm();
        cut.Find("#submit").Click();
        cut.WaitForAssertion(() => Assert.Equal(Ex077_CustomValidatorComponent.SkuError, Messages(cut, "sku")));

        cut.Render(p => p.Add(c => c.Model, _model).Add(c => c.ValidatorAttached, false));

        Assert.Equal("", Messages(cut, "sku"));
        Assert.Equal("", Messages(cut, "note"));

        cut.Find("#sku").Change("still-bad");
        cut.WaitForAssertion(() => Assert.Equal("still-bad", _model.Sku));
        Assert.Equal("", Messages(cut, "sku"));
    }
}
