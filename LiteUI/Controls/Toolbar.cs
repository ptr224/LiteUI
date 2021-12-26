using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace LiteUI.Controls
{
    public class ToolbarItemsCollection : ObservableCollection<ToolbarItem> { }

    public enum ToolbarPosition { Left, Right }

    public sealed class ToolbarItem : Decorator
    {
        static ToolbarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolbarItem), new FrameworkPropertyMetadata(typeof(ToolbarItem)));
        }

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            nameof(Position),
            typeof(ToolbarPosition),
            typeof(ToolbarItem),
            new FrameworkPropertyMetadata(ToolbarPosition.Left, FrameworkPropertyMetadataOptions.AffectsRender));

        [Bindable(true)]
        [Category(nameof(LiteUI))]
        public ToolbarPosition Position
        {
            get => (ToolbarPosition)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }
    }
}
