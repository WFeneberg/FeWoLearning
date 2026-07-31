using System.Linq;
using FeWoLearning.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Advanced;

public class Ex085_MinimalEndpointHandlerTests
{
    [Fact]
    public void ValidRequest_ReturnsSuccessWithMappedResponse()
    {
        var request = new CreateUserRequest("Ada Lovelace", "ada@example.com", 30);

        var result = MinimalEndpointHandler.Handle(request);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        Assert.NotNull(result.Response);
        Assert.Equal("Ada Lovelace", result.Response!.Name);
        Assert.Equal("ada@example.com", result.Response.Email);
        Assert.Equal(30, result.Response.Age);
    }

    [Fact]
    public void InvalidRequest_ReturnsFailureWithAllErrors()
    {
        var request = new CreateUserRequest(" ", "not-an-email", 200);

        var result = MinimalEndpointHandler.Handle(request);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Response);
        Assert.Equal(3, result.Errors.Count);
        Assert.Contains(result.Errors, e => e.Contains("Name"));
        Assert.Contains(result.Errors, e => e.Contains("Email"));
        Assert.Contains(result.Errors, e => e.Contains("Age"));
    }

    [Fact]
    public void MissingEmailAtSign_IsRejected()
    {
        var request = new CreateUserRequest("Bob", "bob.example.com", 40);

        var result = MinimalEndpointHandler.Handle(request);

        Assert.False(result.IsSuccess);
        var error = Assert.Single(result.Errors);
        Assert.Contains("Email", error);
    }

    [Fact]
    public void BoundaryAges_AreAccepted()
    {
        var low = MinimalEndpointHandler.Handle(new CreateUserRequest("A", "a@b.co", 0));
        var high = MinimalEndpointHandler.Handle(new CreateUserRequest("B", "b@c.co", 149));

        Assert.True(low.IsSuccess);
        Assert.True(high.IsSuccess);
    }

    [Fact]
    public void NegativeAge_IsRejected()
    {
        var result = MinimalEndpointHandler.Handle(new CreateUserRequest("C", "c@d.co", -1));

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
    }
}
