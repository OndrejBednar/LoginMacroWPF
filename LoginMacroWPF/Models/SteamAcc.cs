using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginMacroWPF.Models
{
    public class SteamAcc : IAccount
    {
        public SteamAcc(string[] credentials)
        {
            Credentials = credentials;
        }

        public string Username => Credentials[2].Split(':')[1];
        public string Password => Credentials[3].Split(':')[1];
        public Platforms Platform => (Platforms)Enum.Parse(typeof(Platforms), Credentials[0]);
        public string[] Credentials { get; set; }

        public string AccountName => Credentials[1].Split(':')[1];
        public string VisibleText => AccountName;

    }
}
