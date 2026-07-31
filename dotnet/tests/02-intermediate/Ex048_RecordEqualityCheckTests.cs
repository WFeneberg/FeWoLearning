using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex048_RecordEqualityCheckTests
{
    private static RecordEqualityCheck.Person MakePerson() =>
        new("Ada Lovelace", new RecordEqualityCheck.Address("10 Main St", "London"));

    [Fact]
    public void WithCity_ChangesOnlyCity()
    {
        var original = MakePerson();

        var moved = RecordEqualityCheck.WithCity(original, "Paris");

        Assert.Equal("Ada Lovelace", moved.Name);
        Assert.Equal("10 Main St", moved.Address.Street);
        Assert.Equal("Paris", moved.Address.City);
        Assert.Equal("London", original.Address.City);
    }

    [Fact]
    public void WithCity_ReturnsNewInstance_NotSameReference()
    {
        var original = MakePerson();

        var moved = RecordEqualityCheck.WithCity(original, "Paris");

        Assert.NotSame(original, moved);
        Assert.NotSame(original.Address, moved.Address);
    }

    [Fact]
    public void AreEqual_TwoStructurallyIdenticalInstances_AreEqual()
    {
        var a = new RecordEqualityCheck.Person("Ada Lovelace", new RecordEqualityCheck.Address("10 Main St", "London"));
        var b = new RecordEqualityCheck.Person("Ada Lovelace", new RecordEqualityCheck.Address("10 Main St", "London"));

        Assert.NotSame(a, b);
        Assert.True(RecordEqualityCheck.AreEqual(a, b));
        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void AreEqual_AfterWithExpressionChangesNestedValue_AreNotEqual()
    {
        var original = MakePerson();
        var moved = RecordEqualityCheck.WithCity(original, "Paris");

        Assert.False(RecordEqualityCheck.AreEqual(original, moved));
        Assert.NotEqual(original, moved);
    }

    [Fact]
    public void AreEqual_DifferentName_AreNotEqual()
    {
        var a = MakePerson();
        var b = a with { Name = "Grace Hopper" };

        Assert.False(RecordEqualityCheck.AreEqual(a, b));
    }
}
