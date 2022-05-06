using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WebDriverManager;

namespace LoginMacroWPF.Services
{
    public class ChromeDriverInstaller
    {
        public static string GetChromeVersion()
        {
            string chromePath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\chrome.exe", null, null);
            if (chromePath == null)
            {
                throw new Exception("Google Chrome not found in registry");
            }

            var fileVersionInfo = FileVersionInfo.GetVersionInfo(chromePath);
            return fileVersionInfo.FileVersion;
        }

        public static void Install()
        {
            new DriverManager().SetUpDriver(
                $"https://chromedriver.storage.googleapis.com/{GetChromeVersion()}/chromedriver_win32.zip",
                Path.Combine(Directory.GetCurrentDirectory(), "chromedriver.exe")
            );
        }
    }
}
