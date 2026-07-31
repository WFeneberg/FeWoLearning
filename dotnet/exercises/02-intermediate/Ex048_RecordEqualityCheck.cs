namespace FeWoLearning.Exercises.Intermediate;

// Exercise 048 — RecordEqualityCheck (intermediate).
// Goal:   Model a Person record with a nested Address record, produce a modified
//         copy using a with-expression, and reason about record value semantics
//         (structural equality) versus reference semantics.
// Drills: records, nested records, with-expressions, structural equality (Equals/==),
//         GetHashCode consistency.
public static class RecordEqualityCheck
{
    public record Address(string Street, string City);

    public record Person(string Name, Address Address);

    // Returns a copy of "person" with the same Name but a different City in the
    // nested Address (Street unchanged).
    public static Person WithCity(Person person, string newCity) => throw new NotImplementedException();

    // Returns true if the two people are structurally equal (records + nested
    // records compare value-by-value), false otherwise.
    public static bool AreEqual(Person left, Person right) => throw new NotImplementedException();
}
