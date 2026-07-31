using System.Collections.Generic;
using FeWoLearning.Exercises.Expert;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Expert;

public class Ex094_AvaloniaMvvmViewModelTests
{
    [Fact]
    public void Execute_IncrementsCountAndRaisesPropertyChangedForBothProperties()
    {
        var vm = new AvaloniaMvvmViewModel();
        var raised = new List<string>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName!);

        Assert.Equal(0, vm.Count);
        Assert.True(vm.IncrementCommand.CanExecute(null));

        vm.IncrementCommand.Execute(null);

        Assert.Equal(1, vm.Count);
        Assert.Contains(nameof(AvaloniaMvvmViewModel.Count), raised);
        Assert.Contains(nameof(AvaloniaMvvmViewModel.CanIncrement), raised);
    }

    [Fact]
    public void PropertyChanged_DoesNotFireForUnrelatedPropertyName()
    {
        var vm = new AvaloniaMvvmViewModel();
        var raised = new List<string>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName!);

        vm.IncrementCommand.Execute(null);

        Assert.DoesNotContain("IncrementCommand", raised);
    }

    [Fact]
    public void Execute_DisablesCommandAtMaxAndRaisesCanExecuteChangedOncePerFlip()
    {
        var vm = new AvaloniaMvvmViewModel();
        var canExecuteChangedCount = 0;
        vm.IncrementCommand.CanExecuteChanged += (_, _) => canExecuteChangedCount++;

        for (var i = 0; i < AvaloniaMvvmViewModel.MaxCount; i++)
        {
            Assert.True(vm.IncrementCommand.CanExecute(null));
            vm.IncrementCommand.Execute(null);
        }

        Assert.Equal(AvaloniaMvvmViewModel.MaxCount, vm.Count);
        Assert.False(vm.CanIncrement);
        Assert.False(vm.IncrementCommand.CanExecute(null));

        // CanIncrement only flips from true -> false exactly once (on the last
        // increment), so CanExecuteChanged must have fired exactly once.
        Assert.Equal(1, canExecuteChangedCount);
    }

    [Fact]
    public void Execute_AtMax_DoesNotIncrementFurtherOrRaisePropertyChangedAgain()
    {
        var vm = new AvaloniaMvvmViewModel();
        for (var i = 0; i < AvaloniaMvvmViewModel.MaxCount; i++)
            vm.IncrementCommand.Execute(null);

        var raised = new List<string>();
        vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName!);

        vm.IncrementCommand.Execute(null);

        Assert.Equal(AvaloniaMvvmViewModel.MaxCount, vm.Count);
        Assert.Empty(raised);
    }
}
