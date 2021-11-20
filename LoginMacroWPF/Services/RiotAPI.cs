using RiotSharp;
using RiotSharp.Endpoints.SpectatorEndpoint;
using RiotSharp.Misc;
using System;
using System.Threading.Tasks;

namespace LoginMacroWPF.Services
{
    class RiotAPI
    {
        public static RiotApi Api { get; private set; }
        public static RiotSharp.Endpoints.SummonerEndpoint.Summoner Sum { get; private set; }


        public static bool SetApiKey(string key)
        {
            try
            {
                Api = RiotApi.GetDevelopmentInstance(key);

            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public async void GetSummoner(string name, Region region)
        {
            Sum = await Api.Summoner.GetSummonerByNameAsync(region, name);
        }
        public static int GetPlayerLvl(string name, Region region)
        {
            return (int)Api.Summoner.GetSummonerByNameAsync(region, name).Result.Level;
        }
        public static string GetPlayerId(string name, Region region)
        {
            return Api.Summoner.GetSummonerByNameAsync(region, name).Result.Id;
        }

        public async static Task<bool?> GetPlayerStatus(string name, Region region)
        {
            if (Api == null) return null;
            CurrentGame game;
            string id;
            try
            {
                id = (await Api.Summoner.GetSummonerByNameAsync(region, name)).Id;
            }
            catch (Exception)
            {
                return null;
            }
            try
            {
                game = await Api.Spectator.GetCurrentGameAsync(region, id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //public void GetChampionMasteries()
        //{
        //    var championMasteries = Api.ChampionMastery.GetChampionMasteriesAsync(RiotSharp.Misc.Region.Eune, Summoner.Id).Result;
        //    int i = 1;
        //    foreach (var championMastery in championMasteries)
        //    {
        //        var id = championMastery.ChampionId;
        //        var name = Api.StaticData.Champions.GetAllAsync("10.1.1").Result.Champions.Values.Single(x => x.Id == id).Name;
        //        var level = championMastery.ChampionLevel;
        //        var points = championMastery.ChampionPoints;

        //        Console.WriteLine($"{i}  **Level {level} {name}**  {points} Points");
        //        i++;
        //    }
        //}
    }
}