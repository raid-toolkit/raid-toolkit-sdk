using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Raid.DataModel;
using SharedModel.Meta.Skills;
using Extractor = RaidExtractor.Core.Extractor;

namespace Raid.Service
{
    internal class AccountApi : ApiHandler, IAccountApi
    {
        private readonly StaticDataCache StaticDataCache;
        private readonly UserData UserData;
        private readonly Extractor Extractor;
        public AccountApi(ILogger<ApiHandler> logger, UserData userData, StaticDataCache staticData, Extractor extractor)
            : base(logger) => (UserData, StaticDataCache, Extractor) = (userData, staticData, extractor);

        [PublicApi("updated")]
#pragma warning disable 0067
        public event EventHandler<SerializableEventArgs> Updated;
#pragma warning restore 0067

        public Task<RaidExtractor.Core.AccountDump> GetAccountDump(string accountId) => Task.FromResult(Extractor.DumpAccount(UserData.GetAccount(accountId)));

        public Task<Resources> GetAllResources(string accountId) => Task.FromResult(ResourcesFacet.ReadValue(UserData.GetAccount(accountId)));

        public Task<Account[]> GetAccounts() => Task.FromResult(UserData.UserAccounts.Select(AccountFacet.ReadValue).ToArray());

        public Task<Account> GetAccount(string accountId) => Task.FromResult(AccountFacet.ReadValue(UserData.GetAccount(accountId)));

        public Task<Artifact[]> GetArtifacts(string accountId) => Task.FromResult(ArtifactsFacet.ReadValue(UserData.GetAccount(accountId)).Values.ToArray());

        public Task<Artifact> GetArtifactById(string accountId, int artifactId) => Task.FromResult(ArtifactsFacet.ReadValue(UserData.GetAccount(accountId))[artifactId]);

        public Task<Hero[]> GetHeroes(string accountId, bool snapshot = false)
        {
            var heroes = HeroesFacet.ReadValue(UserData.GetAccount(accountId)).Values;
            if (!snapshot)
                return Task.FromResult(heroes.ToArray());

            return Task.FromResult<Hero[]>(heroes.Select(hero => GetSnapshot(accountId, hero)).ToArray());
        }

        public Task<Hero> GetHeroById(string accountId, int heroId, bool snapshot = false)
        {
            var hero = HeroesFacet.ReadValue(UserData.GetAccount(accountId))[heroId];
            if (!snapshot)
                return Task.FromResult(hero);

            return Task.FromResult<Hero>(GetSnapshot(accountId, hero));
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
                    if (upgrade.SkillBonusType == SkillBonusType.CooltimeTurn.ToString())
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

            if (staticData.ArenaData.Leagues.TryGetValue(arenaData.LeagueId, out var league))
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
                {
                    Logger.LogWarning(ServiceEvent.MissingSkill.EventId(), $"Skill '{skill.TypeId}' is missing from static data");
                    continue;
                }

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