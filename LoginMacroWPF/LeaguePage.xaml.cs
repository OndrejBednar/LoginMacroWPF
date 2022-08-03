using LoginMacroWPF.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoginMacroWPF
{
    /// <summary>
    /// Interaction logic for LeaguePage.xaml
    /// </summary>
    public partial class LeaguePage : Page
    {
        public LeaguePage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Accounts.Visibility = Visibility.Hidden;
            ServerSelect.Visibility = Visibility.Visible;
            ReturnBtn.Visibility = Visibility.Hidden;
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((LeaguePageViewmodel)DataContext).SelectedServer = ((System.Collections.Generic.KeyValuePair<string, System.Collections.ObjectModel.ObservableCollection<Models.Summoner>>)((ListBoxItem)sender).Content).Value;
            Accounts.ItemsSource = ((LeaguePageViewmodel)DataContext).GetSummoners();
            Accounts.Visibility = Visibility.Visible;
            ServerSelect.Visibility = Visibility.Hidden;
            ReturnBtn.Visibility = Visibility.Visible;
        }

        private void Accounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Accounts.SelectedItem != null)
                ((LeaguePageViewmodel)DataContext).SummonerSelected();
            EditBtn.IsEnabled = true;
            LoginBtn.IsEnabled = true;
        }
    }
}
