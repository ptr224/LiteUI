using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LambdaCom.LiteUI.Test
{
    /// <summary>
    /// Logica di interazione per NavPage.xaml
    /// </summary>
    [PageOptions(LaunchMode = PageLaunchMode.Normal)]
    public partial class NavPage : LiTEPage
    {
        public NavPage()
        {
            InitializeComponent();
        }

        protected override void Created(NavigationParams extras)
        {
            base.Created(extras);
            Title = extras.Get<string>("Title", "Default") + "C";
        }

        protected override void Retrieved(NavigationParams extras)
        {
            base.Retrieved(extras);
            Title = extras.Get<string>("Title", "Default") + "R";
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var me = (ListBox)sender;

            switch (me.SelectedIndex)
            {
                case 0:
                    GetWindow()?.SetColors(LiTEWindowTheme.Light);
                    return;
                case 1:
                    GetWindow()?.SetColors(LiTEWindowTheme.Dark);
                    return;
                case 2:
                    GetWindow()?.SetColors(Color.FromRgb(0x00, 0xFF, 0x00), Color.FromRgb(0xFF, 0x00, 0x00), Color.FromRgb(0x00, 0x00, 0x00));
                    break;
                case 3:
                    GetWindow()?.SetColors(Color.FromRgb(0x00, 0xFF, 0xFF), Color.FromRgb(0x00, 0x7F, 0x7F), Color.FromRgb(0x00, 0x00, 0x00));
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LiTEMessageBox.Show(GetWindow(), "Ciao", MessageBoxImage.Information, MessageBoxButton.YesNoCancel);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate<NavPage>(("Title", Title));
            /*Application.Current.Resources["WindowBarHeight"] = 40d;
            Application.Current.Resources["WindowButtonWidth"] = 40d;
            Application.Current.Resources["WindowContentMargin"] = new Thickness(10, 40, 10, 10);*/

            popup.IsOpen = !popup.IsOpen;
        }

        private void tbb_Click(object sender, RoutedEventArgs e)
        {
            pop.IsOpen = !pop.IsOpen;
        }
    }
}
