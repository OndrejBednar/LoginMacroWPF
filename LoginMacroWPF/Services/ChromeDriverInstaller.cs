using System.IO;
using System.Net;
using WebDriverManager;

namespace LoginMacroWPF.Services
{
    public class ChromeDriverInstaller
    {
        public static string GetChromeVersion()
        {
            WebClient client = new WebClient();
            string s = client.DownloadString("https://chromedriver.storage.googleapis.com/LATEST_RELEASE");
            /*
            string chromePath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\chrome.exe", null, null);
            if (chromePath == null)
            {
                throw new Exception("Google Chrome not found in registry");
            }

            var fileVersionInfo = FileVersionInfo.GetVersionInfo(chromePath);
            return fileVersionInfo.FileVersion;
        */
            return s;
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
