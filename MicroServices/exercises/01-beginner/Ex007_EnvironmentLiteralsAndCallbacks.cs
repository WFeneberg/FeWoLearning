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
/// Passes: REGION comes from the LITERAL overload and only from it, API_URL and
///         MODE come from the callback and only from it, API_URL is an
///         EndpointReference to the "http" endpoint of "api" (not a hard-coded
///         URL), and MODE is "run" under a run context but "publish" under a
///         publish context.
/// Note:   Measured - EnvironmentAnnotation, the annotation a LITERAL writes,
///         derives from EnvironmentCallbackAnnotation and is internal. So counting
///         EnvironmentCallbackAnnotations cannot tell a literal from a callback,
///         and neither can inspecting the merged environment: writing REGION as a
///         plain string from INSIDE the callback produces the same merged result.
///         The exercise is graded by running each mechanism's annotations
///         separately, which is what makes "literal versus callback" gradeable in
///         both directions.
/// </summary>
public static class Ex007_EnvironmentLiteralsAndCallbacks
{
    public static void Configure(IDistributedApplicationBuilder builder)
        => throw new NotImplementedException(
            "TODO: ex007 - add container 'api' (image nginx) with an http endpoint "
            + "named 'http' on target port 8080, then container 'worker' (image "
            + "busybox) carrying REGION as the literal 'eu-west', plus a callback "
            + "that sets API_URL to the 'http' endpoint of 'api' and MODE to "
            + "'publish' in publish mode or 'run' otherwise.");
}
