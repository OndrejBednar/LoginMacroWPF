using AngleSharp.Dom;
using AngleSharp.Html.Parser;
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
using System.Net.Http;
using System.Windows;

namespace LoginMacroWPF.Services
{
    public class CredentialsRW
    {
        private List<IAccount> Accounts = new List<IAccount>();
        private string[] loginCredentials;
        private string checkErrors = "";
        private string path;
        public CredentialsRW(string path)
        {
            this.path = path;

            try
            {
                //reading all the credentials from our file
                loginCredentials = File.ReadAllText(path).Split('{', '}').Where((item, index) => index % 2 != 0).ToArray(); //regex equivalent /\{([^}]*)\}/g
                foreach (var account in loginCredentials)
                {
                    string[] login = account.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    IAccount acc = null;
                    switch ((Platforms)Enum.Parse(typeof(Platforms), login[0]))
                    {
                        case Platforms.Lol:
                            acc = new Summoner(login);
                            break;
                        case Platforms.Steam:
                            acc = new SteamAcc(login);
                            break;
                        case Platforms.BattleNet:
                            break;
                        default:
                            break;
                    }
                    Accounts.Add(acc);
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
                var httpClient = new HttpClient();
                var request = httpClient.GetAsync($"https://{sum.Server.ToString().ToLower()}.op.gg/summoners/{sum.Server.ToString().ToLower()}/{sum.AccountName}").Result;
                using (var response = request.Content.ReadAsStreamAsync().Result)
                {
                    var parser = new HtmlParser();
                    var document = parser.ParseDocument(response);
                    var divisions = document.QuerySelector("#content-container").FirstChild.ChildNodes;

                    if (divisions[1].ChildNodes.Count() == 2)
                        sum.SoloQ = divisions[1].LastChild.ChildNodes[1].FirstChild.TextContent.Trim();
                    else { sum.SoloQ = "Unranked"; }
                    sum.SoloQ = char.ToUpper(sum.SoloQ[0]) + sum.SoloQ.Substring(1);
                    if (divisions[3].ChildNodes.Count() == 2)
                        sum.FlexQ = divisions[3].LastChild.ChildNodes[1].FirstChild.TextContent.Trim();
                    else { sum.FlexQ = "Unranked"; }
                    sum.FlexQ = char.ToUpper(sum.FlexQ[0]) + sum.FlexQ.Substring(1);
                }
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
                    var httpClient = new HttpClient();
                    var request = httpClient.GetAsync($"https://{sum.Server.ToString().ToLower()}.op.gg/summoners/{sum.Server.ToString().ToLower()}/{sum.AccountName}").Result;
                    using (var response = request.Content.ReadAsStreamAsync().Result)
                    {
                        var parser = new HtmlParser();
                        var document = parser.ParseDocument(response);
                        var divisions = document.QuerySelector("#content-container").FirstChild.ChildNodes;

                        if (divisions[1].ChildNodes.Count() == 2)
                            sum.SoloQ = divisions[1].LastChild.ChildNodes[1].FirstChild.TextContent.Trim();
                        else { sum.SoloQ = "Unranked"; }
                        sum.SoloQ = char.ToUpper(sum.SoloQ[0]) + sum.SoloQ.Substring(1);
                        if (divisions[3].ChildNodes.Count() == 2)
                            sum.FlexQ = divisions[3].LastChild.ChildNodes[1].FirstChild.TextContent.Trim();
                        else { sum.FlexQ = "Unranked"; }
                        sum.FlexQ = char.ToUpper(sum.FlexQ[0]) + sum.FlexQ.Substring(1);
                    }
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
                    var httpClient = new HttpClient();
                    var httpRequest = new HttpRequestMessage();
                    httpRequest.Method = HttpMethod.Get;
                    httpRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:55.0) Gecko/20100101 Firefox/55.0");
                    httpRequest.RequestUri = new Uri($"https://{sum.Server.ToString().ToLower()}.op.gg/summoners/{sum.Server.ToString().ToLower()}/{sum.AccountName.Split('#')[0]}-{sum.AccountTag}");
                    var request = httpClient.Send(httpRequest);
                    //var request = httpClient.GetAsync($"https://{sum.Server.ToString().ToLower()}.op.gg/summoners/{sum.Server.ToString().ToLower()}/{sum.AccountName.Split('#')[0]}-{sum.AccountTag}").Result;
                    using (var response = request.Content.ReadAsStreamAsync().Result)
                    {
                        var parser = new HtmlParser();
                        var document = parser.ParseDocument(response);

                        if (document.GetElementsByTagName("h1")[0].TextContent.Contains("No search results"))
                        {
                            checkErrors += sum.AccountName + Environment.NewLine;
                            sum.SoloQ = "Error";
                            sum.FlexQ = "Error";
                            continue;
                        }

                        var headers = document.QuerySelectorAll(".header");
                        foreach (var division in headers)
                        {
                            if (division.TextContent == "Ranked Solo/Duo")
                                sum.SoloQ = char.ToUpper(division.ParentElement.GetElementsByClassName("content").FirstOrDefault().GetElementsByClassName("info").FirstOrDefault().FirstChild.TextContent.Trim()[0]) + division.ParentElement.GetElementsByClassName("content").FirstOrDefault().GetElementsByClassName("info").FirstOrDefault().FirstChild.TextContent.Trim().Split(' ')[1];
                            else if (division.TextContent == "Ranked Flex")
                                sum.FlexQ = char.ToUpper(division.ParentElement.GetElementsByClassName("content").FirstOrDefault().GetElementsByClassName("info").FirstOrDefault().FirstChild.TextContent.Trim()[0]) + division.ParentElement.GetElementsByClassName("content").FirstOrDefault().GetElementsByClassName("info").FirstOrDefault().FirstChild.TextContent.Trim().Split(' ')[1];
                        }

                        if(sum.SoloQ == "")
                            sum.SoloQ = "Unranked";
                        if(sum.FlexQ == "")
                            sum.FlexQ = "Unranked";

                    }
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

        public Platforms GetNotEmptyPlatform()
        {
            foreach (Platforms pf in Enum.GetValues(typeof(Platforms)))
            {
                if (Accounts.Select(acc => acc.Platform == pf).Count() != 0)
                {
                    return pf;
                }
            }
            return default;
        }
    }
}
