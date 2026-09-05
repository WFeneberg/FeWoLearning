using System.Windows.Controls;
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex015_NameTransformerRuleTests : CaliburnViewContext
{
    [WpfFact]
    public void Before_Registering_The_Rule_The_Model_Cannot_Be_Found()
    {
        // The harness resets ViewLocator.NameTransformer before every test - this is the
        // pristine baseline the rest of this exercise builds on.
        Assert.Equal(4, ViewLocator.NameTransformer.Count);

        var subject = new Ex015_NameTransformerRule();
        var view = subject.Locate(new Ex015_ReportPresenter());

        Assert.IsType<TextBlock>(view);
    }

    [WpfFact]
    public void RegisterPresenterRule_Adds_Exactly_One_Rule()
    {
        var subject = new Ex015_NameTransformerRule();

        subject.RegisterPresenterRule();

        Assert.Equal(5, ViewLocator.NameTransformer.Count);
    }

    [WpfFact]
    public void After_Registering_The_Rule_Locate_Resolves_The_Model_To_Its_View()
    {
        var subject = new Ex015_NameTransformerRule();

        subject.RegisterPresenterRule();
        var view = subject.Locate(new Ex015_ReportPresenter());

        Assert.IsType<Ex015_ReportView>(view);
    }

    [WpfFact]
    public void One_RegisterPresenterRule_Call_Resolves_Every_Presenter_Suffixed_Model_Not_Just_One()
    {
        var subject = new Ex015_NameTransformerRule();

        subject.RegisterPresenterRule();

        // A rule hard-coded to Ex015_ReportPresenter's exact name (or to Ex015_ReportView's)
        // would satisfy the first assertion below and stop there - this second, unrelated
        // Presenter-suffixed model is what forces a genuine, general rule instead.
        Assert.IsType<Ex015_ReportView>(subject.Locate(new Ex015_ReportPresenter()));
        Assert.IsType<Ex015_SummaryView>(subject.Locate(new Ex015_SummaryPresenter()));
    }

    [WpfFact]
    public void The_New_Rule_Does_Not_Make_An_Unrelated_Missing_Model_Findable_Too()
    {
        var subject = new Ex015_NameTransformerRule();
        subject.RegisterPresenterRule();

        // Not a "Presenter"-suffixed model at all - the new rule must not have turned into a
        // catch-all that finds a placeholder-worthy view for it.
        var view = subject.Locate(new UnrelatedNotAViewModelAtAll());

        Assert.IsType<TextBlock>(view);
    }

    [WpfFact]
    public void RegisterPresenterRule_Leaves_The_Default_Convention_Working_Too()
    {
        var subject = new Ex015_NameTransformerRule();
        // The default "...ViewModel" -> "...View" convention works before the new rule exists...
        Assert.IsType<StillWorksView>(subject.Locate(new StillWorksViewModel()));

        subject.RegisterPresenterRule();

        // ...and must still work after - AddRule appends, it does not replace the built-ins.
        Assert.IsType<StillWorksView>(subject.Locate(new StillWorksViewModel()));
    }

    public class UnrelatedNotAViewModelAtAll;
}

// Top-level, not nested: the default suffix convention (measured in ex013) resolves by
// building a "+"-free candidate type name, which a nested class's "Outer+Inner" full name does
// not satisfy.
public class StillWorksViewModel;

public class StillWorksView : UserControl;
