using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using FeWoLearning.Wpf.Exercises.Intermediate;

namespace FeWoLearning.Wpf.Tests.Intermediate;

public class Ex041_ViewModelFirstNavigationTests : WpfTestContext
{
    // Two distinct page view models with no shared base beyond object - navigation must key
    // purely off runtime type, exactly like row 023's implicit style lookup.
    private sealed class PageA
    {
        public PageA(string title) => Title = title;
        public string Title { get; }
    }

    private sealed class PageB
    {
        public PageB(string title) => Title = title;
        public string Title { get; }
    }

    private static DataTemplate BuildTemplateA()
    {
        var text = new FrameworkElementFactory(typeof(TextBlock));
        text.SetBinding(TextBlock.TextProperty, new Binding(nameof(PageA.Title)));
        return new DataTemplate(typeof(PageA)) { VisualTree = text };
    }

    // Deliberately a different visual shape (a Border wrapping the text) than PageA's
    // template, so a swap can be confirmed by shape, not only by the text inside it.
    private static DataTemplate BuildTemplateB()
    {
        var text = new FrameworkElementFactory(typeof(TextBlock));
        text.SetBinding(TextBlock.TextProperty, new Binding(nameof(PageB.Title)));
        var border = new FrameworkElementFactory(typeof(Border));
        border.AppendChild(text);
        return new DataTemplate(typeof(PageB)) { VisualTree = border };
    }

    private static IEnumerable<DependencyObject> Descendants(DependencyObject root)
    {
        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            yield return child;
            foreach (var grandchild in Descendants(child))
            {
                yield return grandchild;
            }
        }
    }

    [WpfFact]
    public void CurrentViewModel_Raises_PropertyChanged_Only_On_An_Actual_Change()
    {
        var shell = new Ex041_NavigationShell();
        var raises = 0;
        shell.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Ex041_NavigationShell.CurrentViewModel)) raises++;
        };

        var page = new PageA("Alpha");
        shell.CurrentViewModel = page;
        shell.CurrentViewModel = page; // same reference again - no event

        Assert.Equal(1, raises);
        Assert.Same(page, shell.CurrentViewModel);
    }

    [WpfFact]
    public void RegisterViewTemplate_Keys_The_Entry_By_DataTemplateKey_Not_The_Bare_Type()
    {
        var resources = new ResourceDictionary();
        var template = BuildTemplateA();

        Ex041_ViewModelFirstNavigation.RegisterViewTemplate(resources, typeof(PageA), template);

        // The load-bearing structural check: row 023's Style convention keys on the bare
        // Type. A DataTemplate does not - it needs the wrapper key type.
        Assert.True(resources.Contains(new DataTemplateKey(typeof(PageA))));
        Assert.False(resources.Contains(typeof(PageA)));
        var stored = Assert.IsType<DataTemplate>(resources[new DataTemplateKey(typeof(PageA))]);
        Assert.Equal(typeof(PageA), stored.DataType);
    }

    [WpfFact]
    public void BindShell_Declares_A_Real_Binding_Sourced_On_The_Shell()
    {
        var shell = new Ex041_NavigationShell();
        var host = new ContentControl();

        Ex041_ViewModelFirstNavigation.BindShell(host, shell);

        var binding = BindingOperations.GetBinding(host, ContentControl.ContentProperty);
        Assert.NotNull(binding);
        Assert.Equal(nameof(Ex041_NavigationShell.CurrentViewModel), binding!.Path.Path);
        Assert.Same(shell, binding.Source);
    }

    [WpfFact]
    public void The_ContentControl_Realizes_The_Template_Matching_The_Initial_View_Model()
    {
        var root = new StackPanel();
        Ex041_ViewModelFirstNavigation.RegisterViewTemplate(root.Resources, typeof(PageA), BuildTemplateA());
        Ex041_ViewModelFirstNavigation.RegisterViewTemplate(root.Resources, typeof(PageB), BuildTemplateB());

        var shell = new Ex041_NavigationShell { CurrentViewModel = new PageA("Alpha") };
        var host = new ContentControl();
        Ex041_ViewModelFirstNavigation.BindShell(host, shell);
        root.Children.Add(host);

        Layout(root);
        Pump();

        var text = Descendants(host).OfType<TextBlock>().FirstOrDefault();
        Assert.NotNull(text);
        Assert.Equal("Alpha", text!.Text);
        Assert.Empty(Descendants(host).OfType<Border>());
    }

    [WpfFact]
    public void Navigating_To_A_Different_View_Model_Type_Swaps_The_Realized_View_With_No_Manual_Reassignment()
    {
        var root = new StackPanel();
        Ex041_ViewModelFirstNavigation.RegisterViewTemplate(root.Resources, typeof(PageA), BuildTemplateA());
        Ex041_ViewModelFirstNavigation.RegisterViewTemplate(root.Resources, typeof(PageB), BuildTemplateB());

        var shell = new Ex041_NavigationShell { CurrentViewModel = new PageA("Report") };
        var host = new ContentControl();
        Ex041_ViewModelFirstNavigation.BindShell(host, shell);
        root.Children.Add(host);
        Layout(root);
        Pump();

        // A BindShell that only assigned host.Content once, at call time, would never see
        // this second navigation - host.Content (and everything realized under it) would
        // still show PageA's "Report".
        shell.CurrentViewModel = new PageB("Invoice");
        Layout(root);
        Pump();

        var text = Descendants(host).OfType<TextBlock>().FirstOrDefault();
        Assert.NotNull(text);
        Assert.Equal("Invoice", text!.Text);
        Assert.Single(Descendants(host).OfType<Border>()); // PageB's distinct visual shape
    }

    [WpfFact]
    public void Without_A_Reachable_Template_The_Content_Falls_Back_To_The_View_Models_Own_ToString()
    {
        // Contrast case, mirroring row 023: no Application here either, so a ContentControl
        // whose ancestry offers no matching DataTemplateKey has nowhere left to look.
        var root = new StackPanel();
        var shell = new Ex041_NavigationShell { CurrentViewModel = new PageA("Gamma") };
        var host = new ContentControl();
        Ex041_ViewModelFirstNavigation.BindShell(host, shell);
        root.Children.Add(host);

        Layout(root);
        Pump();

        // WPF's own built-in fallback for untemplated content: a plain TextBlock showing
        // Content.ToString() - here PageA's default, unoverridden ToString(), its full type
        // name, never the bound "Gamma" title BuildTemplateA()'s TextBlock would have shown.
        var text = Descendants(host).OfType<TextBlock>().FirstOrDefault();
        Assert.NotNull(text);
        Assert.Equal(typeof(PageA).ToString(), text!.Text);
    }
}
