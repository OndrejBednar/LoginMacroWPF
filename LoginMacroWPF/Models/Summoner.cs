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
        string[] _credentials = new string[] { }; 

        public string Username { get => Credentials[3].Split(':')[1]; }
        public string Password { get => Credentials[4].Split(':')[1]; }
        public Platforms Platform { get => (Platforms)Enum.Parse(typeof(Platforms), Credentials[0]); }
        public string[] Credentials { get { return _credentials; } set { _credentials = value; NotifyPropertyChanged("AccountName"); NotifyPropertyChanged("AccountTag"); NotifyPropertyChanged("Server"); NotifyPropertyChanged("VisibleText"); } }


        public string AccountName => Credentials[1].Split(':')[1].Split('#')[0];
        public string AccountTag => Credentials[1].Split(':')[1].Split("#").Length == 2 ? Credentials[1].Split(':')[1].Split("#")[1] : Server.ToString().ToUpper();
        public Servers Server => (Servers)Enum.Parse(typeof(Servers), Credentials[2].Split(':')[1]);
        public string SoloQ { get { return _soloQ; } set { _soloQ = value; NotifyPropertyChanged("VisibleText"); } }
        public string FlexQ { get { return _flexQ; } set { _flexQ = value; NotifyPropertyChanged("VisibleText"); } }
        public string VisibleText
        {
            get
            {
                return $"{AccountName}#{AccountTag} S: {SoloQ} F: {FlexQ}";
            }
        }


    }
}
