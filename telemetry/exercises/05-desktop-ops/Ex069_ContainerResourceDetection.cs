using OpenTelemetry.Resources;

namespace FeWoLearning.Telemetry.Exercises.DesktopOps;

// Exercise 069 — ContainerResourceDetection (desktop-ops). 🐳
// Goal:   Let a process work out that it is in a container, and write its logs where a
//         container's log driver will find them.
// Drills: cgroup v1 and v2 container-id detection, OTEL_SERVICE_NAME, one-line JSON on
//         stdout.
// Passes: a cgroup v1 file yields the container id;
//         a cgroup v2 file - which contains no id at all - falls back to mountinfo and
//                     yields it from there;
//         neither available means the attribute is ABSENT, not "unknown";
//         the service name comes from the environment;
//         each log record is ONE line of valid JSON with its fields as members;
//         and 🐳 a real container's own files yield that container's real id.
//
// The second clause contradicts almost every recipe written about this, and it was
// measured here rather than read. The classic advice is "parse /proc/self/cgroup for a
// 64-hex segment", which worked under cgroup v1. Under cgroup v2 - which is what current
// Docker uses, including Docker Desktop on this machine - that file reads exactly:
//
//     0::/
//
// No id, no path, nothing. The container id is instead findable in /proc/self/mountinfo,
// in a line naming a path like /docker/containers/<64 hex>/resolv.conf. A detector that
// only knows the old recipe silently reports no container id on every modern host, and
// every span it produces is missing the attribute that says which replica emitted it.
//
// The third clause is the paired half and it matters more than it looks. "unknown" is a
// value: it groups, it charts, and it makes a thousand processes that are not in
// containers look like one container called unknown. Absent is the honest answer, and
// every backend already knows how to render it.
//
// The fifth is what a container log driver needs. It reads stdout line by line and parses
// each as JSON; a record spread over several lines becomes several broken records, and a
// rendered sentence becomes one field called "log" containing everything, which is row
// 001's problem arriving through the floor.
public static class Ex069_ContainerResourceDetection
{
    /// <summary>The conventional attribute naming the container.</summary>
    public const string ContainerIdAttribute = "container.id";

    /// <summary>The variable an operator sets to name the service.</summary>
    public const string ServiceNameVariable = "OTEL_SERVICE_NAME";

    /// <summary>The conventional attribute the service name lands on.</summary>
    public const string ServiceNameAttribute = "service.name";

    /// <summary>
    /// Work out the container id from the two files a Linux process can read about itself.
    ///
    /// <paramref name="cgroupContents"/> is <c>/proc/self/cgroup</c>: under v1 its lines
    /// end in a 64-character hex id. Under v2 it is just <c>0::/</c> and carries nothing,
    /// so fall back to <paramref name="mountInfoContents"/> - <c>/proc/self/mountinfo</c> -
    /// where a line names a path containing <c>/containers/&lt;64 hex&gt;/</c>.
    ///
    /// Return null when neither says anything. Either argument may be null, because a
    /// process not on Linux has no such file.
    /// </summary>
    public static string? DetectContainerId(string? cgroupContents, string? mountInfoContents) =>
        throw new NotImplementedException(
            "TODO: Ex069 - find the container id in cgroup v1, or in mountinfo under v2");

    /// <summary>
    /// Build a resource naming the service from <see cref="ServiceNameVariable"/> and, when
    /// there is one, the container.
    ///
    /// No container means no <see cref="ContainerIdAttribute"/> at all. Start from an empty
    /// resource.
    /// </summary>
    public static Resource BuildResource(string? cgroupContents, string? mountInfoContents) =>
        throw new NotImplementedException(
            "TODO: Ex069 - name the service from the environment and the container only if there is one");

    /// <summary>
    /// Render one record as ONE line of JSON for a container log driver: a
    /// <c>level</c> member, a <c>template</c> member carrying the message template
    /// unrendered, and every field as its own member keeping its type.
    /// </summary>
    public static string ToJsonLine(
        string level, string messageTemplate, IReadOnlyDictionary<string, object?> fields) =>
        throw new NotImplementedException("TODO: Ex069 - one line, valid JSON, fields as members");
}
