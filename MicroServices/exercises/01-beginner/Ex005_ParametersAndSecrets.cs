using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Model one plain configuration parameter and one GENERATED secret.
/// Drills: `AddParameter`, `ParameterResource.Secret`, and the
///         `GenerateParameterDefault` that makes the manifest carry an
///         `inputs.value.default.generate` policy instead of a baked-in literal.
/// Passes: "region" is a non-secret parameter with the literal "eu-west" and no
///         generate policy; "dbpassword" is a secret parameter whose manifest
///         entry has inputs.value.secret = true and
///         inputs.value.default.generate.minLength = 22.
/// </summary>
public static class Ex005_ParametersAndSecrets
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: add a plain parameter 'region' with the value 'eu-west', and a "
            + "secret parameter 'dbpassword' whose value is GENERATED with a "
            + "minimum length of 22.");
}
