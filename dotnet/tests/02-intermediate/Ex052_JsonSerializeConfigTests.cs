using FeWoLearning.Exercises.Intermediate;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Intermediate;

public class Ex052_JsonSerializeConfigTests
{
    [Fact]
    public void Serialize_OmitsNullProperties_AndUsesCamelCase()
    {
        var settings = new JsonSerializeConfig.Settings
        {
            UserName = "wolf",
            RetryCount = 3,
            IsEnabled = true,
            Description = null,
        };

        var json = JsonSerializeConfig.Serialize(settings);

        Assert.Equal("{\"userName\":\"wolf\",\"retryCount\":3,\"isEnabled\":true}", json);
    }

    [Fact]
    public void Serialize_IncludesNonNullDescription()
    {
        var settings = new JsonSerializeConfig.Settings
        {
            UserName = "ada",
            RetryCount = 0,
            IsEnabled = false,
            Description = "seed data",
        };

        var json = JsonSerializeConfig.Serialize(settings);

        Assert.Equal(
            "{\"userName\":\"ada\",\"retryCount\":0,\"isEnabled\":false,\"description\":\"seed data\"}",
            json);
    }

    [Fact]
    public void Serialize_OmitsNullUserName()
    {
        var settings = new JsonSerializeConfig.Settings
        {
            UserName = null,
            RetryCount = 5,
            IsEnabled = true,
            Description = "no name",
        };

        var json = JsonSerializeConfig.Serialize(settings);

        Assert.Equal(
            "{\"retryCount\":5,\"isEnabled\":true,\"description\":\"no name\"}",
            json);
    }
}
