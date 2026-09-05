namespace FeWoLearning.Architecture.Exercises.Web.Ex011.Domain
{
    /// <summary>An internal annotation nobody outside the service is allowed to see.</summary>
    public sealed record Note(string Author, string Text);

    public sealed class Customer(string id, string firstName, string lastName, decimal creditLimit)
    {
        private readonly List<Note> _notes = [];

        public string Id { get; } = id;
        public string FirstName { get; } = firstName;
        public string LastName { get; } = lastName;
        public decimal CreditLimit { get; } = creditLimit;

        public IReadOnlyList<Note> InternalNotes => _notes;

        public void Annotate(string author, string text) => _notes.Add(new Note(author, text));
    }
}

namespace FeWoLearning.Architecture.Exercises.Web.Ex011.Contracts
{
    using Domain;

    /// <summary>The shape the outside world sees. No domain type appears in it.</summary>
    public sealed record CustomerDto(string Id, string DisplayName, decimal CreditLimit);

    /// <summary>Deliberate leak: a domain aggregate straight through the boundary.</summary>
    public sealed record LeakyCustomerDto(string Id, Customer Customer);

    /// <summary>
    /// Deliberate leak, one level down: the property type is a BCL collection, and only
    /// its generic argument is a domain type. A checker that compares property types
    /// directly is an earnest implementation that walks straight past this one.
    /// </summary>
    public sealed record LeakyNotesDto(string Id, IReadOnlyList<Note> Notes);
}

namespace FeWoLearning.Architecture.Exercises.Web
{
    using Ex011.Contracts;
    using Ex011.Domain;

    // Exercise 011 — DtoBoundaryMapping (web).
    // Goal:   Project an aggregate onto a contract the outside world can depend on, and
    //         write the check that proves no domain type escaped through it.
    // Drills: domain-to-DTO projection, preventing domain leakage across the boundary.
    // Passes: ToDto()          - Id and CreditLimit carry over, and DisplayName is the
    //                            first and last name joined with a space, so a mapper
    //                            that copies a single field cannot pass.
    //         FindDomainLeaks() - reports "LeakyCustomerDto" and "LeakyNotesDto", and
    //                            reports neither "CustomerDto" nor anything else.
    //
    // The second leak is the interesting one. Once IReadOnlyList<Note> is public, the
    // domain's Note record is part of the published contract: renaming its Author field
    // is now a breaking change for every consumer, and nobody writing that rename will
    // think to look.
    public static class Ex011_DtoBoundaryMapping
    {
        public static CustomerDto ToDto(Customer customer) =>
            throw new NotImplementedException(
                "TODO: Ex011 - project the customer onto CustomerDto, joining first and last name into DisplayName");

        /// <summary>
        /// Scan this assembly for types in a namespace ending ".Ex011.Contracts" whose
        /// public properties expose a type from ".Ex011.Domain" - directly, or as a
        /// generic argument, or as an array element. Return the offending type names.
        /// </summary>
        public static IReadOnlyList<string> FindDomainLeaks() =>
            throw new NotImplementedException(
                "TODO: Ex011 - report contract types whose properties expose a domain type, including through a generic argument");
    }
}
