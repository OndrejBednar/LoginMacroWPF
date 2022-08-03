using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginMacroWPF.Models
{
    public interface IAccount
    {
        string Username { get; }
        string Password { get; }
        Platforms Platform { get; }
        string[] Credentials { get; set; }
        string VisibleText { get; }
    }
}
