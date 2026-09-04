using Avalonia.Controls;
using Avalonia.Media;

namespace FeWoLearning.Avalonia.Gallery;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var list = this.FindControl<ListBox>("EntryList")!;
        var host = this.FindControl<ContentControl>("Host")!;

        list.ItemsSource = GalleryCatalog.Entries;
        list.SelectionChanged += (_, _) =>
        {
            if (list.SelectedItem is not GalleryEntry entry)
                return;

            // In exercises mode Create() throws the exercise's NotImplementedException.
            // Surface it in the pane instead of killing the app, so the gallery stays
            // usable as a browser of unfinished work.
            try
            {
                host.Content = entry.Create();
            }
            catch (Exception ex)
            {
                host.Content = new TextBlock
                {
                    Text = ex.Message,
                    TextWrapping = TextWrapping.Wrap,
                };
            }
        };
    }
}
