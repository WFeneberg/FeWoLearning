using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex036_ParentChildRelationshipTests : CaliburnCoreContext
{
    [Fact]
    public async Task GetParentConductor_Returns_The_Conductor_That_Activated_The_Child()
    {
        var subject = new Ex036_ParentChildRelationship();
        var conductor = new Conductor<Ex036_Child>();
        await ((IActivate)conductor).ActivateAsync();
        var child = new Ex036_Child();
        await conductor.ActivateItemAsync(child);

        var parent = subject.GetParentConductor(child);

        Assert.Same(conductor, parent);
    }

    [Fact]
    public void GetParentConductor_Returns_Null_For_A_Child_Never_Activated_Into_Anything()
    {
        var subject = new Ex036_ParentChildRelationship();
        var child = new Ex036_Child();

        Assert.Null(subject.GetParentConductor(child));
    }

    [Fact]
    public async Task RequestCloseAsync_Closes_The_Child_Through_Its_Parent_Conductor()
    {
        var subject = new Ex036_ParentChildRelationship();
        var conductor = new Conductor<Ex036_Child>();
        await ((IActivate)conductor).ActivateAsync();
        var child = new Ex036_Child();
        await conductor.ActivateItemAsync(child);

        await subject.RequestCloseAsync(child);

        Assert.Null(conductor.ActiveItem);
        Assert.False(child.IsActive);
        Assert.Equal(1, child.DeactivateCount);
    }

    [Fact]
    public async Task RequestCloseAsync_Respects_A_Refusing_CanCloseAsync()
    {
        var subject = new Ex036_ParentChildRelationship();
        var conductor = new Conductor<Ex036_Child>();
        await ((IActivate)conductor).ActivateAsync();
        var child = new Ex036_Child { RefuseClose = true };
        await conductor.ActivateItemAsync(child);

        await subject.RequestCloseAsync(child);

        // A wrong implementation that bypasses the conductor's own close guard (e.g. removing
        // the item directly instead of going through DeactivateItemAsync) would let this
        // succeed - it does not.
        Assert.Same(child, conductor.ActiveItem);
        Assert.True(child.IsActive);
        Assert.Equal(0, child.DeactivateCount);
    }

    [Fact]
    public async Task RequestCloseAsync_Is_A_NoOp_For_A_Child_With_No_Parent_Conductor()
    {
        var subject = new Ex036_ParentChildRelationship();
        var child = new Ex036_Child();

        var ex = await Record.ExceptionAsync(() => subject.RequestCloseAsync(child));

        Assert.Null(ex);
        Assert.Equal(0, child.DeactivateCount);
    }
}
