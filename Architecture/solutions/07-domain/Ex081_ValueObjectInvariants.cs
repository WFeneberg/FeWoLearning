namespace FeWoLearning.Architecture.Exercises.Domain.Ex081;

public sealed class InvalidValueException(string message) : Exception(message);

// Exercise 081 — ValueObjectInvariants (reference solution).
public sealed record Money
{
    public decimal Amount { get; }

    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        // Validated in THE constructor, and there is only one. A static Create beside a
        // public constructor validates nothing: the constructor is still there, and
        // somebody will use it.
        if (amount < 0)
            throw new InvalidValueException($"Amount must not be negative, was {amount}.");

        if (currency is null || currency.Trim().Length != 3 || !currency.Trim().All(char.IsLetter))
            throw new InvalidValueException($"Currency must be three letters, was '{currency}'.");

        Amount = amount;

        // Normalised here, so equality never depends on how somebody typed it. Doing this
        // at the comparison instead means every comparison has to remember.
        Currency = currency.Trim().ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        ArgumentNullException.ThrowIfNull(other);

        // The alternative to this type is a decimal, and a decimal will add euros to
        // dollars in silence.
        if (other.Currency != Currency)
            throw new InvalidValueException($"Cannot add {other.Currency} to {Currency}.");

        return new Money(Amount + other.Amount, Currency);
    }
}

public sealed record EmailAddress
{
    public string Value { get; }

    public EmailAddress(string value)
    {
        var trimmed = value?.Trim() ?? "";
        var parts = trimmed.Split('@');

        if (parts.Length != 2 || parts[0].Length == 0 || parts[1].Length == 0)
            throw new InvalidValueException($"'{value}' is not an email address.");

        // Every place that receives an EmailAddress can now stop asking whether it
        // contains an "@" - not because the check moved, but because the type makes the
        // question unanswerable.
        Value = trimmed.ToLowerInvariant();
    }

    public string Domain => Value.Split('@')[1];
}
