using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LambdaCom.LiteUI
{
    public enum ToolbarPosition { Left, Right }

    public abstract class ToolbarButton : Button
    {
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description),
            typeof(string), typeof(ToolbarButton), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, DescriptionChanged));

        [Bindable(true)]
        [Category(nameof(ToolbarButton))]
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        private static void DescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (ToolbarButton)d;
            button.ToolTip = e.NewValue;
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

        protected ToolbarButton()
        {
            DockPanel.SetDock(this, Dock.Right);
        }
    }

    public class SymbolToolbarButton : ToolbarButton
    {
        public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(nameof(Symbol),
            typeof(string), typeof(SymbolToolbarButton), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, SymbolChanged));

        [Bindable(true)]
        [Category(nameof(ToolbarButton))]
        public string Symbol
        {
            get => (string)GetValue(SymbolProperty);
            set => SetValue(SymbolProperty, value);
        }

        private static void SymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (SymbolToolbarButton)d;
            button.Content = e.NewValue;
        }

        public SymbolToolbarButton() : base()
        {
            Style = (Style)FindResource("LiTEToolbarButton");
            FontFamily = new FontFamily("Segoe MDL2 Assets");
        }
    }

    public class ImageToolbarButton : ToolbarButton
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source),
            typeof(Uri), typeof(ImageToolbarButton), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, SourceChanged));

        [Bindable(true)]
        [Category(nameof(ToolbarButton))]
        public Uri Source
        {
            get => (Uri)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (ImageToolbarButton)d;
            button.Content = new Image()
            {
                Source = new BitmapImage((Uri)e.NewValue),
                Stretch = Stretch.Uniform
            };
        }

        public ImageToolbarButton() : base()
        {
            Style = (Style)FindResource("LiTEToolbarButton");
            FontFamily = new FontFamily("Segoe MDL2 Assets");
            Padding = new Thickness(5);
        }
    }
}
