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

            //if (RiotAPI.Api == null) InvalidApiKey.Visible = true;
        }


        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Accounts.Visibility = Visibility.Hidden;
            ServerSelect.Visibility = Visibility.Visible;
            ReturnBtn.Visibility = Visibility.Hidden;
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindowViewmodel)DataContext).SelectedServer = ((System.Collections.Generic.KeyValuePair<string, System.Collections.ObjectModel.ObservableCollection<Models.Summoner>>)((ListBoxItem)sender).Content).Value;
            Accounts.ItemsSource = ((MainWindowViewmodel)DataContext).GetSummoners();
            Accounts.Visibility = Visibility.Visible;
            ServerSelect.Visibility = Visibility.Hidden;
            ReturnBtn.Visibility = Visibility.Visible;
        }

        private void Accounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Accounts.SelectedItem != null)
            ((MainWindowViewmodel)DataContext).SummonerSelected();
            EditBtn.IsEnabled = true;
            LoginBtn.IsEnabled = true;
        }
    }
}
