using Microsoft.AspNetCore.Http;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 016 — InsecureDirectObjectReference (reference solution).
public sealed record Ex016_Invoice(int Id, string OwnerId, decimal Amount);

public static class Ex016_InsecureDirectObjectReference
{
    public static IResult GetInvoice(string callerId, int invoiceId, IReadOnlyList<Ex016_Invoice> store)
    {
        var invoice = store.FirstOrDefault(i => i.Id == invoiceId);

        // Same 404 whether the id does not exist or belongs to someone else - a
        // 403 here would confirm the id is real and become an enumeration oracle.
        if (invoice is null || !string.Equals(invoice.OwnerId, callerId, StringComparison.Ordinal))
        {
            return Results.NotFound();
        }

        return Results.Ok(invoice.Amount);
    }
}
