using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Raid.Service.DataModel;
using SharedModel.Meta.Skills;
using Extractor = RaidExtractor.Core.Extractor;

namespace Raid.Service
{
    [PublicApi("account-api")]
    internal class AccountApi : ApiHandler
    {
        private readonly StaticDataCache StaticDataCache;
        private readonly UserData UserData;
        private readonly Extractor Extractor;
        public AccountApi(ILogger<ApiHandler> logger, UserData userData, StaticDataCache staticData, Extractor extractor)
            : base(logger) => (UserData, StaticDataCache, Extractor) = (userData, staticData, extractor);

        [PublicApi("updated")]
        public event EventHandler<SerializableEventArgs> Updated;

        [PublicApi("dump")]
        public RaidExtractor.Core.AccountDump GetAccountDump(string accountId) => Extractor.DumpAccount(UserData.GetAccount(accountId));

        [PublicApi("getAllResources")]
        public Resources GetAllResources(string accountId) => ResourcesFacet.ReadValue(UserData.GetAccount(accountId));

        [PublicApi("getAccounts")]
        public Account[] GetAccounts() => UserData.UserAccounts.Select(AccountFacet.ReadValue).ToArray();

        [PublicApi("accountInfo")]
        public Account GetAccount(string accountId) => AccountFacet.ReadValue(UserData.GetAccount(accountId));

        [PublicApi("getArtifacts")]
        public Artifact[] GetArtifacts(string accountId) => ArtifactsFacet.ReadValue(UserData.GetAccount(accountId)).Values.ToArray();

        [PublicApi("getArtifactById")]
        public Artifact GetArtifactById(string accountId, int artifactId) => ArtifactsFacet.ReadValue(UserData.GetAccount(accountId))[artifactId];

        [PublicApi("getHeroes")]
        public Hero[] GetHeroes(string accountId, bool snapshot = false)
        {
            var heroes = HeroesFacet.ReadValue(UserData.GetAccount(accountId)).Values;
            if (!snapshot)
                return heroes.ToArray();

            return heroes.Select(hero => GetSnapshot(accountId, hero)).ToArray();
        }

        [PublicApi("getHeroById")]
        public Hero GetHeroById(string accountId, int heroId, bool snapshot = false)
        {
            var hero = HeroesFacet.ReadValue(UserData.GetAccount(accountId))[heroId];
            if (!snapshot)
                return hero;

            return GetSnapshot(accountId, hero);
        }

        private SkillSnapshot GetSkillSnapshot(DataModel.SkillType skill, int level)
        {
            SkillSnapshot snapshot = new(skill)
            {
                Level = level
            };
            if (skill.Upgrades != null)
            {
                foreach (var upgrade in skill.Upgrades)
                {
                    if (upgrade.SkillBonusType == SkillBonusType.CooltimeTurn)
                        snapshot.Cooldown -= (int)Math.Round(upgrade.Value);
                }
            }

            return snapshot;
        }

        private HeroSnapshot GetSnapshot(string accountId, Hero hero)
        {
            var staticData = StaticDataFacet.ReadValue(StaticDataCache);
            var arenaData = ArenaFacet.ReadValue(UserData.GetAccount(accountId));
            var artifactData = ArtifactsFacet.ReadValue(UserData.GetAccount(accountId));
            HeroType type = hero.Type;
            HeroStatsCalculator stats = new(type, hero.Rank, hero.Level);

            // arena
            var greatHallBonus = arenaData.GreatHallBonuses?.FirstOrDefault(ghb => ghb.Affinity == type.Affinity);
            if (greatHallBonus != null)
                stats.ApplyBonuses(StatSource.GreatHall, greatHallBonus.Bonus.ToArray());

            if (staticData.ArenaData.Leagues.TryGetValue((int)arenaData.LeagueId, out var league))
                stats.applyArenaStats(league.StatBonus);

            // masteries
            if (hero.Masteries != null)
                stats.ApplyMasteries(hero.Masteries);

            // artifacts
            var equippedArtifacts = hero.EquippedArtifactIds?.Values.Select(artifactId => artifactData.TryGetValue(artifactId, out var value) ? value : null).Where(artifact => artifact != null);
            if (equippedArtifacts != null)
            {
                stats.ApplyArtifacts(equippedArtifacts);

                // sets
                var setCounts = equippedArtifacts.Select(artifact => artifact.SetKindId).GroupBy(setKindId => setKindId).ToDictionary(group => group.Key, group => group.Count());
                foreach ((var setKindId, var count) in setCounts)
                {
                    if (!staticData.ArtifactData.ArtifactSetKinds.TryGetValue(setKindId, out ArtifactSetKind setKind))
                        continue;
                    int numSets = count / setKind.ArtifactCount;

                    if (numSets > 0)
                        stats.ApplyArtifactSetBonuses(numSets, setKind.StatBonuses);
                }
            }

            List<SkillSnapshot> skillSnapshots = new();
            foreach (var skill in hero.SkillsById.Values)
            {
                if (!staticData.SkillData.SkillTypes.TryGetValue(skill.TypeId, out var skillType))
                    // TODO: Logging
                    continue;

                skillSnapshots.Add(GetSkillSnapshot(skillType, skill.Level));
            }

            return new(hero)
            {
                Skills = skillSnapshots.ToArray(),
                Stats = stats.Snapshot,
                // TODO
                Teams = Array.Empty<string>()
            };
        }
    }
}