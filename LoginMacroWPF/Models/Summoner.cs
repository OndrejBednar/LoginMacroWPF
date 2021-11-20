using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginMacroWPF.Models
{
    public class Summoner
    {
        public string AccountName
        {
            get
            {
                return Credentials[0].Split(':')[1];
            }
        }
        public Servers Server 
        {
            get
            {
                return (Servers)Enum.Parse(typeof(Servers),Credentials[1].Split(':')[1]);
            }
        }
        public string[] Credentials { get; set; }

        public Summoner(string[] credentials)
        {
            Credentials = credentials;
        }
    }
}
