using LoginMacroWPF.Components;
using LoginMacroWPF.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace LoginMacroWPF.Services
{
    internal class CredentialsReader
    {
        private List<Summoner> Accounts = new List<Summoner>();
        private string[] loginCredentials;
        private string checkErrors = "";
        public CredentialsReader(string path)
        {
            //reading all the credentials from our file
            loginCredentials = File.ReadAllText(path).Split('{', '}').Where((item, index) => index % 2 != 0).ToArray(); //regex equivalent /\{([^}]*)\}/g


            ChromeDriverService chromeDriverService;
            ChromeOptions options = new ChromeOptions();
            options.AddArguments(new List<string>() { $"--headless" });
            IWebDriver Chrome;
            try { chromeDriverService = ChromeDriverService.CreateDefaultService(); chromeDriverService.HideCommandPromptWindow = true; Chrome = new ChromeDriver(chromeDriverService, options); }
            catch (Exception)
            {
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
                foreach (var login in loginCredentials)
                {
                    var sum = new Summoner(login.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                    try
                    {
                        Chrome.Url = $"https://{sum.Server.ToString().ToLower()}.op.gg/summoners/{sum.Server.ToString().ToLower()}/{sum.AccountName}";
                        Chrome.Navigate();

                        var unrankedCheck = Chrome.FindElements(By.CssSelector("#content-container .header"));
                        var divisions = Chrome.FindElements(By.CssSelector(".content .info"));
                        var s = unrankedCheck[0].FindElements(By.ClassName("unranked"));
                        var b = unrankedCheck[0].FindElements(By.TagName("span"));
                        if (unrankedCheck[0].FindElements(By.TagName("span")).Count == 0)
                        {
                            sum.SoloQ = divisions[0].FindElements(By.TagName("div"))[0].Text;
                        }
                        if (unrankedCheck[1].FindElements(By.TagName("span")).Count == 0)
                        {
                            if (sum.SoloQ == "Unranked") sum.FlexQ = divisions[0].FindElements(By.TagName("div"))[0].Text;
                            else { sum.FlexQ = divisions[1].FindElements(By.TagName("div"))[0].Text; }
                        }
                    }
                    catch (Exception ex)
                    {
                        checkErrors += sum.AccountName + Environment.NewLine;
                        sum.SoloQ = "Error";
                        sum.FlexQ = "Error";
                    }
                    Accounts.Add(sum);
                }
            }
            catch (Exception ex)
            {
                if (Environment.CurrentDirectory != @"C:\Windows\system32")
                {
                    File.AppendAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source", "repos", "_Github_PrivateProjects", "LoginMacroWPF") + "/debug.log", ex.ToString());
                    MessageBox.Show($"You dont have any accounts saved yet {Environment.NewLine} {Environment.CurrentDirectory}");
                }
            }
            if (checkErrors != "")
            {
                MessageBox.Show("There was an error with getting divisions for following accounts: " + Environment.NewLine + checkErrors);
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
