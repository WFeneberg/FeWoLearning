using Bunit;
using Bunit.TestDoubles;
using FeWoLearning.Blazor.Exercises.Advanced;
using Xunit;

namespace FeWoLearning.Blazor.Tests.Advanced;

// AddAuthorization() installs bUnit's stand-ins for the whole authorization stack -
// state provider, policy provider and authorization service - which is what
// <AuthorizeView> resolves out of DI.
public class Ex081_CascadingAuthStateBasicsTests : BunitContext
{
    [Fact]
    public void A_Signed_Out_Visitor_Gets_The_Not_Authorized_Fragment()
    {
        AddAuthorization().SetNotAuthorized();

        var cut = Render<Ex081_CascadingAuthStateBasics>();

        Assert.Equal("sign in", cut.Find("#anon").TextContent);
        Assert.Empty(cut.FindAll("#user"));
        Assert.Empty(cut.FindAll("#pending"));
    }

    // @context.User is the ClaimsPrincipal, so this also pins down that the name is
    // read off the principal rather than hardcoded or taken from a parameter.
    [Fact]
    public void A_Signed_In_User_Gets_Their_Name_From_The_Principal()
    {
        AddAuthorization().SetAuthorized("ada");

        var cut = Render<Ex081_CascadingAuthStateBasics>();

        Assert.Equal("ada", cut.Find("#user").TextContent);
        Assert.Empty(cut.FindAll("#anon"));
    }

    // The third fragment, and the one most often left out: the state is a Task, and
    // until it completes there is no answer - not "no", just not yet.
    [Fact]
    public void An_Unresolved_State_Gets_The_Authorizing_Fragment()
    {
        AddAuthorization().SetAuthorizing();

        var cut = Render<Ex081_CascadingAuthStateBasics>();

        Assert.Equal("checking…", cut.Find("#pending").TextContent);
        Assert.Empty(cut.FindAll("#user"));
        Assert.Empty(cut.FindAll("#anon"));
    }

    [Fact]
    public void The_Role_Gated_Block_Appears_Only_For_That_Role()
    {
        AddAuthorization().SetAuthorized("ada").SetRoles("admin");

        var cut = Render<Ex081_CascadingAuthStateBasics>();

        Assert.Equal("admin tools", cut.Find("#admin").TextContent);
    }

    // Non-vacuity for Roles: being signed in is not the same as being in the role.
    // Negative assertion, so it stays bare (README §11).
    [Fact]
    public void The_Role_Gated_Block_Stays_Away_From_Other_Users()
    {
        AddAuthorization().SetAuthorized("bob").SetRoles("reader");

        var cut = Render<Ex081_CascadingAuthStateBasics>();

        Assert.Equal("bob", cut.Find("#user").TextContent);
        Assert.Empty(cut.FindAll("#admin"));
    }
}
