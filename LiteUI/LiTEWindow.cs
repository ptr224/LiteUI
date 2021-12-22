using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LiteUI
{
    public enum WindowBarStyle { Hidden, Normal, Big }

    public class LiteWindow : Window
    {
        #region Theme

        internal LiteTheme Theme { get; private set; }

        /// <summary>
        /// Imposta il tema della finestra corrente.
        /// </summary>
        /// <param name="theme">Il tema da usare.</param>
        public void SetTheme(LiteTheme theme)
        {
            Theme = theme;
            LiteTheming.UpdateResources(Resources, theme);
        }

        #endregion

        #region Style and properties

        static LiteWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LiteWindow), new FrameworkPropertyMetadata(typeof(LiteWindow)));
        }

        public static readonly DependencyProperty IsFullscreenProperty = DependencyProperty.Register(
            nameof(IsFullscreen),
            typeof(bool),
            typeof(LiteWindow),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, FullscreenChanged)
        );

        [Bindable(true)]
        [Category(nameof(LiteWindow))]
        public bool IsFullscreen
        {
            get => (bool)GetValue(IsFullscreenProperty);
            set => SetValue(IsFullscreenProperty, value);
        }

        private static void FullscreenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (LiteWindow)d;
            window.MaxHeight = (bool)e.NewValue ? double.PositiveInfinity : SystemParameters.WorkArea.Height + 8;

            if (window.WindowState == WindowState.Maximized)
            {
                window.WindowState = WindowState.Minimized;
                window.WindowState = WindowState.Maximized;
            }
        }

        public static readonly DependencyProperty BarStyleProperty = DependencyProperty.Register(
            nameof(BarStyle),
            typeof(WindowBarStyle),
            typeof(LiteWindow),
            new FrameworkPropertyMetadata(WindowBarStyle.Normal, FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [Bindable(true)]
        [Category(nameof(LiteWindow))]
        public WindowBarStyle BarStyle
        {
            get => (WindowBarStyle)GetValue(BarStyleProperty);
            set => SetValue(BarStyleProperty, value);
        }

        public static readonly DependencyProperty ToolbarProperty = DependencyProperty.Register(
            nameof(Toolbar),
            typeof(ToolbarItemsCollection),
            typeof(LiteWindow),
            new FrameworkPropertyMetadata(new ToolbarItemsCollection(), FrameworkPropertyMetadataOptions.AffectsRender)
        );

        [Bindable(true)]
        [Category(nameof(LiteWindow))]
        public ToolbarItemsCollection Toolbar
        {
            get => (ToolbarItemsCollection)GetValue(ToolbarProperty);
            set => SetValue(ToolbarProperty, value);
        }

        #endregion

        public LiteWindow()
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