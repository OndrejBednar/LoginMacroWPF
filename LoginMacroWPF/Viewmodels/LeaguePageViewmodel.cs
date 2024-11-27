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
using System.Windows.Controls;
using UWPBindingCollection.ViewModels;
using static LoginMacroWPF.PassManager;

namespace LoginMacroWPF.Viewmodels
{
    internal class LeaguePageViewmodel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string _username, _accName, _password;
        private List<Servers> _serverStrings = new List<Servers>();
        private int _selectedIndex = 1;
        private Summoner _selectedSummoner;
        private ObservableCollection<Summoner> _selectedServer;
        private ObservableDictionary<string, ObservableCollection<Summoner>> _servers = new ObservableDictionary<string, ObservableCollection<Summoner>>();
        private bool _isError = false;


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
        public RelayCommand ScrapLeagueAccs { get; set; }
        public LeaguePageViewmodel()
        {
            ServerStrings = Enum.GetValues(typeof(Servers)).Cast<Servers>().ToList();

            App.CredentialRW.CreateServerLogins(Servers);

            Login = new RelayCommand(
                () =>
                {
                    MessageSender.Login(SelectedSummoner, IsError);
                },
                () => true);
            EditLogin = new RelayCommand(
                () =>
                {
                    //getting replacement for credentials
                    string replacement = $"\t0\r\n\tAccountName:{AccName}\r\n\tServer:{ServerStrings[SelectedIndex]}\r\n\tusername:{Username}\r\n\tpass:{Encrypt(Password)}";


                    //writing changes to the file
                    App.CredentialRW.EditLogin(SelectedSummoner, replacement);
                    
                },
                () => true);
            AddAccount = new RelayCommand(
                () =>
                {
                    //create new summoner class and put it into ObservableCollection
                    Summoner sum = (Summoner)App.CredentialRW.CreateLogin(new string[] { "\t0", "\tAccountName:" + AccName, "\tServer:" + ServerStrings[SelectedIndex], "\tusername:" + Username, "\tpass:" + Encrypt(Password) }, Platforms.Lol);
                    if (Servers.ContainsKey(ServerStrings[SelectedIndex].ToString().ToUpper()))
                    {
                        Servers.Where(ser => ser.Key == ServerStrings[SelectedIndex].ToString().ToUpper()).First().Value.Add(sum);
                    }
                    else { Servers.Add(ServerStrings[SelectedIndex].ToString().ToUpper(), new ObservableCollection<Summoner>() { sum }); }
                    
                },
                () => true);
            CopyPassword = new RelayCommand(
                () =>
                {
                    Clipboard.SetText(Password);
                },
                () => true);
            ScrapLeagueAccs = new RelayCommand(
                () =>
                {
                    App.CredentialRW.ScrapLeagueAcc();
                },
                () => true);
        }


        public ObservableCollection<Summoner> GetSummoners()
        {
            return SelectedServer;
        }

        public void SummonerSelected()
        {
            AccName = SelectedSummoner.AccountName + "#" + SelectedSummoner.AccountTag;
            Username = SelectedSummoner.Username;
            Password = Decrypt(SelectedSummoner.Password);
            SelectedIndex = (int)SelectedSummoner.Server;
        }

    }
}
