using FeWoLearning.Security.Exercises.WebAspNet;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex005_ModelBindingOverpostingTests
{
    private static Ex005_UserProfile Existing() => new()
    {
        Id = 7,
        DisplayName = "Alice",
        Email = "alice@example.com",
        IsAdministrator = false,
    };

    [Fact]
    public void Attack_IsAdministrator_In_The_Request_Body_Is_Ignored()
    {
        var result = Ex005_ModelBindingOverposting.Apply(
            Existing(), """{"displayName":"Alice B.","isAdministrator":true}""");

        Assert.False(result.IsAdministrator);
    }

    [Fact]
    public void Attack_Id_In_The_Request_Body_Is_Ignored()
    {
        var result = Ex005_ModelBindingOverposting.Apply(
            Existing(), """{"displayName":"Alice B.","id":999}""");

        Assert.Equal(7, result.Id);
    }

    [Fact]
    public void Use_DisplayName_And_Email_Both_Update_When_Both_Are_Sent()
    {
        var result = Ex005_ModelBindingOverposting.Apply(
            Existing(), """{"displayName":"Alice B.","email":"alice.b@example.com"}""");

        Assert.Equal("Alice B.", result.DisplayName);
        Assert.Equal("alice.b@example.com", result.Email);
    }

    [Fact]
    public void Use_Sending_Only_DisplayName_Leaves_Email_Unchanged()
    {
        var result = Ex005_ModelBindingOverposting.Apply(Existing(), """{"displayName":"Alice B."}""");

        Assert.Equal("Alice B.", result.DisplayName);
        Assert.Equal("alice@example.com", result.Email);
    }
}
