namespace FeWoLearning.Exercises.Beginner;

// Exercise 031 — Animal Inheritance (beginner).
// Goal:   Define an abstract Animal class with an abstract Speak method,
//         implemented by Dog and Cat subclasses. Polymorphic calls through
//         an Animal reference should return the correct sound per subclass.
// Drills: abstract class, inheritance, method overriding, polymorphism.
public abstract class Animal
{
    public abstract string Speak();
}

public class Dog : Animal
{
    public override string Speak() => throw new NotImplementedException();
}

public class Cat : Animal
{
    public override string Speak() => throw new NotImplementedException();
}
