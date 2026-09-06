namespace FeWoLearning.Architecture.Exercises.Domain.Ex082;

/// <summary>
/// Where ids come from. A port, because "who assigns the id" is a design decision and
/// not a detail - see the header comment below.
/// </summary>
public interface IIdGenerator
{
    string Next();
}

// Exercise 082 — EntityIdentity (reference solution).
public sealed class Customer : IEquatable<Customer>
{
    public string Id { get; }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public Customer(IIdGenerator ids, string name, string email)
    {
        ArgumentNullException.ThrowIfNull(ids);

        // Assigned NOW, in memory. An entity with no id until the database supplies one
        // cannot be put in a set, referenced by an aggregate created in the same
        // transaction, published in an outbox message, or logged about - for the entire
        // time it is most interesting.
        Id = ids.Next();
        Name = name;
        Email = email;
    }

    public void Rename(string name) => Name = name;

    // Identity, not contents. A customer who changes their name is the same customer -
    // and the opposite reading produces a HashSet that loses entities the moment anybody
    // edits one.
    public bool Equals(Customer? other) => other is not null && other.Id == Id;

    public override bool Equals(object? obj) => Equals(obj as Customer);

    // The id and nothing else. A hash over mutable fields moves the entity to a different
    // bucket when it changes, and the set that contained it no longer finds it - while
    // still holding it.
    public override int GetHashCode() => Id.GetHashCode(StringComparison.Ordinal);
}
