using Microsoft.Extensions.DependencyInjection;

namespace FeWoLearning.Architecture.Exercises.Web.Ex001.Domain
{
    public sealed record Invoice(string Id, decimal Amount);

    /// <summary>
    /// The port. The DOMAIN owns this interface; infrastructure implements it. That
    /// inversion is the whole reason the dependency arrow can point inwards.
    /// </summary>
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

    // Two deliberate violations, shipped so the fitness check has something to catch.
    // Both sit in the domain namespace and both reach out into infrastructure - one
    // through a constructor parameter, one through a private field. A checker that
    // only inspects constructors finds the first and silently misses the second.

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
    // Exercise 001 — LayeredCompositionRoot (web).
    // Goal:   Wire a three-layer application from a single composition root, and write
    //         the reflection check that proves the dependency arrow points inwards.
    // Drills: layering, composition root, dependency direction, ports and adapters.
    // Passes: Build()   - resolving InvoiceService round-trips an invoice; the port
    //                     IInvoiceStore resolves to the infrastructure adapter
    //                     InMemoryInvoiceStore; and the store is a SINGLETON, so an
    //                     invoice issued in one scope is readable from another.
    //         FindDependencyDirectionViolations() - reports LeakyByConstructor AND
    //                     LeakyByField by name, and reports neither InvoiceService
    //                     nor Invoice.
    //
    // Note on grading: dependency direction is a property of the code's metadata, not
    // of its behaviour - no runtime assertion can tell "the domain does not reference
    // infrastructure" apart from "the domain happens not to call it in this test". So
    // the second half of this exercise is graded by reflection, and the shipped
    // violations above are what stop a checker that returns nothing from passing.
    public static class Ex001_LayeredCompositionRoot
    {
        /// <summary>
        /// The composition root: the one place that knows both the ports and the
        /// adapters. Register Domain.IInvoiceStore against
        /// Infrastructure.InMemoryInvoiceStore as a singleton, and Domain.InvoiceService
        /// so it can be resolved.
        ///
        /// Gotcha: this class sits in ...Exercises.Web, and the layers are nested one
        /// level deeper, so they are Ex001.Domain and Ex001.Infrastructure from here -
        /// a bare "Domain.IInvoiceStore" fails CS0246.
        /// </summary>
        public static ServiceProvider Build() =>
            throw new NotImplementedException(
                "TODO: Ex001 - build a ServiceProvider that binds IInvoiceStore to InMemoryInvoiceStore as a singleton and registers InvoiceService");

        /// <summary>
        /// Scan this assembly for types in a namespace ending ".Ex001.Domain" that
        /// depend on a type in a namespace ending ".Ex001.Infrastructure", and return
        /// the offending type names. A dependency counts if it appears as a
        /// constructor parameter, a field, or a property.
        /// </summary>
        public static IReadOnlyList<string> FindDependencyDirectionViolations() =>
            throw new NotImplementedException(
                "TODO: Ex001 - report domain types that reference an infrastructure type through a constructor parameter, a field, or a property");
    }
}
