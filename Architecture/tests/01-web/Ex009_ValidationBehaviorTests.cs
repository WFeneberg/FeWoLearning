using FeWoLearning.Architecture.Exercises.Web.Ex009;

namespace FeWoLearning.Architecture.Tests.Web;

public class Ex009_ValidationBehaviorTests
{
    private static IReadOnlyList<IValidator<CreateUser>> Validators() =>
        [new EmailValidator(), new AgeValidator()];

    [Fact]
    public void Use_A_Valid_Request_Reaches_The_Handler_Once()
    {
        var handler = new CreateUserHandler();

        var result = Ex009_ValidationBehavior.Execute(
            new CreateUser("ada@example.com", 36), Validators(), handler);

        Assert.Equal("user:ada@example.com", result);
        Assert.Equal(1, handler.Invocations);
    }

    [Fact]
    public void An_Invalid_Request_Is_Rejected()
    {
        var handler = new CreateUserHandler();

        Assert.Throws<RequestValidationException>(() => Ex009_ValidationBehavior.Execute(
            new CreateUser("not-an-email", 36), Validators(), handler));
    }

    [Fact]
    public void Mechanism_An_Invalid_Request_Never_Reaches_The_Handler()
    {
        // The fact this exercise exists for. Validating INSIDE the handler produces the
        // same exception and passes the fact above; it differs only in that the handler
        // has already started - opened its transaction, taken its lock, charged the
        // caller's rate limit. Invocations is what tells them apart.
        var handler = new CreateUserHandler();

        Assert.Throws<RequestValidationException>(() => Ex009_ValidationBehavior.Execute(
            new CreateUser("not-an-email", 36), Validators(), handler));

        Assert.Equal(0, handler.Invocations);
    }

    [Fact]
    public void Adversarial_Every_Problem_Is_Reported_Not_Just_The_First()
    {
        // Fail-fast is an earnest implementation that passes both facts above. It costs
        // the caller one round trip per problem, and the caller is usually a person
        // filling in a form.
        var handler = new CreateUserHandler();

        var failure = Assert.Throws<RequestValidationException>(() => Ex009_ValidationBehavior.Execute(
            new CreateUser("not-an-email", 12), Validators(), handler));

        Assert.Contains(EmailValidator.Message, failure.Errors);
        Assert.Contains(AgeValidator.Message, failure.Errors);
        Assert.Equal(2, failure.Errors.Count);
    }

    [Fact]
    public void A_Validator_That_Found_Nothing_Contributes_Nothing()
    {
        // Pairs with the fact above: "collect everything" must not mean "report
        // everything", or every rejection carries noise the caller has to filter.
        var handler = new CreateUserHandler();

        var failure = Assert.Throws<RequestValidationException>(() => Ex009_ValidationBehavior.Execute(
            new CreateUser("ada@example.com", 12), Validators(), handler));

        Assert.Equal([AgeValidator.Message], failure.Errors);
    }
}
