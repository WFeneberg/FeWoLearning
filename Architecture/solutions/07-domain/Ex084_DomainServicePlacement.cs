namespace FeWoLearning.Architecture.Exercises.Domain.Ex084;

public sealed record Account(string Id, decimal Balance, string Currency);

public sealed class TransferRefusedException(string reason) : Exception(reason);

/// <summary>
/// The result of a transfer: two NEW accounts. Neither original is mutated, because
/// neither of them owns this operation.
/// </summary>
public sealed record TransferResult(Account From, Account To);

// Exercise 084 — DomainServicePlacement (reference solution).
public static class Ex084_DomainServicePlacement
{
    public static TransferResult Transfer(Account source, Account destination, decimal amount)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        // Every check before anything moves - the same discipline as exercise 010's
        // transfer, for the same reason: once money has moved, "return a failure" is no
        // longer an honest description of what happened.
        if (amount <= 0)
            throw new TransferRefusedException("Amount must be positive.");

        if (source.Currency != destination.Currency)
            throw new TransferRefusedException($"Cannot transfer {source.Currency} into a {destination.Currency} account.");

        if (source.Balance < amount)
            throw new TransferRefusedException($"Account {source.Id} holds less than {amount}.");

        // NEW values, both of them. The service is not the owner of either account and
        // must not behave as though it were - mutating them here is the same coupling as
        // putting the method on one of them, just less visible.
        return new TransferResult(
            source with { Balance = source.Balance - amount },
            destination with { Balance = destination.Balance + amount });
    }
}
