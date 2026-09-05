using FeWoLearning.Security.Exercises.WebAspNet;
using Microsoft.AspNetCore.Http;

namespace FeWoLearning.Security.Tests.WebAspNet;

public class Ex011_SessionFixationTests
{
    [Fact]
    public void Attack_SignIn_Never_Returns_The_Identifier_The_Request_Presented()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["Cookie"] = "sid=attacker-fixed-id-1";

        var newId = Ex011_SessionFixation.SignIn(context, "alice");

        Assert.NotEqual("attacker-fixed-id-1", newId);
    }

    [Fact]
    public void Attack_The_Presented_Identifier_Resolves_To_An_Anonymous_Session_Afterwards()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["Cookie"] = "sid=attacker-fixed-id-2";

        Ex011_SessionFixation.SignIn(context, "alice");

        Assert.Null(Ex011_SessionFixation.Resolve("attacker-fixed-id-2"));
    }

    [Fact]
    public void Use_The_Returned_Identifier_Resolves_To_A_Session_Carrying_The_User_Name()
    {
        var context = new DefaultHttpContext();

        var newId = Ex011_SessionFixation.SignIn(context, "alice");

        Assert.Equal("alice", Ex011_SessionFixation.Resolve(newId));
    }

    [Fact]
    public void Use_Presenting_The_New_Identifier_Twice_Resolves_The_Same_Session_Both_Times()
    {
        var context = new DefaultHttpContext();
        var newId = Ex011_SessionFixation.SignIn(context, "alice");

        var first = Ex011_SessionFixation.Resolve(newId);
        var second = Ex011_SessionFixation.Resolve(newId);

        Assert.Equal("alice", first);
        Assert.Equal(first, second);
    }
}
