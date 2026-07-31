namespace FeWoLearning.Exercises.Intermediate;

// Exercise 048 — RecordEqualityCheck (reference solution).
public static class RecordEqualityCheck
{
    public record Address(string Street, string City);

    public record Person(string Name, Address Address);

    public static Person WithCity(Person person, string newCity) =>
        person with { Address = person.Address with { City = newCity } };

    public static bool AreEqual(Person left, Person right) => left == right;
}
