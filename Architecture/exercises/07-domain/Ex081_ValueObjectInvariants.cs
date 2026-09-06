namespace FeWoLearning.Architecture.Exercises.Domain.Ex081;

public sealed class InvalidValueException(string message) : Exception(message);

// Exercise 081 — ValueObjectInvariants (domain).
// Goal:   Make a value that cannot exist in an invalid state, so nothing downstream ever
//         has to check it again.
// Drills: constructor validation, equality by value, immutability, normalisation.
// Passes: construction - a valid input produces the value; every invalid one throws
//                        InvalidValueException naming what was wrong.
//         THE ONE       - there is NO WAY to get an invalid instance. Not through a
//                        parameterless constructor, not through a setter, not through
//                        `with`. A validated factory beside a public constructor validates
//                        nothing.
//         equality     - two values with the same content are equal and hash alike.
//                        Two Money amounts in different currencies are NOT equal, and
//                        adding them is refused rather than silently wrong.
//         normalisation- the currency is upper-cased and the email lower-cased ON
//                        CONSTRUCTION, so equality does not depend on how somebody typed it.
//
// The point of a value object is not tidiness, it is deleting checks. Every place that
// receives an EmailAddress can stop asking whether it contains an "@" - not because the
// check moved, but because the type makes the question unanswerable. That only holds if
// the invalid instance is genuinely unreachable: one public constructor, one setter, or
// one `record` with an init property, and every caller is back to guessing.
//
// Adding two Money values in different currencies is the same idea one level up. The
// alternative is a decimal, and a decimal will let you add euros to dollars in silence.
public sealed record Money
{
    public decimal Amount { get; }

    public string Currency { get; }

    /// <summary>Amount must not be negative; currency must be three letters.</summary>
    public Money(decimal amount, string currency) =>
        throw new NotImplementedException(
            "TODO: Ex081 - validate the amount and the currency, normalise the currency to upper case, and assign");

    public Money Add(Money other) =>
        throw new NotImplementedException(
            "TODO: Ex081 - add only when the currencies match, otherwise refuse");
}

public sealed record EmailAddress
{
    public string Value { get; }

    /// <summary>Must contain exactly one "@", with something on each side.</summary>
    public EmailAddress(string value) =>
        throw new NotImplementedException(
            "TODO: Ex081 - validate the shape, trim and lower-case it, and assign");

    public string Domain =>
        throw new NotImplementedException("TODO: Ex081 - the part after the @");
}
