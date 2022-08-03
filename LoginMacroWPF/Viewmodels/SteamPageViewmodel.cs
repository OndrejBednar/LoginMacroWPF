using LoginMacroWPF.Models;
using LoginMacroWPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using UWPBindingCollection.ViewModels;

namespace LoginMacroWPF.Viewmodels
{
    public class SteamPageViewmodel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<SteamAcc> _accounts = new ObservableCollection<SteamAcc>();
        private SteamAcc acc;
        private string _username, _accName, _password;

        public SteamAcc SelectedAccount { get { return acc; } set { acc = value; NotifyPropertyChanged(); } }
        public string AccName { get { return _accName; } set { _accName = value; NotifyPropertyChanged(); } }
        public string Username { get { return _username; } set { _username = value; NotifyPropertyChanged(); } }
        public string Password { get { return _password; } set { _password = value; NotifyPropertyChanged(); } }
        public ObservableCollection<SteamAcc> Accounts { get { return _accounts; } set { _accounts = value; NotifyPropertyChanged(); } }


        public RelayCommand Login { get; set; }
        public RelayCommand EditLogin { get; set; }
        public RelayCommand AddAccount { get; set; }
        public RelayCommand CopyPassword { get; set; }
        public SteamPageViewmodel()
        {

            App.CredentialRW.CreateServerLogins(Accounts);

            Login = new RelayCommand(
                () =>
                {
                    MessageSender.Login(SelectedAccount);
                },
                () => true);
            EditLogin = new RelayCommand(
                () =>
                {
                    //getting replacement for credentials
                    string replacement = $"\t1\r\n\tAccountName:{AccName}\r\n\tusername:{Username}\r\n\tpass:{PassManager.Encrypt(Password)}";


                    //writing changes to the file
                    App.CredentialRW.EditLogin(SelectedAccount, replacement);

                },
                () => true);
            AddAccount = new RelayCommand(
                () =>
                {
                    //create new summoner class and put it into ObservableCollection
                    SteamAcc acc = (SteamAcc)App.CredentialRW.CreateLogin(new string[] { "\t1", "\tAccountName:" + AccName, "\tusername:" + Username, "\tpass:" + PassManager.Encrypt(Password) }, Platforms.Steam);
                    Accounts.Add(acc);

                },
                () => true);
            CopyPassword = new RelayCommand(
                () =>
                {
                    Clipboard.SetText(Password);
                },
                () => true);
        }

        public void AccountSelected()
        {
            AccName = SelectedAccount.AccountName;
            Username = SelectedAccount.Username;
            Password = PassManager.Decrypt(SelectedAccount.Password);
        }
    }
}