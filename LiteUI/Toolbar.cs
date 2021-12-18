using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace LiteUI
{
    public class ToolbarItemsCollection : ObservableCollection<ToolbarButton> { }

    public enum ToolbarPosition { Left, Right }

    public class ToolbarButton : Button
    {
        static ToolbarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolbarButton), new FrameworkPropertyMetadata(typeof(ToolbarButton)));
        }

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(nameof(Position),
            typeof(ToolbarPosition), typeof(ToolbarButton), new FrameworkPropertyMetadata(ToolbarPosition.Right, FrameworkPropertyMetadataOptions.AffectsRender, PositionChanged));

        [Bindable(true)]
        [Category(nameof(ToolbarButton))]
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
                    DockPanel.SetDock((ToolbarButton)d, Dock.Left);
                    break;
                case ToolbarPosition.Right:
                    DockPanel.SetDock((ToolbarButton)d, Dock.Right);
                    break;
            }
        }

        public ToolbarButton()
        {
            DockPanel.SetDock(this, Dock.Right);
        }
    }
}
