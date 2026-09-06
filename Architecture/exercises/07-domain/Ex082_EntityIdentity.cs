namespace FeWoLearning.Architecture.Exercises.Domain.Ex082;

/// <summary>
/// Where ids come from. A port, because "who assigns the id" is a design decision and
/// not a detail - see the header comment below.
/// </summary>
public interface IIdGenerator
{
    string Next();
}

// Exercise 082 — EntityIdentity (domain).
// Goal:   Give an entity an identity that is stable from the moment it exists, and equality
//         that follows the identity rather than the contents.
// Drills: identity vs equality, id generation as a port, the unsaved entity.
// Passes: identity  - two Customers with the same Id are EQUAL even when every other
//                     field differs. A customer who changes their name is the same
//                     customer.
//         difference- two Customers with different Ids are NOT equal even when every
//                     other field matches. Two people called John Smith are two people.
//         hashing   - equal entities hash alike, and the hash uses ONLY the id - so
//                     renaming a customer does not lose it inside a HashSet.
//         THE ONE    - the id exists as soon as the entity does, from the generator. An
//                     entity that has no id until the database gives it one cannot be put
//                     in a set, referenced by another aggregate, or logged about, for the
//                     entire time it is most interesting.
//         generation- the entity never calls Guid.NewGuid itself; the generator is
//                     injected, which is what makes the id predictable in a test.
//
// Client-side ids are the quiet enabler of half this track. An entity that gets its
// identity from the database cannot be published in an outbox message before it is
// committed, cannot be its own idempotency key, and cannot be referenced by the aggregate
// created alongside it in the same transaction. Every one of those problems is usually
// solved by another round trip, and none of them exists if the id is assigned in memory.
//
// Equality by id is the other half, and it is what separates an entity from a value
// object (exercise 081): a Money that changes its amount is a different Money, while a
// Customer that changes its name is the same customer. Getting this backwards produces a
// HashSet that loses entities the moment anybody edits one.
public sealed class Customer : IEquatable<Customer>
{
    public string Id { get; }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public Customer(IIdGenerator ids, string name, string email) =>
        throw new NotImplementedException(
            "TODO: Ex082 - take the id from the generator now, and assign the rest");

    public void Rename(string name) =>
        throw new NotImplementedException("TODO: Ex082 - change the name; the customer is still the same customer");

    public bool Equals(Customer? other) =>
        throw new NotImplementedException("TODO: Ex082 - equal when the ids match");

    public override bool Equals(object? obj) => Equals(obj as Customer);

    public override int GetHashCode() =>
        throw new NotImplementedException("TODO: Ex082 - hash the id and nothing else");
}
