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
            new FrameworkPropertyMetadata(ToolbarPosition.Left, FrameworkPropertyMetadataOptions.AffectsRender, PositionChanged));

        [Bindable(true)]
        [Category(nameof(LiteUI))]
        public ToolbarPosition Position
        {
            get => (ToolbarPosition)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        private static void PositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            switch ((ToolbarPosition)e.NewValue)
            {
                case ToolbarPosition.Left:
                    DockPanel.SetDock((ToolbarItem)d, Dock.Left);
                    break;
                case ToolbarPosition.Right:
                    DockPanel.SetDock((ToolbarItem)d, Dock.Right);
                    break;
            }
        }
    }
}
