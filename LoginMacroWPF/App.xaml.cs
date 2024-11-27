using LoginMacroWPF.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LoginMacroWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly string path = $"{Environment.CurrentDirectory}/.pwd";
        public static CredentialsRW CredentialRW { get; set; }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CredentialRW = new CredentialsRW(path);
        }

    }
}
