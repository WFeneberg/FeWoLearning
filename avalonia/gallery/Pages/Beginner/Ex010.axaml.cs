using Avalonia.Controls;
using FeWoLearning.Avalonia.Exercises.Beginner;

namespace FeWoLearning.Avalonia.Gallery.Pages.Beginner;

public partial class Ex010 : UserControl
{
    public Ex010()
    {
        InitializeComponent();
        this.FindControl<Ex010_CompiledBinding>("Subject")!.DataContext = new Ex010_BookViewModel
        {
            Title = "Design Patterns",
            Author = new Ex010_AuthorViewModel { Name = "Erich Gamma" },
        };
    }
}
