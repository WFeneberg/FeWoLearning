using Aspire.Hosting;

namespace FeWoLearning.MicroServices.Exercises.Beginner;

/// <summary>
/// Goal:   Give one worker both kinds of environment variable: a literal decided
///         when the model is built, and computed ones decided when the callback
///         runs.
/// Drills: `WithEnvironment(name, value)` versus `WithEnvironment(callback)`. The
///         callback receives an EnvironmentCallbackContext, which is what lets it
///         read another resource's endpoint and ask which operation the AppHost is
///         performing - neither of which a literal can express.
/// Passes: Running the worker's environment callbacks yields REGION as the fixed
///         string "eu-west" in every context, API_URL as an EndpointReference to
///         the "http" endpoint of "api" (not a hard-coded URL), and MODE as "run"
///         under a run context but "publish" under a publish context.
/// Note:   Measured - EnvironmentAnnotation, the annotation a LITERAL writes,
///         derives from EnvironmentCallbackAnnotation. So counting
///         EnvironmentCallbackAnnotations cannot tell a literal from a callback,
///         and this exercise is graded by running the callbacks instead.
/// </summary>
public static class Ex007_EnvironmentLiteralsAndCallbacks
{
    public static void Configure(IDistributedApplicationBuilder builder)
    {
        var api = builder.AddContainer("api", "nginx")
                         .WithHttpEndpoint(targetPort: 8080, name: "http");

        builder.AddContainer("worker", "busybox")
               // A literal: the value is decided here, once, and is the same string
               // in every context the model is later evaluated in.
               .WithEnvironment("REGION", "eu-west")
               // A callback: it runs later, once per EnvironmentCallbackContext, and
               // can therefore reach things that do not exist yet at model-build
               // time - another resource's endpoint, and which operation the AppHost
               // is performing.
               .WithEnvironment(context =>
               {
                   context.EnvironmentVariables["API_URL"] = api.GetEndpoint("http");
                   context.EnvironmentVariables["MODE"] =
                       context.ExecutionContext.IsPublishMode ? "publish" : "run";
               });
    }
}
