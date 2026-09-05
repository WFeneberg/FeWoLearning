using Microsoft.AspNetCore.Http;

namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 016 — InsecureDirectObjectReference (web-aspnet).
// Goal:   Look up an invoice by id for a specific caller without letting the
//         response distinguish "this id belongs to someone else" from "this id
//         does not exist" - a 403 on the former confirms the id is real and
//         hands an attacker a working enumeration oracle, so both must come
//         back as the very same 404.
// Drills: ownership enforcement, opaque identifiers, enumeration.
// Passes: attack facts   - requesting another user's existing invoice returns
//                          404 (never 403), a non-existent id also returns 404,
//                          and the two 404 responses are byte-identical;
//         use facts      - the owner requesting their own invoice gets 200
//                          with the amount, and an owner with two invoices can
//                          fetch both.
public sealed record Ex016_Invoice(int Id, string OwnerId, decimal Amount);

public static class Ex016_InsecureDirectObjectReference
{
    public static IResult GetInvoice(string callerId, int invoiceId, IReadOnlyList<Ex016_Invoice> store) =>
        throw new NotImplementedException(
            "TODO: Ex016 - return 200 with the amount for the owner, and an identical 404 for both a missing id and someone else's id");
}
