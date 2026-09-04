using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex026 : UserControl
{
    public Ex026()
    {
        InitializeComponent();
        this.FindControl<Ex026_ObservableCollectionUpdates>("Subject")!.DataContext =
            new Ex026_ObservableCollectionUpdatesViewModel();
    }
}
