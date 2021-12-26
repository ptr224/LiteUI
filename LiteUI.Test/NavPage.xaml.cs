using LiteUI.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiteUI.Test
{
    /// <summary>
    /// Logica di interazione per NavPage.xaml
    /// </summary>
    [PageOptions(LaunchMode = PageLaunchMode.Normal)]
    public partial class NavPage : Controls.Page
    {
        public NavPage()
        {
            InitializeComponent();
        }

        private void LitePage_Created(object sender, NavigationParams e)
        {
            Title = e.Get<string>("Title", "Default") + "C";
        }

        private void LitePage_Retrieved(object sender, NavigationParams e)
        {
            Title = e.Get<string>("Title", "Default") + "R";
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var window = GetWindow();

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
            Controls.MessageBox.Show(GetWindow(), "Ciao", MessageBoxImage.Question, MessageBoxButton.OK);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //popup.IsOpen = !popup.IsOpen;

            /*var Arr = Enum.GetValues<WindowBarStyle>();
            int j = Array.IndexOf(Arr, GetWindow().BarStyle) + 1;

            GetWindow().BarStyle = (Arr.Length == j) ? Arr[0] : Arr[j];*/

            NavigationService.Navigate<NavPage>(("Title", Title));
        }

        private void tbb_Click(object sender, RoutedEventArgs e)
        {
            pop.IsOpen = !pop.IsOpen;
        }
    }
}
