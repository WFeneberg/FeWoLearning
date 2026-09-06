namespace FeWoLearning.Architecture.Exercises.Domain.Ex084;

public sealed record Account(string Id, decimal Balance, string Currency);

public sealed class TransferRefusedException(string reason) : Exception(reason);

/// <summary>
/// The result of a transfer: two NEW accounts. Neither original is mutated, because
/// neither of them owns this operation.
/// </summary>
public sealed record TransferResult(Account From, Account To);

// Exercise 084 — DomainServicePlacement (domain).
// Goal:   Put a rule somewhere sensible when it belongs to no single entity.
// Drills: domain services, the anaemic-model trap, where an operation on two aggregates goes.
// Passes: transfer  - moves the amount, returning both updated accounts.
//         rules     - refuses a non-positive amount, mismatched currencies, and more than
//                     the source holds; nothing is changed in any of those cases.
//         THE ONE    - neither input Account is mutated. The service returns new values,
//                     because it is not the owner of either one and must not pretend to be.
//         placement - Account itself has no Transfer method, and no reference to another
//                     Account. "account.TransferTo(other)" has to pick a winner, and
//                     whichever it picks now knows about a second aggregate for ever.
//
// A domain service is what is left when an operation genuinely belongs to no entity, and
// transferring between two accounts is the canonical case: put it on the source and the
// source now knows how to modify a different aggregate; put it on the target and the same
// thing happens in reverse; put it on a static helper in the application layer and the
// rule leaves the domain entirely, to be re-implemented slightly differently by the next
// caller.
//
// It is also the most over-used escape hatch in the tactical toolkit. Every rule moved to
// a service is a rule the entity no longer enforces, and a model where all the behaviour
// lives in services is not a domain model - it is a database with extra classes. The test
// is whether the operation has a natural owner: withdrawing does (the account), transferring
// does not (neither account is more responsible than the other).
//
// A C# aside, because it costs ten minutes the first time you hit it: the parameters are
// called `source` and `destination` rather than `from` and `to` because `from` is a
// contextual query keyword. `from with { ... }` parses as the start of a LINQ query
// expression and produces half a dozen bewildering errors on one line, none of which
// mentions LINQ.
public static class Ex084_DomainServicePlacement
{
    public static TransferResult Transfer(Account source, Account destination, decimal amount) =>
        throw new NotImplementedException(
            "TODO: Ex084 - validate the amount, the currencies and the balance BEFORE moving anything, then return two new accounts");
}
