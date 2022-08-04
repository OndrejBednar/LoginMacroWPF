using LoginMacroWPF.Components;
using LoginMacroWPF.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace LoginMacroWPF.Services
{
    public class CredentialsRW
    {
        private List<IAccount> Accounts = new List<IAccount>();
        private string[] loginCredentials;
        private string checkErrors = "";
        private string path;
        IWebDriver Chrome;
        public CredentialsRW(string path)
        {
            this.path = path;
            //reading all the credentials from our file

            ChromeDriverService chromeDriverService;
            ChromeOptions options = new ChromeOptions();
            options.AddArguments(new List<string>() { $"--headless" });
            try { chromeDriverService = ChromeDriverService.CreateDefaultService(); chromeDriverService.HideCommandPromptWindow = true; Chrome = new ChromeDriver(chromeDriverService, options); }
            catch (Exception)
            {
                var processes = Process.GetProcesses().Where(p => p.ProcessName == "chromedriver");
                foreach (var process in processes)
                {
                    process.Kill();
                }
                ChromeDriverInstaller.Install();
                chromeDriverService = ChromeDriverService.CreateDefaultService();
                chromeDriverService.HideCommandPromptWindow = true;
                Chrome = new ChromeDriver(chromeDriverService, options);
            }

            WebDriverWait Wait = new WebDriverWait(Chrome, TimeSpan.FromSeconds(30))
            {
                PollingInterval = TimeSpan.FromSeconds(5)
            };
            Wait.IgnoreExceptionTypes(typeof(NoSuchElementException));

            try
            {
                loginCredentials = File.ReadAllText(path).Split('{', '}').Where((item, index) => index % 2 != 0).ToArray(); //regex equivalent /\{([^}]*)\}/g
                foreach (var account in loginCredentials)
                {
                    string[] login = account.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    switch ((Platforms)Enum.Parse(typeof(Platforms), login[0]))
                    {
                        case Platforms.Lol:
                            var sum = new Summoner(login);
                            Accounts.Add(sum);
                            break;
                        case Platforms.Steam:
                            var acc = new SteamAcc(login);
                            Accounts.Add(acc);
                            break;
                        case Platforms.BattleNet:
                            break;
                        default:
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                if (Environment.CurrentDirectory != @"C:\Windows\system32")
                {
                    File.AppendAllText(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "/debug.log", ex.ToString());
                    MessageBox.Show($"You dont have any accounts saved yet");
                }
            }
        }

        public void CreateServerLogins(ObservableDictionary<string, ObservableCollection<Summoner>> collection)
        {
            var accs = Accounts.Where(acc => acc.Platform == Platforms.Lol);
            foreach (Summoner sum in accs)
            {
                if (collection.ContainsKey(sum.Server.ToString().ToUpper()))
                {
                    collection.Where(ser => ser.Key == sum.Server.ToString().ToUpper()).First().Value.Add(sum);
                }
                else
                {
                    collection.Add(sum.Server.ToString().ToUpper(), new ObservableCollection<Summoner>() { sum });
                }
            }
        }
        public void CreateServerLogins(ObservableCollection<SteamAcc> collection)
        {
            var accs = Accounts.Where(acc => acc.Platform == Platforms.Steam);
            foreach (SteamAcc acc in accs)
            {
                collection.Add(acc);
            }
        }

        public void EditLogin(IAccount acc, string replacement)
        {
            //reading file to replace it later
            string wholeFile = File.ReadAllText(path);

            //getting credentials and replacing them with new ones
            string toReplace = string.Join(Environment.NewLine, acc.Credentials);

            wholeFile = wholeFile.Replace(toReplace, replacement);

            File.WriteAllText(path, wholeFile);

            Accounts.Find(a => a == acc).Credentials = replacement.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        public IAccount CreateLogin(string[] creds, Platforms platform)
        {
            switch (platform)
            {
                case Platforms.Lol:
                    Summoner sum = new Summoner(creds);
                    Accounts.Add(sum);

                    //append to the file
                    File.AppendAllText(path,
                        $"{{{Environment.NewLine}" +
                        $"\t0{Environment.NewLine}" +
                        $"\tAccountName:{sum.AccountName}{Environment.NewLine}" +
                        $"\tServer:{sum.Server} {Environment.NewLine}" +
                        $"\tusername:{sum.Username}{Environment.NewLine}" +
                        $"\tpass:{sum.Password}{Environment.NewLine}" +
                        $"}}{Environment.NewLine}");
                    return sum;
                case Platforms.Steam:
                    SteamAcc acc = new SteamAcc(creds);
                    Accounts.Add(acc);

                    //append to the file
                    File.AppendAllText(path,
                        $"{{{Environment.NewLine}" +
                        $"\t1{Environment.NewLine}" +
                        $"\tAccountName:{acc.AccountName}{Environment.NewLine}" +
                        $"\tusername:{acc.Username}{Environment.NewLine}" +
                        $"\tpass:{acc.Password}{Environment.NewLine}" +
                        $"}}{Environment.NewLine}");

                    return acc;
                case Platforms.BattleNet:
                    return null;
                default:
                    return null;
            }
        }

        public void ScrapLeagueAcc(Summoner sum)
        {
            checkErrors = "";
            try
            {
                Chrome.Url = $"https://{sum.Server.ToString().ToLower()}.op.gg/summoners/{sum.Server.ToString().ToLower()}/{sum.AccountName}";
                Chrome.Navigate();

                //not needed anymore
                //var unrankedCheck = Chrome.FindElements(By.CssSelector(".wrapper"));
                var divisions = Chrome.FindElements(By.CssSelector(".wrapper .info"));

                sum.SoloQ = divisions[0].FindElements(By.TagName("div"))[1].Text;
                sum.FlexQ = divisions[1].FindElements(By.TagName("div"))[1].Text;
            }
            catch (Exception)
            {
                checkErrors += sum.AccountName + Environment.NewLine;
                sum.SoloQ = "Error";
                sum.FlexQ = "Error";
            }
            if (checkErrors != "")
            {
                MessageBox.Show("There was an error with getting divisions for following accounts: " + Environment.NewLine + checkErrors);
            }

        } //for a specified summoner
        public void ScrapLeagueAcc(List<Summoner> sums)
        {
            checkErrors = "";
            foreach (var sum in sums)
            {
                try
                {
                    Chrome.Url = $"https://{sum.Server.ToString().ToLower()}.op.gg/summoners/{sum.Server.ToString().ToLower()}/{sum.AccountName}";
                    Chrome.Navigate();

                    //not needed anymore
                    //var unrankedCheck = Chrome.FindElements(By.CssSelector(".wrapper"));
                    var divisions = Chrome.FindElements(By.CssSelector(".wrapper .info"));

                    sum.SoloQ = divisions[0].FindElements(By.TagName("div"))[1].Text;
                    sum.FlexQ = divisions[1].FindElements(By.TagName("div"))[1].Text;
                }
                catch (Exception)
                {
                    checkErrors += sum.AccountName + Environment.NewLine;
                    sum.SoloQ = "Error";
                    sum.FlexQ = "Error";
                }
                if (checkErrors != "")
                {
                    MessageBox.Show("There was an error with getting divisions for following accounts: " + Environment.NewLine + checkErrors);
                }
            }

        } //for a specified list of summoners
        public void ScrapLeagueAcc()
        {
            checkErrors = "";
            foreach (Summoner sum in Accounts.Where(a => a.Platform == Platforms.Lol))
            {
                try
                {
                    Chrome.Url = $"https://{sum.Server.ToString().ToLower()}.op.gg/summoners/{sum.Server.ToString().ToLower()}/{sum.AccountName}";
                    Chrome.Navigate();

                    //not needed anymore
                    //var unrankedCheck = Chrome.FindElements(By.CssSelector(".wrapper"));
                    var divisions = Chrome.FindElements(By.CssSelector("#content-container"))[0].FindElements(By.TagName("div"))[0].FindElements(By.XPath("./div"));

                    if (divisions[0].FindElements(By.XPath("./div")).Count == 2)
                    {
                        /*
                        string[] temp = divisions[0].FindElements(By.XPath("./div"))[1].FindElements(By.XPath("./div"))[1].Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        sum.SoloQ = String.Join(" ", temp);
                        */
                        sum.SoloQ = divisions[0].FindElements(By.XPath("./div"))[1].FindElements(By.XPath("./div"))[1].FindElements(By.XPath("./div"))[0].Text.Trim();
                    }
                    else { sum.SoloQ = "Unranked"; }
                    if (divisions[1].FindElements(By.XPath("./div")).Count == 2)
                    {
                        /*
                        string[] temp = divisions[1].FindElements(By.XPath("./div"))[1].FindElements(By.XPath("./div"))[1].Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        sum.FlexQ = String.Join(" ", temp);
                        */
                        sum.FlexQ = divisions[1].FindElements(By.XPath("./div"))[1].FindElements(By.XPath("./div"))[1].FindElements(By.XPath("./div"))[0].Text.Trim();
                    }
                    else { sum.FlexQ = "Unranked"; }
                }
                catch (Exception)
                {
                    checkErrors += sum.AccountName + Environment.NewLine;
                    sum.SoloQ = "Error";
                    sum.FlexQ = "Error";
                }
            }
            if (checkErrors != "")
            {
                MessageBox.Show("There was an error with getting divisions for following accounts: " + Environment.NewLine + checkErrors);
            }

        } //for all summoners at once

        public void DisposeOfChromedriver()
        {
            Chrome.Close();
            Chrome.Dispose();
        } //freeing up memory before closing the program
    }
}
