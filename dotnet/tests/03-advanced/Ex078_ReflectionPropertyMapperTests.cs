using System;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex078_ReflectionPropertyMapperTests
{
    private class PersonDto
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public double Height { get; set; }
        public string Internal { get; set; } = "unused"; // no counterpart on the target
    }

    private class PersonEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public double Height { get; set; }
        public string Extra { get; set; } = "untouched"; // no counterpart on the source
    }

    private class MismatchedTarget
    {
        // Same name, incompatible type -> must be skipped, not throw.
        public string Age { get; set; } = "unset";
    }

    [Fact]
    public void Map_CreatesNewTarget_WithMatchingPropertiesCopied()
    {
        var source = new PersonDto { Name = "Ada", Age = 36, Height = 1.68, Internal = "secret" };

        var target = ReflectionPropertyMapper.Map<PersonDto, PersonEntity>(source);

        Assert.Equal(source.Name, target.Name);
        Assert.Equal(source.Age, target.Age);
        Assert.Equal(source.Height, target.Height);
        Assert.Equal("untouched", target.Extra); // unmatched target property left as-is
    }

    [Fact]
    public void Map_OntoExistingTarget_OverwritesMatchingProperties()
    {
        var source = new PersonDto { Name = "Grace", Age = 45, Height = 1.7 };
        var target = new PersonEntity { Name = "old", Age = 0, Height = 0, Extra = "kept" };

        ReflectionPropertyMapper.Map(source, target);

        Assert.Equal("Grace", target.Name);
        Assert.Equal(45, target.Age);
        Assert.Equal(1.7, target.Height);
        Assert.Equal("kept", target.Extra);
    }

    [Fact]
    public void Map_SkipsPropertiesWithIncompatibleTypes()
    {
        var source = new PersonDto { Name = "Linus", Age = 55, Height = 1.8 };

        var target = ReflectionPropertyMapper.Map<PersonDto, MismatchedTarget>(source);

        Assert.Equal("unset", target.Age); // int -> string is not assignable, so untouched
    }

    [Fact]
    public void Map_ThrowsOnNullSource()
        => Assert.Throws<ArgumentNullException>(
            () => ReflectionPropertyMapper.Map<PersonDto, PersonEntity>(null!));
}
