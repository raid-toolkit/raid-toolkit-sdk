using System;
using System.Threading.Tasks;
using Raid.Client;
using Raid.DataModel;

namespace Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            RaidToolkitClient client = new();
            client.Connect();
            await client.EnsureInstalled();
            var staticData = new StaticData
            {
                ArenaData = await client.StaticDataApi.GetArenaData(),
                ArtifactData = await client.StaticDataApi.GetArtifactData(),
                HeroData = await client.StaticDataApi.GetHeroData(),
                LocalizedStrings = await client.StaticDataApi.GetLocalizedStrings(),
                SkillData = await client.StaticDataApi.GetSkillData(),
                StageData = await client.StaticDataApi.GetStageData()
            };
            var data = await client.StaticDataApi.GetAllData();

        }
    }
}
