using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LoginMacroWPF.Models
{
    public class Summoner : IAccount, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Summoner(string[] credentials)
        {
            Credentials = credentials;
        }
        string _soloQ = "", _flexQ = "";

        public string Username { get => Credentials[3].Split(':')[1]; }
        public string Password { get => Credentials[4].Split(':')[1]; }
        public Platforms Platform { get => (Platforms)Enum.Parse(typeof(Platforms), Credentials[0]); }
        public string[] Credentials { get; set; }


        public string AccountName => Credentials[1].Split(':')[1];
        public Servers Server => (Servers)Enum.Parse(typeof(Servers), Credentials[2].Split(':')[1]);
        public string SoloQ { get { return _soloQ; } set { _soloQ = value; NotifyPropertyChanged("VisibleText"); } }
        public string FlexQ { get { return _flexQ; } set { _flexQ = value; NotifyPropertyChanged("VisibleText"); } }
        public string VisibleText
        {
            get
            {
                return $"{AccountName} S: {SoloQ} F: {FlexQ}";
            }
        }


    }
}
