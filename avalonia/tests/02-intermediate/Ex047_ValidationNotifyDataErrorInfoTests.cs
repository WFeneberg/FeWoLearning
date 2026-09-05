using System.ComponentModel;
using System.Linq;
using FeWoLearning.Avalonia.Exercises.Intermediate;

namespace FeWoLearning.Avalonia.Tests.Intermediate;

public class Ex047_ValidationNotifyDataErrorInfoTests
{
    // The given defaults (Name = "Ada", Email = "ada@example.com") are already
    // valid, so a freshly constructed view model must report no errors - true
    // whether a solution validates eagerly at construction or lazily only when a
    // setter runs. Pinning "no errors before anything is touched" here would
    // overfit to one of those two equally-correct styles, so this test instead
    // pins the one thing both styles must agree on: valid input means no errors.
    [Fact]
    public void A_Freshly_Constructed_ViewModel_With_Valid_Defaults_Has_No_Errors()
    {
        var vm = new Ex047_ValidationNotifyDataErrorInfoViewModel();

        Assert.False(vm.HasErrors);
        Assert.Empty(vm.GetErrors(nameof(vm.Name)).Cast<object>());
        Assert.Empty(vm.GetErrors(nameof(vm.Email)).Cast<object>());
    }

    // Drives valid -> invalid -> valid on two properties independently, and checks
    // at every step that GetErrors distinguishes Name from Email rather than
    // reporting the same thing (or the same global flag) for both. A solution that
    // returns a single fixed error list regardless of propertyName fails here.
    [Fact]
    public void Errors_Are_Tracked_Independently_Per_Property_Through_A_Valid_Invalid_Valid_Cycle()
    {
        var vm = new Ex047_ValidationNotifyDataErrorInfoViewModel();
        var raisedFor = new List<string?>();
        vm.ErrorsChanged += (_, e) => raisedFor.Add(e.PropertyName);

        // Step 1: Name stays valid, Email becomes invalid (a real change from the
        // valid default).
        vm.Email = "not-an-email";

        Assert.True(vm.HasErrors);
        Assert.Empty(vm.GetErrors(nameof(vm.Name)).Cast<object>());
        Assert.NotEmpty(vm.GetErrors(nameof(vm.Email)).Cast<object>());

        // Step 2: Email becomes valid again - both clear.
        vm.Email = "ada@example.com";

        Assert.False(vm.HasErrors);
        Assert.Empty(vm.GetErrors(nameof(vm.Name)).Cast<object>());
        Assert.Empty(vm.GetErrors(nameof(vm.Email)).Cast<object>());

        // Step 3: Name becomes invalid (a real change, "Ada" -> "", not a
        // same-value no-op) while Email is untouched - proves Name's error does not
        // leak into, or get confused with, Email's already-clean state.
        vm.Name = "";

        Assert.True(vm.HasErrors);
        Assert.NotEmpty(vm.GetErrors(nameof(vm.Name)).Cast<object>());
        Assert.Empty(vm.GetErrors(nameof(vm.Email)).Cast<object>());

        // The mechanism itself: ErrorsChanged actually fired, naming each property
        // that was actually revalidated - not a single blanket notification, and not
        // silence from a solution that mutates state without raising the event at all.
        Assert.Contains(nameof(vm.Name), raisedFor);
        Assert.Contains(nameof(vm.Email), raisedFor);
    }

    // A second, independently-constructed view model exercising the opposite
    // starting property: guards against a solution that only wires up validation
    // for whichever property happens to be set first.
    [Fact]
    public void A_Second_ViewModel_Validates_Email_Independently_Of_Name()
    {
        var vm = new Ex047_ValidationNotifyDataErrorInfoViewModel();

        vm.Email = "still-not-an-email";
        Assert.True(vm.HasErrors);
        Assert.NotEmpty(vm.GetErrors(nameof(vm.Email)).Cast<object>());
        Assert.Empty(vm.GetErrors(nameof(vm.Name)).Cast<object>());

        vm.Email = "person@example.org";
        Assert.False(vm.HasErrors);
        Assert.Empty(vm.GetErrors(nameof(vm.Email)).Cast<object>());
    }
}
