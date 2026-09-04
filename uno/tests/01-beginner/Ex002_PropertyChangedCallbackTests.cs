using FeWoLearning.Uno.Exercises.Beginner;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex002_PropertyChangedCallbackTests : UnoTestContext
{
    [Fact]
    public void Records_The_First_Change_Against_The_Default()
    {
        var gauge = new Ex002_PropertyChangedCallback();

        // Construction is not a change: the default value was never assigned.
        Assert.Empty(gauge.Transitions);

        gauge.Level = 3;

        Assert.Equal(["0->3"], gauge.Transitions);
    }

    [Fact]
    public void Records_Every_Change_In_Order()
    {
        var gauge = new Ex002_PropertyChangedCallback();

        gauge.Level = 3;
        gauge.Level = 4;
        gauge.Level = 1;

        Assert.Equal(["0->3", "3->4", "4->1"], gauge.Transitions);
    }

    [Fact]
    public void Writing_The_Same_Value_Is_Not_A_Change()
    {
        var gauge = new Ex002_PropertyChangedCallback();

        gauge.Level = 3;
        gauge.Level = 3;
        gauge.Level = 3;

        // The framework compares before it calls back, so the callback does not have to.
        Assert.Equal(["0->3"], gauge.Transitions);
    }

    [Fact]
    public void Also_Fires_For_Writes_Through_SetValue()
    {
        var gauge = new Ex002_PropertyChangedCallback();

        gauge.SetValue(Ex002_PropertyChangedCallback.LevelProperty, 8);

        // Proof the hook sits in the property metadata rather than in the CLR setter.
        Assert.Equal(["0->8"], gauge.Transitions);
    }
}
