using System.Text.Json;
using DotNet.Testcontainers.Builders;
using FeWoLearning.Telemetry.Exercises.DesktopOps;
using FeWoLearning.Telemetry.Tests.Harness;

namespace FeWoLearning.Telemetry.Tests.DesktopOps;

public class Ex069_ContainerResourceDetectionTests
{
    private const string Id =
        "7f1c2b3a4d5e6f708192a3b4c5d6e7f8091a2b3c4d5e6f708192a3b4c5d6e7f8";

    /// <summary>What /proc/self/cgroup looks like under cgroup v1 inside Docker.</summary>
    private const string CgroupV1 = $"""
        12:pids:/docker/{Id}
        11:hugetlb:/docker/{Id}
        0::/docker/{Id}
        """;

    /// <summary>
    /// What it looks like under cgroup v2 - which is what current Docker uses, including
    /// Docker Desktop on this machine. Measured, not quoted: it really is this and nothing
    /// else.
    /// </summary>
    private const string CgroupV2 = "0::/\n";

    /// <summary>The line in /proc/self/mountinfo that still carries the id under v2.</summary>
    private const string MountInfo = $"""
        1690 1655 0:157 / / rw,relatime - overlay overlay rw,lowerdir=/var/lib/docker/overlay2/l/ABC
        1701 1690 8:32 /docker/containers/{Id}/resolv.conf /etc/resolv.conf rw,relatime - ext4 /dev/sdc rw
        1702 1690 8:32 /docker/containers/{Id}/hostname /etc/hostname rw,relatime - ext4 /dev/sdc rw
        """;

    private static string? Attribute(OpenTelemetry.Resources.Resource resource, string key) =>
        resource.Attributes.FirstOrDefault(a => a.Key == key).Value?.ToString();

    private static T WithServiceName<T>(string? name, Func<T> body)
    {
        var previous = Environment.GetEnvironmentVariable(
            Ex069_ContainerResourceDetection.ServiceNameVariable);
        Environment.SetEnvironmentVariable(
            Ex069_ContainerResourceDetection.ServiceNameVariable, name);

        try
        {
            return body();
        }
        finally
        {
            Environment.SetEnvironmentVariable(
                Ex069_ContainerResourceDetection.ServiceNameVariable, previous);
        }
    }

    [Fact]
    public void A_cgroup_v1_file_yields_the_container_id()
    {
        Assert.Equal(Id, Ex069_ContainerResourceDetection.DetectContainerId(CgroupV1, null));
    }

    [Fact]
    public void Adversarial_A_Under_cgroup_v2_the_id_comes_from_mountinfo()
    {
        // The row's whole point, and the thing every recipe on the subject gets wrong. Under
        // v2 /proc/self/cgroup is literally "0::/" - there is no id in it to parse. A
        // detector that only knows the classic recipe returns null here, silently, on every
        // modern host, and every span it produces is missing the attribute that says which
        // replica emitted it.
        Assert.Equal(Id, Ex069_ContainerResourceDetection.DetectContainerId(CgroupV2, MountInfo));
    }

    [Fact]
    public void Adversarial_B_Not_in_a_container_means_the_attribute_is_absent()
    {
        // The paired half, and it matters more than it looks. "unknown" is a value: it
        // groups, it charts, and it makes a thousand desktop installations look like one
        // very busy container called unknown. Absent is the honest answer and every backend
        // already renders it.
        Assert.Null(Ex069_ContainerResourceDetection.DetectContainerId(null, null));
        Assert.Null(Ex069_ContainerResourceDetection.DetectContainerId(CgroupV2, "no mounts here"));

        var resource = WithServiceName(
            "desktop-app", () => Ex069_ContainerResourceDetection.BuildResource(null, null));

        Assert.DoesNotContain(
            resource.Attributes,
            a => a.Key == Ex069_ContainerResourceDetection.ContainerIdAttribute);
        Assert.Equal(
            "desktop-app",
            Attribute(resource, Ex069_ContainerResourceDetection.ServiceNameAttribute));
    }

    [Fact]
    public void The_resource_names_the_service_from_the_environment_and_the_container_when_there_is_one()
    {
        var resource = WithServiceName(
            "orders-api",
            () => Ex069_ContainerResourceDetection.BuildResource(CgroupV2, MountInfo));

        Assert.Equal(
            "orders-api",
            Attribute(resource, Ex069_ContainerResourceDetection.ServiceNameAttribute));
        Assert.Equal(
            Id, Attribute(resource, Ex069_ContainerResourceDetection.ContainerIdAttribute));
    }

    [Fact]
    public void Adversarial_C_A_record_is_one_line_of_JSON_with_its_fields_as_typed_members()
    {
        // What a container log driver actually needs. It reads stdout line by line and
        // parses each line as JSON. A record spread over several lines becomes several
        // broken records; a rendered sentence becomes one member called "log" carrying
        // everything, which is row 001's problem arriving through the floor.
        var line = Ex069_ContainerResourceDetection.ToJsonLine(
            "Information",
            "Order {OrderId} for {Customer} settled at {Amount}",
            new Dictionary<string, object?>
            {
                ["OrderId"] = 4711,
                ["Customer"] = "ada",
                ["Amount"] = 12.5,
                ["Paid"] = true,
            });

        Assert.DoesNotContain('\n', line.TrimEnd('\n'));
        Assert.DoesNotContain('\r', line);

        var root = JsonDocument.Parse(line).RootElement;

        Assert.Equal("Information", root.GetProperty("level").GetString());

        // Unrendered: the template is the thing you group ten million records by.
        Assert.Equal(
            "Order {OrderId} for {Customer} settled at {Amount}",
            root.GetProperty("template").GetString());

        // Each field its own member, keeping its type - a number that arrives as a string
        // cannot be summed, ranged or plotted by the backend that receives it.
        Assert.Equal(JsonValueKind.Number, root.GetProperty("OrderId").ValueKind);
        Assert.Equal(4711, root.GetProperty("OrderId").GetInt32());
        Assert.Equal("ada", root.GetProperty("Customer").GetString());
        Assert.Equal(12.5, root.GetProperty("Amount").GetDouble());
        Assert.Equal(JsonValueKind.True, root.GetProperty("Paid").ValueKind);
    }

    [Fact]
    public async Task Container_A_real_containers_own_files_yield_its_real_id()
    {
        // 🐳 The fact the whole row rests on. Everything above is a string I typed; this one
        // asks a running container what it can see about itself and checks the answer
        // against the id Docker handed out.
        //
        // If the "parse /proc/self/cgroup" recipe were sufficient, this would be redundant.
        // It is not, and that is why it is here.
        ContainerGate.SkipUnlessEnabled();

        await using var container = new ContainerBuilder("alpine:3.21")
            .WithEntrypoint("/bin/sh", "-c", "sleep infinity")
            .Build();

        await container.StartAsync();

        var cgroup = await container.ExecAsync(["cat", "/proc/self/cgroup"]);
        var mountInfo = await container.ExecAsync(["cat", "/proc/self/mountinfo"]);

        Assert.Equal(0, cgroup.ExitCode);
        Assert.Equal(0, mountInfo.ExitCode);

        var detected = Ex069_ContainerResourceDetection.DetectContainerId(
            cgroup.Stdout, mountInfo.Stdout);

        Assert.Equal(container.Id, detected);
    }
}
