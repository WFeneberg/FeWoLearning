using FeWoLearning.Architecture.Exercises.Web.Ex010;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex010_ResultErrorModelTests
{
    private static AccountStore SeededStore()
    {
        var store = new AccountStore();
        store.Seed("alice", 100m);
        store.Seed("bob", 5m);
        return store;
    }

    [Fact]
    public void Use_A_Valid_Transfer_Moves_The_Money_And_Returns_A_Receipt()
    {
        var store = SeededStore();

        var result = Ex010_ResultErrorModel.Transfer(store, "alice", "bob", 30m);

        Assert.True(result.IsSuccess);
        Assert.Equal(new Receipt("alice", "bob", 30m), result.Value);
        Assert.Equal(70m, store.BalanceOf("alice"));
        Assert.Equal(35m, store.BalanceOf("bob"));
    }

    [Fact]
    public void An_Unknown_Account_Is_A_Failure_Value_Not_An_Exception()
    {
        // Reaching the assertion at all is half the fact: an implementation that threw
        // would fail this test by throwing, not by asserting.
        var store = SeededStore();

        var result = Ex010_ResultErrorModel.Transfer(store, "alice", "nobody", 30m);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.NotFound, result.Error.Code);
    }

    [Fact]
    public void Too_Little_Money_Is_A_Failure_Value()
    {
        var store = SeededStore();

        var result = Ex010_ResultErrorModel.Transfer(store, "bob", "alice", 30m);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.InsufficientFunds, result.Error.Code);
    }

    [Fact]
    public void Mechanism_A_Failed_Transfer_Leaves_Both_Balances_Untouched()
    {
        // The fact that grades the design rather than the naming. Debiting first and
        // discovering the shortfall afterwards returns exactly the same Failure - and
        // has already taken the money.
        var store = SeededStore();

        Ex010_ResultErrorModel.Transfer(store, "bob", "alice", 30m);

        Assert.Equal(5m, store.BalanceOf("bob"));
        Assert.Equal(100m, store.BalanceOf("alice"));
    }

    [Fact]
    public void A_Nonpositive_Amount_Is_A_Validation_Failure()
    {
        var store = SeededStore();

        var result = Ex010_ResultErrorModel.Transfer(store, "alice", "bob", 0m);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.Validation, result.Error.Code);
    }

    [Fact]
    public void Every_Declared_Error_Code_Maps_To_Its_Own_Status()
    {
        Assert.Equal(404, Ex010_ResultErrorModel.ToStatusCode(ErrorCode.NotFound));
        Assert.Equal(409, Ex010_ResultErrorModel.ToStatusCode(ErrorCode.InsufficientFunds));
        Assert.Equal(400, Ex010_ResultErrorModel.ToStatusCode(ErrorCode.Validation));
    }

    [Fact]
    public void Adversarial_A_Code_Outside_The_Enum_Is_Rejected_Rather_Than_Defaulted()
    {
        // Catches the `_ => 500` catch-all. That arm maps every code, including ones
        // nobody has thought about yet, so the next ErrorCode someone adds becomes a
        // silent 500 in production rather than a decision made in this switch.
        var undefined = (ErrorCode)999;

        Assert.Throws<ArgumentOutOfRangeException>(
            () => Ex010_ResultErrorModel.ToStatusCode(undefined));
    }
}
