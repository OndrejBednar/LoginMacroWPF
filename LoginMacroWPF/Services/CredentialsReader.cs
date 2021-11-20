using LoginMacroWPF.Components;
using LoginMacroWPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace LoginMacroWPF.Services
{
    internal class CredentialsReader
    {
        private List<Summoner> Accounts = new List<Summoner>();
        private string[] loginCredentials;

        public CredentialsReader(string path)
        {
            //reading all the credentials from our file
            try
            {
                loginCredentials = File.ReadAllText(path).Split('{', '}').Where((item, index) => index % 2 != 0).ToArray(); //regex equivalent /\{([^}]*)\}/g

                foreach (var login in loginCredentials)
                {
                    Accounts.Add(new Summoner(login.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)));
                }
            }
            catch (Exception ex)
            {
                if (Environment.CurrentDirectory != @"C:\Windows\System32")
                {
                    Debug.WriteLine("exception: " + ex);
                    File.AppendAllText("C:\\Users\\Ondra-PC\\source\\repos\\LoginMacroWPF\\LoginMacroWPF\\bin\\Debug" + "/debug.log", ex.ToString());
                    MessageBox.Show("You dont have any accounts saved yet");
                }
            }

        }

        public void CreateServerLogins(ObservableDictionary<string, ObservableCollection<Summoner>> collection)
        {
            foreach (Summoner acc in Accounts)
            {
                if (collection.ContainsKey(acc.Server.ToString().ToUpper()))
                {
                    collection.Where(ser => ser.Key == acc.Server.ToString().ToUpper()).First().Value.Add(acc);
                }
                else
                {
                    collection.Add(acc.Server.ToString().ToUpper(), new ObservableCollection<Summoner>() { acc });
                }
            }
        }

        public void EditLogin(Summoner sum, string replacement)
        {
            Accounts.Find(s => s == sum).Credentials = replacement.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        public void CreateLogin(Summoner sum)
        {
            Accounts.Add(sum);
        }
    }
}
