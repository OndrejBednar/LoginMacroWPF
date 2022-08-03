using LoginMacroWPF.Viewmodels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LoginMacroWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            PlatformFrame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
            //if (RiotAPI.Api == null) InvalidApiKey.Visible = true;
        }


        private void LeagueClicked(object sender, RoutedEventArgs e)
        {
            PlatformFrame.Navigate(new System.Uri("LeaguePage.xaml", System.UriKind.RelativeOrAbsolute));
        }

        private void SteamClicked(object sender, RoutedEventArgs e)
        {
            PlatformFrame.Navigate(new System.Uri("SteamPage.xaml", System.UriKind.RelativeOrAbsolute));
        }
    }
}
