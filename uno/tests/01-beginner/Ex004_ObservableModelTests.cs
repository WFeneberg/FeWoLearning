using FeWoLearning.Uno.Exercises.Beginner;

namespace FeWoLearning.Uno.Tests.Beginner;

public class Ex004_ObservableModelTests : UnoTestContext
{
    private static List<string?> Record(Ex004_ObservableModel model)
    {
        var names = new List<string?>();
        model.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        return names;
    }

    [Fact]
    public void Setting_A_Property_Stores_The_Value()
    {
        var model = new Ex004_ObservableModel { Name = "Ada", Age = 36 };

        Assert.Equal("Ada", model.Name);
        Assert.Equal(36, model.Age);
    }

    [Fact]
    public void Setting_A_Property_Announces_It_By_Name()
    {
        var model = new Ex004_ObservableModel();
        var names = Record(model);

        model.Name = "Ada";

        Assert.Contains("Name", names);
    }

    [Fact]
    public void Writing_The_Same_Value_Announces_Nothing()
    {
        var model = new Ex004_ObservableModel { Name = "Ada", Age = 36 };
        var names = Record(model);

        model.Name = "Ada";
        model.Age = 36;

        // Every redundant notification re-evaluates bindings and re-runs converters.
        Assert.Empty(names);
    }

    [Fact]
    public void Computed_Summary_Is_Announced_When_Its_Inputs_Move()
    {
        var model = new Ex004_ObservableModel();
        var names = Record(model);

        model.Name = "Ada";
        model.Age = 36;

        // Summary has no setter, so nothing else can announce it - a binding to Summary
        // would go stale without this.
        Assert.Equal(2, names.Count(n => n == nameof(Ex004_ObservableModel.Summary)));
        Assert.Equal("Ada (36)", model.Summary);
    }

    [Fact]
    public void Announces_Names_That_Actually_Exist_On_The_Type()
    {
        var model = new Ex004_ObservableModel();
        var names = Record(model);

        model.Name = "Ada";
        model.Age = 36;

        // A typo in a hand-written string literal fails silently at runtime; the compiler
        // never sees it. [CallerMemberName] is how you stop writing them.
        Assert.All(names, name =>
        {
            Assert.False(string.IsNullOrEmpty(name), "PropertyChanged must name the property that changed.");
            Assert.NotNull(typeof(Ex004_ObservableModel).GetProperty(name!));
        });
    }
}
