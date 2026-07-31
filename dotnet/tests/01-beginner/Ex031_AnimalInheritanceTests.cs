using FeWoLearning.Exercises.Beginner;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Beginner;

public class Ex031_AnimalInheritanceTests
{
    [Fact]
    public void Dog_Speak_ReturnsWoof()
    {
        Animal animal = new Dog();
        Assert.Equal("Woof", animal.Speak());
    }

    [Fact]
    public void Cat_Speak_ReturnsMeow()
    {
        Animal animal = new Cat();
        Assert.Equal("Meow", animal.Speak());
    }

    [Fact]
    public void Animals_PolymorphicSpeak_ReturnExpectedSounds()
    {
        Animal[] animals = { new Dog(), new Cat() };

        var sounds = new string[animals.Length];
        for (int i = 0; i < animals.Length; i++)
        {
            sounds[i] = animals[i].Speak();
        }

        Assert.Equal(new[] { "Woof", "Meow" }, sounds);
    }
}
