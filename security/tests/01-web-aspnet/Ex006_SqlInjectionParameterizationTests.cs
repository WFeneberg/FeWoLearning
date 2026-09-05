using FeWoLearning.Security.Exercises.Support;
using FeWoLearning.Security.Exercises.WebAspNet;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex006_SqlInjectionParameterizationTests
{
    [Theory]
    [InlineData("x' or '1'='1")]
    [InlineData("x' or '1'='1' --")]
    public void Attack_A_Tautology_Payload_Returns_No_Rows(string payload)
    {
        using var db = new Ex006_UserDatabase();

        var result = Ex006_SqlInjectionParameterization.FindEmailsByName(db.Connection, payload);

        Assert.Empty(result);
    }

    [Fact]
    public void Attack_A_Drop_Table_Payload_Leaves_The_Table_Intact()
    {
        using var db = new Ex006_UserDatabase();

        Ex006_SqlInjectionParameterization.FindEmailsByName(db.Connection, "'; drop table users; --");

        using var check = db.Connection.CreateCommand();
        check.CommandText = "select count(*) from users;";
        var count = (long)check.ExecuteScalar()!;

        Assert.Equal(2, count);
    }

    [Fact]
    public void Use_A_Known_Name_Returns_Its_Email()
    {
        using var db = new Ex006_UserDatabase();

        var result = Ex006_SqlInjectionParameterization.FindEmailsByName(db.Connection, "ada");

        Assert.Equal(new[] { "ada@example.com" }, result);
    }

    [Fact]
    public void Use_An_Unknown_Name_Returns_An_Empty_List()
    {
        using var db = new Ex006_UserDatabase();

        var result = Ex006_SqlInjectionParameterization.FindEmailsByName(db.Connection, "carol");

        Assert.Empty(result);
    }
}
