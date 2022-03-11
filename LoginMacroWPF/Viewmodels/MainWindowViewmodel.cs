using LoginMacroWPF.Components;
using LoginMacroWPF.Models;
using LoginMacroWPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using UWPBindingCollection.ViewModels;
using static LoginMacroWPF.PassManager;

namespace LoginMacroWPF.Viewmodels
{
    internal class MainWindowViewmodel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private readonly string path = $"{Environment.CurrentDirectory}/.pwd";
        private string _username, _accName, _password;
        private List<Servers> _serverStrings = new List<Servers>();
        private int _selectedIndex = 1;
        private Summoner _selectedSummoner;
        private ObservableCollection<Summoner> _selectedServer;
        private ObservableDictionary<string, ObservableCollection<Summoner>> _servers = new ObservableDictionary<string, ObservableCollection<Summoner>>();
        private bool _isError = false;

        private CredentialsReader credentials;
        private MessageSender sender;


        public List<Servers> ServerStrings { get { return _serverStrings; } set { _serverStrings = value; NotifyPropertyChanged(); } }
        public int SelectedIndex { get { return _selectedIndex; } set { _selectedIndex = value; NotifyPropertyChanged(); } }
        public string AccName { get { return _accName; } set { _accName = value; NotifyPropertyChanged(); } }
        public string Username { get { return _username; } set { _username = value; NotifyPropertyChanged(); } }
        public string Password { get { return _password; } set { _password = value; NotifyPropertyChanged(); } }
        public bool IsError { get { return _isError; } set { _isError = value; NotifyPropertyChanged(); } }
        public Summoner SelectedSummoner { get { return _selectedSummoner; } set { _selectedSummoner = value; NotifyPropertyChanged(); } }
        public ObservableCollection<Summoner> SelectedServer { get { return _selectedServer; } set { _selectedServer = value; NotifyPropertyChanged(); } }
        public ObservableDictionary<string, ObservableCollection<Summoner>> Servers { get { return _servers; } set { _servers = value; NotifyPropertyChanged(); } }

        public RelayCommand Login { get; set; }
        public RelayCommand EditLogin { get; set; }
        public RelayCommand AddAccount { get; set; }
        public RelayCommand CopyPassword { get; set; }
        public MainWindowViewmodel()
        {
            ServerStrings = Enum.GetValues(typeof(Servers)).Cast<Servers>().ToList();

            if (credentials == null) credentials = new CredentialsReader(path);
            credentials.CreateServerLogins(Servers);
            sender = new MessageSender();

            Login = new RelayCommand(
                () =>
                {
                    sender.Login(SelectedSummoner, IsError);
                },
                () => true);
            EditLogin = new RelayCommand(
                () =>
                {
                    //reading file to replace it later
                    string wholeFile = File.ReadAllText(path);

                    //getting credentials and replacement for them
                    string replacement = $"\tAccountName:{AccName}\r\n\tServer:{ServerStrings[SelectedIndex]}\r\n\tusername:{Username}\r\n\tpass:{Encrypt(Password)}";
                    string toReplace = string.Join(Environment.NewLine, SelectedSummoner.Credentials);

                    wholeFile = wholeFile.Replace(toReplace, replacement);



                    //writing changes to the file
                    credentials.EditLogin(SelectedSummoner, replacement);
                    File.WriteAllText(path, wholeFile);
                },
                () => true);
            AddAccount = new RelayCommand(
                () =>
                {
                    //create new summoner class and put it into ObservableCollection
                    Summoner sum = new Summoner(new string[] { "\tAccountName:" + AccName, "\tServer:" + ServerStrings[SelectedIndex], "\tusername:" + Username, "\tpass:" + Encrypt(Password) });
                    if (Servers.ContainsKey(ServerStrings[SelectedIndex].ToString().ToUpper()))
                    {
                        Servers.Where(ser => ser.Key == ServerStrings[SelectedIndex].ToString().ToUpper()).First().Value.Add(sum);
                    }
                    else { Servers.Add(ServerStrings[SelectedIndex].ToString().ToUpper(), new ObservableCollection<Summoner>() { sum }); }
                    credentials.CreateLogin(sum);

                    //append to the file
                    File.AppendAllText(path,
                        $"{{{Environment.NewLine}" +
                        $"\tAccountName:{AccName}{Environment.NewLine}" +
                        $"\tServer:{ServerStrings[SelectedIndex]} {Environment.NewLine}" +
                        $"\tusername:{Username}{Environment.NewLine}" +
                        $"\tpass:{Encrypt(Password)}{Environment.NewLine}" +
                        $"}}{Environment.NewLine}");
                },
                () => true);
            CopyPassword = new RelayCommand(
                () =>
                {
                    Clipboard.SetText(Password);
                },
                () => true);
        }


        public ObservableCollection<Summoner> GetSummoners()
        {
            return SelectedServer;
        }

        public void SummonerSelected()
        {
            AccName = SelectedSummoner.AccountName;
            Username = SelectedSummoner.Credentials[2].Split(':')[1];
            Password = Decrypt(SelectedSummoner.Credentials[3].Split(':')[1]);
            SelectedIndex = (int)SelectedSummoner.Server;
        }

    }
}
