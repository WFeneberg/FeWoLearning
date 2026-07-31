namespace FeWoLearning.Exercises.Beginner;

// Exercise 031 — Animal Inheritance (reference solution).
public abstract class Animal
{
    public abstract string Speak();
}

public class Dog : Animal
{
    public override string Speak() => "Woof";
}

public class Cat : Animal
{
    public override string Speak() => "Meow";
}
