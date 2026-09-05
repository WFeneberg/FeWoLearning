using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

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
    {
        // A plain parameter: a value the AppHost supplies, not a secret. Even so the
        // manifest publishes an indirection ({region.inputs.value}) rather than the
        // literal, because the deployment target is what fills parameters in.
        builder.AddParameter("region", "eu-west");

        // A GENERATED secret. Passing a ParameterDefault instead of a string is what
        // turns "a secret I typed in" into "a secret the target mints for itself" -
        // secret: true alone only marks a literal as sensitive, it does not stop the
        // literal from existing.
        builder.AddParameter(
            "dbpassword",
            new GenerateParameterDefault { MinLength = 22 },
            secret: true);
    }
}
