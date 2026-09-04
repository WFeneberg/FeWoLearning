// Exercise 047 - Item Container States (intermediate).
// Goal:   Build the container half of a selectable list, and keep one selection honest.
// Drills: a ContentControl subclass with a selection state group, and a coordinator that
//         owns "exactly one selected" so no container has to know about its siblings.
// Passes: dotnet test --filter FullyQualifiedName~Ex047_
//
// This is what a ListViewItem is: a ContentControl with visual states and no opinion about
// selection policy. The policy lives one level up - which is why the same container works
// for single select, multi select and no selection at all.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace FeWoLearning.Uno.Exercises.Intermediate;

public partial class Ex047_ItemContainerStates : ContentControl
{
    /// <summary>
    /// Test fixture: a highlight behind the content, opaque only in the Selected state.
    /// </summary>
    public static readonly ControlTemplate ContainerTemplate = (ControlTemplate)XamlReader.Load(
        """
        <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                         TargetType="ContentControl">
            <Border x:Name="PART_Highlight" Opacity="0">
                <ContentPresenter Content="{TemplateBinding Content}" />
                <VisualStateManager.VisualStateGroups>
                    <VisualStateGroup x:Name="SelectionStates">
                        <VisualState x:Name="Unselected" />
                        <VisualState x:Name="Selected">
                            <VisualState.Setters>
                                <Setter Target="PART_Highlight.Opacity" Value="1" />
                            </VisualState.Setters>
                        </VisualState>
                    </VisualStateGroup>
                </VisualStateManager.VisualStateGroups>
            </Border>
        </ControlTemplate>
        """);

    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(Ex047_ItemContainerStates),
            new PropertyMetadata(false, OnIsSelectedChanged));

    /// <summary>Whether this container is selected. The container has no say in the policy.</summary>
    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    /// <summary>Enters "Selected" or "Unselected" to match <see cref="IsSelected"/>.</summary>
    public void UpdateVisualState() =>
        VisualStateManager.GoToState(this, IsSelected ? "Selected" : "Unselected", useTransitions: false);

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateVisualState();
    }

    private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
        ((Ex047_ItemContainerStates)sender).UpdateVisualState();
}

/// <summary>
/// Owns the "exactly one at a time" rule over a fixed set of containers.
/// </summary>
public sealed class Ex047_SelectionGroup
{
    private readonly List<Ex047_ItemContainerStates> _containers;

    /// <summary>
    /// Builds one container per item, each already carrying
    /// <see cref="Ex047_ItemContainerStates.ContainerTemplate"/> and the item as its
    /// Content. Nothing is selected to begin with.
    /// </summary>
    public Ex047_SelectionGroup(params object[] items) =>
        _containers = items
            .Select(item => new Ex047_ItemContainerStates
            {
                Content = item,
                Template = Ex047_ItemContainerStates.ContainerTemplate,
            })
            .ToList();

    /// <summary>The containers, in the order the items came in.</summary>
    public IReadOnlyList<Ex047_ItemContainerStates> Containers => _containers;

    /// <summary>The index of the selected container, or null when none is.</summary>
    public int? SelectedIndex
    {
        get
        {
            // Derived, not stored. A field here would be a second copy of the truth, and
            // the two would disagree the first time anybody set IsSelected directly.
            var index = _containers.FindIndex(container => container.IsSelected);
            return index < 0 ? null : index;
        }
    }

    /// <summary>
    /// Selects the container at <paramref name="index"/> and deselects every other one.
    /// An index outside the range clears the selection.
    /// </summary>
    public void Select(int index)
    {
        for (var i = 0; i < _containers.Count; i++)
        {
            // Assigning false to the others is what makes this single-select. The
            // containers themselves have no opinion - the same container type serves
            // multi-select by leaving this loop out.
            _containers[i].IsSelected = i == index;
        }
    }
}
