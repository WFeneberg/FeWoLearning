using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Architecture.Exercises.Web.Ex001.Domain
{
    public sealed record Invoice(string Id, decimal Amount);

    public interface IInvoiceStore
    {
        Invoice? Find(string id);
        void Save(Invoice invoice);
    }

    public sealed class InvoiceService(IInvoiceStore store)
    {
        public void Issue(string id, decimal amount) => store.Save(new Invoice(id, amount));

        public decimal AmountOf(string id) => store.Find(id)?.Amount ?? 0m;
    }

    public sealed class LeakyByConstructor(Infrastructure.InMemoryInvoiceStore store)
    {
        public int Count => store.Count;
    }

    public sealed class LeakyByField
    {
        private readonly Infrastructure.InMemoryInvoiceStore _store = new();

        public int Count => _store.Count;
    }
}

namespace FeWoLearning.Architecture.Exercises.Web.Ex001.Infrastructure
{
    using Domain;

    public sealed class InMemoryInvoiceStore : IInvoiceStore
    {
        private readonly Dictionary<string, Invoice> _invoices = [];

        public int Count => _invoices.Count;

        public Invoice? Find(string id) => _invoices.GetValueOrDefault(id);

        public void Save(Invoice invoice) => _invoices[invoice.Id] = invoice;
    }
}

namespace FeWoLearning.Architecture.Exercises.Web
{
    // Exercise 001 — LayeredCompositionRoot (reference solution).
    public static class Ex001_LayeredCompositionRoot
    {
        private const string DomainSuffix = ".Ex001.Domain";
        private const string InfrastructureSuffix = ".Ex001.Infrastructure";

        public static ServiceProvider Build()
        {
            var services = new ServiceCollection();

            // The port is bound to the adapter here and nowhere else. Singleton,
            // because the in-memory adapter IS the store - a transient one would hand
            // every scope its own empty dictionary.
            services.AddSingleton<Ex001.Domain.IInvoiceStore, Ex001.Infrastructure.InMemoryInvoiceStore>();
            services.AddScoped<Ex001.Domain.InvoiceService>();

            return services.BuildServiceProvider();
        }

        public static IReadOnlyList<string> FindDependencyDirectionViolations()
        {
            var assembly = typeof(Ex001_LayeredCompositionRoot).Assembly;

            var violations = new List<string>();

            foreach (var type in assembly.GetTypes())
            {
                if (type.Namespace is null || !type.Namespace.EndsWith(DomainSuffix, StringComparison.Ordinal))
                    continue;

                if (DependsOnInfrastructure(type))
                    violations.Add(type.Name);
            }

            violations.Sort(StringComparer.Ordinal);
            return violations;
        }

        private static bool DependsOnInfrastructure(Type type)
        {
            const BindingFlags members =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            // Constructor parameters, fields AND properties. Constructor injection is
            // the obvious channel; a field initialised with `new` is the one a
            // constructor-only scan walks straight past, which is why LeakyByField
            // exists.
            var referenced = type.GetConstructors(members)
                .SelectMany(c => c.GetParameters().Select(p => p.ParameterType))
                .Concat(type.GetFields(members).Select(f => f.FieldType))
                .Concat(type.GetProperties(members).Select(p => p.PropertyType));

            return referenced.Any(IsInfrastructure);
        }

        private static bool IsInfrastructure(Type type)
        {
            // Unwrap arrays and generic arguments, so List<InMemoryInvoiceStore> counts.
            if (type.IsArray)
                return IsInfrastructure(type.GetElementType()!);

            if (type.IsGenericType && type.GetGenericArguments().Any(IsInfrastructure))
                return true;

            return type.Namespace?.EndsWith(InfrastructureSuffix, StringComparison.Ordinal) == true;
        }
    }
}
