using FeWoLearning.Security.Exercises.DesktopCore;

namespace FeWoLearning.Security.Tests.DesktopCore;

public class Ex051_SecretRedactionInLogsTests
{
    [Theory]
    [InlineData("password")]
    [InlineData("Password")]
    [InlineData("apiKey")]
    [InlineData("APIKEY")]
    [InlineData("authorization")]
    [InlineData("Authorization")]
    [InlineData("token")]
    [InlineData("TOKEN")]
    public void Attack_A_Value_Under_A_Sensitive_Key_Never_Appears_In_The_Output(string key)
    {
        const string secretValue = "s3cr3t-value-9f21";
        var state = new Dictionary<string, object?> { [key] = secretValue, ["requestId"] = "abc-123" };

        var redacted = Ex051_SecretRedactionInLogs.Redact("Handling request", state);

        Assert.DoesNotContain(secretValue, redacted, StringComparison.Ordinal);
    }

    [Fact]
    public void Attack_A_Sensitive_Value_Baked_Directly_Into_The_Message_Text_Is_Redacted_Too()
    {
        const string secretValue = "baked-in-secret-77ab";
        var message = $"Auth header was Bearer {secretValue}";
        var state = new Dictionary<string, object?> { ["authorization"] = secretValue };

        var redacted = Ex051_SecretRedactionInLogs.Redact(message, state);

        Assert.DoesNotContain(secretValue, redacted, StringComparison.Ordinal);
    }

    [Fact]
    public void Attack_A_Crlf_Inside_A_Value_Cannot_Forge_A_Fake_Log_Line()
    {
        var state = new Dictionary<string, object?>
        {
            ["path"] = "GET /ok\r\n2026-09-05 12:00:00 ERROR forged log entry",
        };

        var redacted = Ex051_SecretRedactionInLogs.Redact("Handled request", state);

        Assert.DoesNotContain("\r", redacted, StringComparison.Ordinal);
        Assert.DoesNotContain("\n", redacted, StringComparison.Ordinal);
    }

    [Fact]
    public void Use_NonSensitive_Keys_And_Values_Appear_In_The_Output()
    {
        var state = new Dictionary<string, object?> { ["userId"] = 42, ["path"] = "/api/orders" };

        var redacted = Ex051_SecretRedactionInLogs.Redact("Handled request", state);

        Assert.Contains("userId", redacted, StringComparison.Ordinal);
        Assert.Contains("42", redacted, StringComparison.Ordinal);
        Assert.Contains("path", redacted, StringComparison.Ordinal);
        Assert.Contains("/api/orders", redacted, StringComparison.Ordinal);
    }

    [Fact]
    public void Use_The_Messages_NonSensitive_Text_Is_Preserved_Verbatim()
    {
        const string message = "Order 55219 shipped to customer 88 via carrier X";

        var redacted = Ex051_SecretRedactionInLogs.Redact(message, new Dictionary<string, object?>());

        Assert.Contains(message, redacted, StringComparison.Ordinal);
    }

    [Fact]
    public void Use_A_Key_Named_PasswordPolicyVersion_Is_Not_Redacted()
    {
        var state = new Dictionary<string, object?> { ["passwordPolicyVersion"] = 3 };

        var redacted = Ex051_SecretRedactionInLogs.Redact("Policy check", state);

        Assert.Contains("passwordPolicyVersion", redacted, StringComparison.Ordinal);
        Assert.Contains("3", redacted, StringComparison.Ordinal);
    }
}
