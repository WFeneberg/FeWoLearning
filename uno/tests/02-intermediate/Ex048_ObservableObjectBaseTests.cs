using FeWoLearning.Uno.Exercises.Intermediate;

namespace FeWoLearning.Uno.Tests.Intermediate;

public class Ex048_ObservableObjectBaseTests : UnoTestContext
{
    private static (Ex048_Person Person, List<string?> Names) Recorded()
    {
        var person = new Ex048_Person();
        var names = new List<string?>();
        person.PropertyChanged += (_, e) => names.Add(e.PropertyName);
        return (person, names);
    }

    [Fact]
    public void Stores_The_Value()
    {
        var person = new Ex048_Person { First = "Ada", Last = "Lovelace" };

        Assert.Equal("Ada", person.First);
        Assert.Equal("Lovelace", person.Last);
    }

    [Fact]
    public void Announces_The_Property_By_Its_Own_Name()
    {
        var (person, names) = Recorded();

        person.First = "Ada";

        // [CallerMemberName] filled this in - no string literal in the setter.
        Assert.Contains(nameof(Ex048_Person.First), names);
    }

    [Fact]
    public void Announces_The_Dependent_Property_Too()
    {
        var (person, names) = Recorded();

        person.First = "Ada";

        Assert.Contains(nameof(Ex048_Person.FullName), names);
    }

    [Fact]
    public void Writing_The_Same_Value_Announces_Nothing()
    {
        var person = new Ex048_Person { First = "Ada" };
        var names = new List<string?>();
        person.PropertyChanged += (_, e) => names.Add(e.PropertyName);

        person.First = "Ada";

        // Not even the dependents: a guard that assigns first and announces anyway is the
        // usual half-fix, and it re-runs every converter bound to FullName.
        Assert.Empty(names);
    }

    [Fact]
    public void Both_Setters_Feed_The_Computed_Property()
    {
        var person = new Ex048_Person { First = "Ada", Last = "Lovelace" };

        Assert.Equal("Ada Lovelace", person.FullName);
    }

    [Fact]
    public void The_Dependent_Property_Is_Announced_Once_Per_Change()
    {
        var (person, names) = Recorded();

        person.First = "Ada";
        person.Last = "Lovelace";

        Assert.Equal(2, names.Count(n => n == nameof(Ex048_Person.FullName)));
    }

    [Fact]
    public void Announces_The_Property_Before_Its_Dependents()
    {
        var (person, names) = Recorded();

        person.First = "Ada";

        // Order matters to a UI that reads a second property inside the first one's
        // handler: the changed value must already be visible.
        Assert.Equal(nameof(Ex048_Person.First), names[0]);
        Assert.Equal(nameof(Ex048_Person.FullName), names[1]);
    }

    [Fact]
    public void Announces_Only_Names_That_Exist()
    {
        var (person, names) = Recorded();

        person.First = "Ada";
        person.Last = "Lovelace";

        Assert.All(names, name => Assert.NotNull(typeof(Ex048_Person).GetProperty(name!)));
    }
}
