using System.Reflection;

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

    // Exercise 011 — DtoBoundaryMapping (reference solution).
    public static class Ex011_DtoBoundaryMapping
    {
        private const string ContractsSuffix = ".Ex011.Contracts";
        private const string DomainSuffix = ".Ex011.Domain";

        public static CustomerDto ToDto(Customer customer) =>
            new(customer.Id,
                customer.FirstName + " " + customer.LastName,
                customer.CreditLimit);

        public static IReadOnlyList<string> FindDomainLeaks()
        {
            var assembly = typeof(Ex011_DtoBoundaryMapping).Assembly;

            var leaks = new List<string>();

            foreach (var type in assembly.GetTypes())
            {
                if (type.Namespace?.EndsWith(ContractsSuffix, StringComparison.Ordinal) != true)
                    continue;

                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                if (properties.Any(p => ExposesDomain(p.PropertyType)))
                    leaks.Add(type.Name);
            }

            leaks.Sort(StringComparer.Ordinal);
            return leaks;
        }

        /// <summary>
        /// Recursive on purpose. IReadOnlyList&lt;Note&gt; is not a domain type, and a
        /// direct namespace comparison says so quite correctly - while the contract it
        /// sits in has nevertheless published Note to every consumer.
        /// </summary>
        private static bool ExposesDomain(Type type)
        {
            if (type.Namespace?.EndsWith(DomainSuffix, StringComparison.Ordinal) == true)
                return true;

            if (type.IsArray)
                return ExposesDomain(type.GetElementType()!);

            return type.IsGenericType && type.GetGenericArguments().Any(ExposesDomain);
        }
    }
}
