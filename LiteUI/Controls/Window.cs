using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LiteUI.Controls
{
    public enum WindowBarStyle { Hidden, Normal, Big }

    public class Window : System.Windows.Window
    {
        static Window()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(typeof(Window)));
        }

        // IsFullscreen

        public static readonly DependencyProperty IsFullscreenProperty = DependencyProperty.Register(
            nameof(IsFullscreen),
            typeof(bool),
            typeof(Window),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, FullscreenChanged)
        );

        [Bindable(true)]
        [Category(nameof(LiteUI))]
        public bool IsFullscreen
        {
            get => (bool)GetValue(IsFullscreenProperty);
            set => SetValue(IsFullscreenProperty, value);
        }

        private static void FullscreenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (Window)d;
            window.MaxHeight = (bool)e.NewValue ? double.PositiveInfinity : SystemParameters.WorkArea.Height + 8;

            if (window.WindowState == WindowState.Maximized)
            {
                window.WindowState = WindowState.Minimized;
                window.WindowState = WindowState.Maximized;
            }
        }

        // BarStyle

        public static readonly DependencyProperty BarStyleProperty = DependencyProperty.Register(
            nameof(BarStyle),
            typeof(WindowBarStyle),
            typeof(Window),
            new FrameworkPropertyMetadata(WindowBarStyle.Normal, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [Bindable(true)]
        [Category(nameof(LiteUI))]
        public WindowBarStyle BarStyle
        {
            get => (WindowBarStyle)GetValue(BarStyleProperty);
            set => SetValue(BarStyleProperty, value);
        }

        // Toolbar

        public static readonly DependencyProperty ToolbarProperty = DependencyProperty.Register(
            nameof(Toolbar),
            typeof(ToolbarItemsCollection),
            typeof(Window),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [Bindable(true)]
        [Category(nameof(LiteUI))]
        public ToolbarItemsCollection Toolbar
        {
            get => (ToolbarItemsCollection)GetValue(ToolbarProperty);
            set => SetValue(ToolbarProperty, value);
        }

        //

        public Window()
        {
            // Forza aggiornamento dell'altezza massima
            IsFullscreen = false;

            // Inizializza comandi bottoni
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (_, __) => SystemCommands.CloseWindow(this)));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, (_, __) => SystemCommands.MinimizeWindow(this)));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, (_, __) => SystemCommands.MaximizeWindow(this)));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, (_, __) => SystemCommands.RestoreWindow(this)));
        }
    }
}