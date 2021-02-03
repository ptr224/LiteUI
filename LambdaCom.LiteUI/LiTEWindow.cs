using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace LambdaCom.LiteUI
{
    public enum LiTEWindowTheme { Light, Dark }

    public enum WindowBarStyle { Hidden, HiddenMaximized, Normal, Big }

    public class ToolbarItemsCollection : ObservableCollection<ToolbarButton> { }

    public class LiTEWindow : Window
    {
        #region Style and properties

        static LiTEWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LiTEWindow), new FrameworkPropertyMetadata(typeof(LiTEWindow)));
        }

        public static readonly DependencyProperty IsFullscreenProperty = DependencyProperty.Register(nameof(IsFullscreen),
            typeof(bool), typeof(LiTEWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, FullscreenChanged));

        [Bindable(true)]
        [Category(nameof(LiTEWindow))]
        public bool IsFullscreen
        {
            get => (bool)GetValue(IsFullscreenProperty);
            set => SetValue(IsFullscreenProperty, value);
        }

        private static void FullscreenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (LiTEWindow)d;
            window.MaxHeight = (bool)e.NewValue ? double.PositiveInfinity : SystemParameters.WorkArea.Height + 8;

            if (window.WindowState == WindowState.Maximized)
            {
                window.WindowState = WindowState.Minimized;
                window.WindowState = WindowState.Maximized;
            }
        }

        public static readonly DependencyProperty IsTransparentProperty = DependencyProperty.Register(nameof(IsTransparent),
            typeof(bool), typeof(LiTEWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        [Bindable(true)]
        [Category(nameof(LiTEWindow))]
        public bool IsTransparent
        {
            get => (bool)GetValue(IsTransparentProperty);
            set => SetValue(IsTransparentProperty, value);
        }

        public static readonly DependencyProperty BarStyleProperty = DependencyProperty.Register(nameof(BarStyle),
            typeof(WindowBarStyle), typeof(LiTEWindow), new FrameworkPropertyMetadata(WindowBarStyle.Normal, FrameworkPropertyMetadataOptions.AffectsRender));

        [Bindable(true)]
        [Category(nameof(LiTEWindow))]
        public WindowBarStyle BarStyle
        {
            get => (WindowBarStyle)GetValue(BarStyleProperty);
            set => SetValue(BarStyleProperty, value);
        }

        public static readonly DependencyProperty ToolbarProperty = DependencyProperty.Register(nameof(Toolbar),
            typeof(ToolbarItemsCollection), typeof(LiTEWindow), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, ToolbarChanged));

        [Bindable(true)]
        [Category(nameof(LiTEWindow))]
        public ToolbarItemsCollection Toolbar
        {
            get => (ToolbarItemsCollection)GetValue(ToolbarProperty);
            set => SetValue(ToolbarProperty, value);
        }

        private static void ToolbarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (LiTEWindow)d;

            if (e.OldValue is ToolbarItemsCollection oldToolbar)
                oldToolbar.CollectionChanged -= window.ToolbarItemsCollectionChanged;

            window.InitializeToolbar();
        }

        #endregion

        #region Theme

        private static Color[] GetThemeColors(LiTEWindowTheme theme)
            => theme switch
            {
                LiTEWindowTheme.Light => new[] { Color.FromRgb(0x26, 0x26, 0x26), Color.FromRgb(0x7F, 0x7F, 0x7F), Color.FromRgb(0xFF, 0xFF, 0xFF) },
                LiTEWindowTheme.Dark => new[] { Color.FromRgb(0xFF, 0xFF, 0xFF), Color.FromRgb(0x7F, 0x7F, 0x7F), Color.FromRgb(0x00, 0x00, 0x00) },
                _ => throw new ArgumentException("Theme not found")
            };

        // Calcola valori per tutte le chiavi dei colori
        private static void SetTheme(ResourceDictionary resources, Color active, Color inactive, Color background)
        {
            resources["ActiveColor"] = new SolidColorBrush(active);
            resources["InactiveColor"] = new SolidColorBrush(inactive);

            // Sottrai da background il 20% di active
            byte sr = (byte)(active.R * 0.2 + background.R * 0.8);
            byte sg = (byte)(active.G * 0.2 + background.G * 0.8);
            byte sb = (byte)(active.B * 0.2 + background.B * 0.8);

            var special = Color.FromRgb(sr, sg, sb);

            // Usa il colore più brillante per evidenziare e il meno per lo sfondo
            var ac = System.Drawing.Color.FromArgb(0xFF, active.R, active.G, active.B);
            var bc = System.Drawing.Color.FromArgb(0xFF, background.R, background.G, background.B);

            if (ac.GetBrightness() > bc.GetBrightness())
            {
                resources["BackgroundColor"] = new SolidColorBrush(Color.FromArgb(0xFF, special.R, special.G, special.B));
                resources["HighlightedColor"] = new SolidColorBrush(Color.FromArgb(0x90, special.R, special.G, special.B));
                resources["WindowBaseColor"] = new SolidColorBrush(Color.FromArgb(0xFF, background.R, background.G, background.B));
                resources["WindowColor"] = new SolidColorBrush(Color.FromArgb(0xC7, background.R, background.G, background.B));
            }
            else
            {
                resources["BackgroundColor"] = new SolidColorBrush(Color.FromArgb(0xFF, background.R, background.G, background.B));
                resources["HighlightedColor"] = new SolidColorBrush(Color.FromArgb(0x90, background.R, background.G, background.B));
                resources["WindowBaseColor"] = new SolidColorBrush(Color.FromArgb(0xFF, special.R, special.G, special.B));
                resources["WindowColor"] = new SolidColorBrush(Color.FromArgb(0xC7, special.R, special.G, special.B));
            }
        }

        /// <summary>
        /// Imposta il tema globale dell'applicazione.
        /// </summary>
        /// <param name="active">Il colore degli elementi attivi.</param>
        /// <param name="inactive">Il colore degli elementi inattivi.</param>
        /// <param name="background">Il colore di base dello sfondo.</param>
        public static void SetGlobalColors(Color active, Color inactive, Color background)
        {
            SetTheme(Application.Current.Resources, active, inactive, background);
        }

        /// <summary>
        /// Imposta il tema globale dell'applicazione.
        /// </summary>
        /// <param name="theme">Il tema da usare.</param>
        public static void SetGlobalColors(LiTEWindowTheme theme)
        {
            var colors = GetThemeColors(theme);
            SetTheme(Application.Current.Resources, colors[0], colors[1], colors[2]);
        }

        internal Color[] Colors { get; private set; }

        /// <summary>
        /// Imposta il tema della finestra corrente.
        /// </summary>
        /// <param name="active">Il colore degli elementi attivi.</param>
        /// <param name="inactive">Il colore degli elementi inattivi.</param>
        /// <param name="background">Il colore di base dello sfondo.</param>
        public void SetColors(Color active, Color inactive, Color background)
        {
            Colors = new[] { active, inactive, background };
            SetTheme(Resources, active, inactive, background);
        }

        /// <summary>
        /// Imposta il tema della finestra corrente.
        /// </summary>
        /// <param name="theme">Il tema da usare.</param>
        public void SetColors(LiTEWindowTheme theme)
        {
            var colors = GetThemeColors(theme);

            Colors = colors;
            SetTheme(Resources, colors[0], colors[1], colors[2]);
        }

        #endregion

        public LiTEWindow()
        {
            DataContext = this;

            // Forza aggiornamento dell'altezza massima
            IsFullscreen = false;
        }

        public override async void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Gestisci pressione bottoni barra
            ((Button)GetTemplateChild("maximize")).Click += (_, __) =>
                WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;

            ((Button)GetTemplateChild("close")).Click += (_, __) =>
                Close();

            ((Button)GetTemplateChild("minimize")).Click += (_, __) =>
                WindowState = WindowState.Minimized;

            // Inizializza toolbar
            InitializeToolbar();

            // Attiva l'effetto blur (su thread UI per non rallentare)
            await Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                var windowHelper = new WindowInteropHelper(this);
                AcrylicHelper.EnableBlur(windowHelper.Handle);
                ContentRendered += OnContentRendered;

                void OnContentRendered(object sender, EventArgs e)
                {
                    if (SizeToContent != SizeToContent.Manual)
                    {
                        InvalidateMeasure();
                    }

                    ContentRendered -= OnContentRendered;
                }
            }));
        }

        private void InitializeToolbar()
        {
            // Verifica che la finestra abbia già il tema applicato
            if (GetTemplateChild("toolbar") is DockPanel toolbar)
            {
                // Ripulisci toolbar
                toolbar.Children.Clear();

                // Aggiungi elementi già presenti
                if (Toolbar != null)
                {
                    foreach (var item in Toolbar)
                        toolbar.Children.Add(item);

                    Toolbar.CollectionChanged += ToolbarItemsCollectionChanged;
                }
            }
        }

        private void ToolbarItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var toolbar = (DockPanel)GetTemplateChild("toolbar");

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    toolbar.Children.Insert(e.NewStartingIndex, (ToolbarButton)e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    toolbar.Children[e.OldStartingIndex] = (ToolbarButton)e.NewItems[0];
                    break;
                case NotifyCollectionChangedAction.Remove:
                    toolbar.Children.RemoveAt(e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
                    toolbar.Children.RemoveAt(e.OldStartingIndex);
                    toolbar.Children.Insert(e.NewStartingIndex, (ToolbarButton)e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    toolbar.Children.Clear();
                    break;
            }
        }
    }
}
