using FeWoLearning.Wpf.Exercises.Beginner;

namespace FeWoLearning.Wpf.Tests.Beginner;

public class Ex010_DependentPropertyFanOutTests : WpfTestContext
{
    private static List<string?> Record(Ex010_TemperatureViewModel model)
    {
        var names = new List<string?>();
        model.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        return names;
    }

    // Unlike ex003, there is no separate "ready to use" wrapper standing between these
    // tests and the subject: Celsius's setter is the only place the fan-out can live, so
    // every test below already exercises it directly rather than through a collaborator.

    [WpfFact]
    public void Setting_A_New_Value_Raises_All_Three_Names_In_That_Order()
    {
        var model = new Ex010_TemperatureViewModel();
        var names = Record(model);

        model.Celsius = 100;

        Assert.Equal(new string?[] { "Celsius", "Fahrenheit", "IsFreezing" }, names);
    }

    [WpfFact]
    public void Fahrenheit_And_IsFreezing_Are_Already_Correct_By_The_Time_Their_Events_Fire()
    {
        var model = new Ex010_TemperatureViewModel();
        var seenDuringFahrenheitEvent = double.NaN;
        var seenDuringIsFreezingEvent = true;

        model.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Ex010_TemperatureViewModel.Fahrenheit))
            {
                seenDuringFahrenheitEvent = model.Fahrenheit;
            }

            if (e.PropertyName == nameof(Ex010_TemperatureViewModel.IsFreezing))
            {
                seenDuringIsFreezingEvent = model.IsFreezing;
            }
        };

        model.Celsius = 100;

        // A fan-out that raised the events before assigning the field (or that raised
        // them for the old value) would leave a handler reading stale data here.
        Assert.Equal(212.0, seenDuringFahrenheitEvent);
        Assert.False(seenDuringIsFreezingEvent);
    }

    [WpfFact]
    public void Assigning_An_Equal_Value_Raises_Nothing()
    {
        var model = new Ex010_TemperatureViewModel { Celsius = 20 };
        var names = Record(model);

        model.Celsius = 20;

        Assert.Empty(names);
    }

    [WpfFact]
    public void Zero_Counts_As_Freezing()
    {
        var model = new Ex010_TemperatureViewModel { Celsius = 5 };

        model.Celsius = 0;

        Assert.True(model.IsFreezing);
    }

    [WpfFact]
    public void Just_Above_Zero_Does_Not_Count_As_Freezing()
    {
        var model = new Ex010_TemperatureViewModel { Celsius = -5 };

        model.Celsius = 0.1;

        Assert.False(model.IsFreezing);
    }

    [WpfFact]
    public void Two_Different_Values_In_A_Row_Each_Raise_Their_Own_Full_Fan_Out()
    {
        var model = new Ex010_TemperatureViewModel();
        var names = Record(model);

        model.Celsius = 10;
        model.Celsius = -10;

        Assert.Equal(
            new string?[] { "Celsius", "Fahrenheit", "IsFreezing", "Celsius", "Fahrenheit", "IsFreezing" },
            names);
    }

    [WpfFact]
    public void Fahrenheit_Reflects_The_Standard_Conversion()
    {
        var model = new Ex010_TemperatureViewModel { Celsius = 100 };

        Assert.Equal(212.0, model.Fahrenheit);
    }
}
