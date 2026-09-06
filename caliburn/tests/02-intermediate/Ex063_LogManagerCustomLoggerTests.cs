using Caliburn.Micro;
using FeWoLearning.Caliburn.Exercises.Intermediate;

namespace FeWoLearning.Caliburn.Tests.Intermediate;

public class Ex063_LogManagerCustomLoggerTests : CaliburnCoreContext
{
    [Fact]
    public void Info_Applies_The_Format_Contract_Rather_Than_Storing_The_Raw_Format_String()
    {
        var log = new Ex063_RecordingLog();

        log.Info("Processed {0} items", 3);

        // A stub that stores format/args unformatted (or ignores args entirely) fails right here.
        Assert.Equal(["Processed 3 items"], log.InfoMessages);
    }

    [Fact]
    public void Warn_Applies_The_Format_Contract_The_Same_Way_As_Info()
    {
        var log = new Ex063_RecordingLog();

        log.Warn("{0} is below the {1} threshold", 2, 5);

        Assert.Equal(["2 is below the 5 threshold"], log.WarnMessages);
    }

    [Fact]
    public void Error_Records_The_Exception_Object_Itself_Not_A_Stringified_Message()
    {
        var log = new Ex063_RecordingLog();
        var exception = new InvalidOperationException("boom");

        log.Error(exception);

        // A stub that tries to squeeze this into InfoMessages/WarnMessages instead (there is no
        // format string here to format) fails right here - Errors must hold the exception itself.
        Assert.Same(exception, Assert.Single(log.Errors));
    }

    [Fact]
    public void Install_Makes_LogManager_GetLog_Return_The_Same_Logger_For_Any_Type()
    {
        var log = new Ex063_RecordingLog();
        var subject = new Ex063_LogManagerCustomLogger();

        subject.Install(log);

        // Install ignores the requested Type entirely - a stub that only wires up ONE specific
        // type (or that leaves LogManager.GetLog untouched) fails at least one of these two.
        Assert.Same(log, LogManager.GetLog(typeof(Ex063_LogManagerCustomLoggerTests)));
        Assert.Same(log, LogManager.GetLog(typeof(string)));
    }

    [Fact]
    public void A_Real_Consumer_Reaches_The_Installed_Logger_Through_LogManager_GetLog_Itself()
    {
        var log = new Ex063_RecordingLog();
        new Ex063_LogManagerCustomLogger().Install(log);

        // Ex063_Worker calls LogManager.GetLog(typeof(Ex063_Worker)) ITSELF - this proves the
        // installed delegate is genuinely reached through the real static, not through some
        // private reference the test happened to keep.
        new Ex063_Worker().Process(3);

        Assert.Equal(["Processed 3 items"], log.InfoMessages);
        Assert.Empty(log.WarnMessages);
    }

    [Fact]
    public void A_Real_Consumer_Warns_Through_The_Same_Installed_Logger_When_Count_Is_Zero()
    {
        var log = new Ex063_RecordingLog();
        new Ex063_LogManagerCustomLogger().Install(log);

        new Ex063_Worker().Process(0);

        Assert.Equal(["Nothing to process"], log.WarnMessages);
    }

    [Fact]
    public void A_Real_Consumer_Reports_A_Failure_Through_Error_Carrying_The_Real_Exception()
    {
        var log = new Ex063_RecordingLog();
        new Ex063_LogManagerCustomLogger().Install(log);
        var exception = new InvalidOperationException("disk full");

        new Ex063_Worker().Fail(exception);

        Assert.Same(exception, Assert.Single(log.Errors));
    }
}
