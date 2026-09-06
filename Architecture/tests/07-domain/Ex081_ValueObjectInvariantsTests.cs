using System.Reflection;
using FeWoLearning.Architecture.Exercises.Domain.Ex081;

namespace FeWoLearning.Architecture.Tests.Domain;

public class Ex081_ValueObjectInvariantsTests
{
    [Fact]
    public void A_Valid_Money_Is_Constructed()
    {
        var money = new Money(9.99m, "eur");

        Assert.Equal(9.99m, money.Amount);
        Assert.Equal("EUR", money.Currency);
    }

    [Theory]
    [InlineData(-1, "EUR")]
    [InlineData(1, "EURO")]
    [InlineData(1, "EU")]
    [InlineData(1, "E1R")]
    [InlineData(1, "")]
    public void An_Invalid_Money_Cannot_Be_Constructed(decimal amount, string currency) =>
        Assert.Throws<InvalidValueException>(() => new Money(amount, currency));

    [Fact]
    public void Mechanism_There_Is_No_Way_Round_The_Constructor()
    {
        // A validated factory beside a public parameterless constructor validates nothing:
        // the constructor is still there, and somebody will use it. Same for a settable
        // property - and for a record, `with` goes through the copy constructor, which is
        // why the properties here are get-only.
        //
        // The construction below is not decoration: everything after it asserts type
        // METADATA, which is equally true of the stub - so without a call into the
        // exercise this fact would pass on the untouched tree and grade nothing.
        Assert.Equal("EUR", new Money(1m, "EUR").Currency);

        var money = typeof(Money);

        Assert.All(money.GetConstructors(), c => Assert.NotEmpty(c.GetParameters()));
        Assert.All(
            money.GetProperties(BindingFlags.Public | BindingFlags.Instance),
            p => Assert.False(p.CanWrite, $"{p.Name} is settable, so an invalid instance is reachable"));
    }

    [Fact]
    public void Values_With_The_Same_Content_Are_Equal()
    {
        Assert.Equal(new Money(10m, "EUR"), new Money(10m, "EUR"));
        Assert.Equal(new Money(10m, "EUR").GetHashCode(), new Money(10m, "eur").GetHashCode());
    }

    [Fact]
    public void Mechanism_Normalisation_Happens_On_Construction()
    {
        // Normalising at the comparison instead means every comparison has to remember -
        // and one of them will not, at which point "EUR" and "eur" are two currencies.
        Assert.Equal(new Money(10m, "eur"), new Money(10m, "EUR"));
        Assert.Equal(new EmailAddress("  Ada@Example.COM "), new EmailAddress("ada@example.com"));
    }

    [Fact]
    public void Money_In_Different_Currencies_Is_Not_Equal_And_Cannot_Be_Added()
    {
        // The alternative to this type is a decimal, and a decimal adds euros to dollars
        // in silence.
        Assert.NotEqual(new Money(10m, "EUR"), new Money(10m, "USD"));
        Assert.Throws<InvalidValueException>(() => new Money(10m, "EUR").Add(new Money(10m, "USD")));
    }

    [Fact]
    public void Adding_Money_In_The_Same_Currency_Works()
    {
        Assert.Equal(new Money(15m, "EUR"), new Money(10m, "EUR").Add(new Money(5m, "EUR")));
    }

    [Theory]
    [InlineData("ada@example.com")]
    [InlineData("  Ada@Example.com  ")]
    public void A_Valid_Email_Is_Constructed(string input) =>
        Assert.Equal("example.com", new EmailAddress(input).Domain);

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("@example.com")]
    [InlineData("ada@")]
    [InlineData("ada@two@example.com")]
    [InlineData("")]
    public void An_Invalid_Email_Cannot_Be_Constructed(string input) =>
        Assert.Throws<InvalidValueException>(() => new EmailAddress(input));
}
