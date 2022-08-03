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
    /// Interaction logic for SteamPage.xaml
    /// </summary>
    public partial class SteamPage : Page
    {
        public SteamPage()
        {
            InitializeComponent();
        }

        private void Accounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Accounts.SelectedItem != null)
                ((SteamPageViewmodel)DataContext).AccountSelected();
            EditBtn.IsEnabled = true;
            LoginBtn.IsEnabled = true;
        }
    }
}
