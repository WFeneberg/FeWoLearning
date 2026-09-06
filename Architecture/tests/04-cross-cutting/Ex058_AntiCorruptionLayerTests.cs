using FeWoLearning.Architecture.Exercises.CrossCutting;
using FeWoLearning.Architecture.Exercises.CrossCutting.Ex058.Legacy;
using FeWoLearning.Architecture.Exercises.CrossCutting.Ex058.Sales;

namespace FeWoLearning.Architecture.Tests.CrossCutting;

public class Ex058_AntiCorruptionLayerTests
{
    private static CUSTREC Record(string status = "A") =>
        new() { CUST_NM = "  Ada Lovelace  ", CUST_STAT = status, CRED_LIM_CENTS = 123456 };

    [Fact]
    public void Translation_Puts_The_Record_Into_Our_Language()
    {
        var customer = Ex058_AntiCorruptionLayer.Translate(Record());

        Assert.Equal("Ada Lovelace", customer.Name);
        Assert.Equal(AccountStanding.Active, customer.Standing);
        Assert.Equal(1234.56m, customer.CreditLimit);
    }

    [Theory]
    [InlineData("A", AccountStanding.Active)]
    [InlineData("S", AccountStanding.Suspended)]
    [InlineData("C", AccountStanding.Closed)]
    public void Every_Known_Status_Code_Maps(string code, AccountStanding expected) =>
        Assert.Equal(expected, Ex058_AntiCorruptionLayer.Translate(Record(code)).Standing);

    [Fact]
    public void Adversarial_An_Unknown_Status_Code_Is_Refused_By_Name()
    {
        // Defaulting an unrecognised code to the permissive value is how a suspended
        // customer starts buying again after the other team adds a status nobody
        // mentioned. A `_ => Active` arm passes every fact above.
        var failure = Assert.Throws<ArgumentOutOfRangeException>(
            () => Ex058_AntiCorruptionLayer.Translate(Record("X")));

        Assert.Contains("X", failure.Message);
    }

    [Fact]
    public void Money_Crosses_The_Boundary_As_Money()
    {
        // Cents are the other context's representation. Carrying them across means every
        // caller on this side has to remember to divide, and one of them will not.
        var customer = Ex058_AntiCorruptionLayer.Translate(
            new CUSTREC { CUST_NM = "x", CUST_STAT = "A", CRED_LIM_CENTS = 5 });

        Assert.Equal(0.05m, customer.CreditLimit);
    }

    [Fact]
    public void Fitness_Our_Own_Model_Is_Not_Reported()
    {
        // Paired with the fact below - alone, an empty list satisfies it.
        Assert.DoesNotContain(nameof(Customer), Ex058_AntiCorruptionLayer.FindForeignTypeLeaks());
    }

    [Fact]
    public void Fitness_A_Type_Keeping_The_Original_Around_Is_Reported()
    {
        // "Just in case" is how it always starts, and with that one field CUSTREC is part
        // of our model forever: renaming CUST_STAT is now our problem too.
        Assert.Contains(nameof(LeakyCustomer), Ex058_AntiCorruptionLayer.FindForeignTypeLeaks());
    }
}
