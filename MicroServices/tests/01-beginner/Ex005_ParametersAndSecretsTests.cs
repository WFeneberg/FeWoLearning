using Aspire.Hosting.ApplicationModel;
using FeWoLearning.MicroServices.Exercises.Beginner;

namespace FeWoLearning.MicroServices.Tests.Beginner;

public class Ex005_ParametersAndSecretsTests
{
    [Fact]
    public async Task Secret_flag_lives_on_the_parameter_resource()
    {
        var model = ModelHarness.Build(Ex005_ParametersAndSecrets.Configure);

        // Both must be real ParameterResources - AddConnectionString, or a literal
        // pushed through WithEnvironment, would also "carry a value" and grade
        // nothing about the parameter system.
        var region = Assert.IsType<ParameterResource>(model.Resource("region"));
        var password = Assert.IsType<ParameterResource>(model.Resource("dbpassword"));

        // Secret is a property of the model, not a naming convention: a learner who
        // called the parameter "dbpassword" but left secret defaulted to false fails
        // here, and one who marked everything secret fails on region.
        Assert.False(region.Secret);
        Assert.True(password.Secret);

        // region is a supplied literal - it has a value and no generation policy.
        Assert.Equal("eu-west", await region.GetValueAsync(TestContext.Current.CancellationToken));
        Assert.Null(region.Default);

        // dbpassword is the opposite: no literal anywhere, a policy instead.
        Assert.IsType<GenerateParameterDefault>(password.Default);
    }

    [Fact]
    public async Task Generated_secret_emits_a_generate_policy_instead_of_a_literal()
    {
        using var manifest = await ManifestHarness.GenerateAsync(
            Ex005_ParametersAndSecrets.Configure,
            TestContext.Current.CancellationToken);

        var resources = manifest.RootElement.GetProperty("resources");

        var region = resources.GetProperty("region");
        Assert.Equal("parameter.v0", region.GetProperty("type").GetString());
        var regionValue = region.GetProperty("inputs").GetProperty("value");
        // A plain parameter carries no secret flag and no generation policy at all.
        Assert.False(regionValue.TryGetProperty("secret", out _));
        Assert.False(regionValue.TryGetProperty("default", out _));

        var password = resources.GetProperty("dbpassword");
        Assert.Equal("parameter.v0", password.GetProperty("type").GetString());
        var passwordValue = password.GetProperty("inputs").GetProperty("value");
        Assert.True(passwordValue.GetProperty("secret").GetBoolean());

        // This is the fact a hard-coded secret cannot fake: AddParameter("dbpassword",
        // "hunter2", secret: true) produces secret:true with NO default.generate, so
        // the deployment target would receive a checked-in password instead of
        // minting its own. The minLength pins that the policy was configured, not
        // merely defaulted.
        var generate = passwordValue.GetProperty("default").GetProperty("generate");
        Assert.Equal(22, generate.GetProperty("minLength").GetInt32());
    }
}
