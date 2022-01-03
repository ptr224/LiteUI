using LiteUI.Controls;
using LiteUI.Navigation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiteUI.Test
{
    /// <summary>
    /// Logica di interazione per NavPage.xaml
    /// </summary>
    [PageOptions(LaunchMode = PageLaunchMode.Normal)]
    public partial class NavPage : Controls.Page, IDisposable
    {
        public NavPage()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            frame.Dispose();
        }

        private void LitePage_Created(object sender, NavigationParams e)
        {
            Title = $"{e.Get(1, "X")} -> {e.Id}C";
        }

        private void LitePage_Retrieved(object sender, NavigationParams e)
        {
            Title = $"{e.Get(1, Title)} {(e.Id == NavigationParams.DEFAULT_ID ? "<-" : "->")} {e.Id}R";
        }

        private void Page_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = frame.CancelLeaving();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var window = System.Windows.Window.GetWindow(this);

            if (window is null)
                return;

            var me = (ListBox)sender;
            var theme = me.SelectedIndex switch
            {
                0 => Theme.Light,
                1 => Theme.Dark,
                2 => new()
                {
                    Active = Color.FromRgb(0x00, 0xFF, 0x00),
                    Inactive = Color.FromRgb(0xFF, 0x00, 0x00),
                    Background = Color.FromRgb(0x00, 0x00, 0x00),
                    AccentForeground = Color.FromRgb(0x00, 0x00, 0x00),
                    AccentBackground = Color.FromRgb(0xFF, 0xFF, 0x00)
                },
                3 => new()
                {
                    Active = Color.FromRgb(0x00, 0xFF, 0xFF),
                    Inactive = Color.FromRgb(0x00, 0x7F, 0x7F),
                    Background = Color.FromRgb(0x00, 0x00, 0x00),
                    AccentForeground = Color.FromRgb(0x00, 0x00, 0x00),
                    AccentBackground = Color.FromRgb(0x00, 0xFF, 0xFF)
                },
                _ => null
            };

            if (theme is not null)
                Theming.SetTheme(window, theme);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Controls.MessageBox.Show(System.Windows.Window.GetWindow(this), "Ciao", MessageBoxImage.Question, MessageBoxButton.OK);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //popup.IsOpen = !popup.IsOpen;

            /*var Arr = Enum.GetValues<WindowBarStyle>();
            int j = Array.IndexOf(Arr, GetWindow().BarStyle) + 1;

            GetWindow().BarStyle = (Arr.Length == j) ? Arr[0] : Arr[j];*/

            NavigationService.Navigate<NavPage>(1, (1, Title));
        }

        private void tbb_Click(object sender, RoutedEventArgs e)
        {
            //pop.IsOpen = !pop.IsOpen;
            var button = sender as Button;
            var item = button.Parent as ToolbarItem;
            item.Position = item.Position == ToolbarPosition.Left ? ToolbarPosition.Right : ToolbarPosition.Left;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack((1, Title));
        }
    }
}
