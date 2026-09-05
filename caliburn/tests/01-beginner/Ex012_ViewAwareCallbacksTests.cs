using System.Windows.Controls;
using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Beginner;

namespace FeWoLearning.Caliburn.Tests.Beginner;

public class Ex012_ViewAwareCallbacksTests : CaliburnViewContext
{
    [WpfFact]
    public void AttachView_Fires_OnViewAttached_Immediately_And_Synchronously()
    {
        var vm = new Ex012_ViewAwareCallbacks();
        var view = new Grid();

        ((IViewAware)vm).AttachView(view, null);

        Assert.Equal(1, vm.AttachCount);
        Assert.Same(view, vm.LastAttachedView);
        Assert.Null(vm.LastAttachedContext);
        // No window at all yet - attaching must not, by itself, count as loading.
        Assert.Equal(0, vm.LoadCount);
    }

    [WpfFact]
    public void OnViewLoaded_Does_Not_Fire_On_Layout_Alone_Only_On_A_Real_Show()
    {
        var vm = new Ex012_ViewAwareCallbacks();
        var view = new Grid();
        ((IViewAware)vm).AttachView(view, null);

        // Measure/Arrange is enough for geometry, not enough to be "loaded".
        Layout(view);
        Assert.Equal(0, vm.LoadCount);

        Show(view);

        Assert.Equal(1, vm.LoadCount);
        Assert.Same(view, vm.LastLoadedView);
    }

    [WpfFact]
    public void GetView_With_No_Context_Returns_The_View_Attached_Under_No_Context()
    {
        var vm = new Ex012_ViewAwareCallbacks();
        var view = new Grid();

        ((IViewAware)vm).AttachView(view, null);

        Assert.Same(view, ((IViewAware)vm).GetView());
    }

    [WpfFact]
    public void Views_Are_Stored_Keyed_By_Context_Not_Visible_Under_A_Different_Key()
    {
        var vm = new Ex012_ViewAwareCallbacks();
        var view = new Grid();

        ((IViewAware)vm).AttachView(view, "Edit");

        Assert.Same(view, ((IViewAware)vm).GetView("Edit"));
        // Attached only under "Edit" - looking it up under no context must not find it.
        Assert.Null(((IViewAware)vm).GetView(null));
    }

    [WpfFact]
    public void AttachView_Called_Again_Fires_OnViewAttached_Again_For_The_New_View()
    {
        var vm = new Ex012_ViewAwareCallbacks();
        var first = new Grid();
        var second = new Grid();

        ((IViewAware)vm).AttachView(first, null);
        ((IViewAware)vm).AttachView(second, "Detail");

        Assert.Equal(2, vm.AttachCount);
        Assert.Same(second, vm.LastAttachedView);
        Assert.Equal("Detail", vm.LastAttachedContext);
    }
}
