using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Services;
using SharedModel.Meta.Skills;
using Extractor = RaidExtractor.Core.Extractor;

namespace Raid.Toolkit.Extension.Account
{
    internal class AccountApi : ApiHandler<IAccountApi>, IAccountApi
    {
        private readonly Extractor Extractor;
        private readonly CachedDataStorage<PersistedDataStorage> Storage;

        public AccountApi(
            ILogger<AccountApi> logger,
            CachedDataStorage<PersistedDataStorage> storage
            )
            : base(logger)
        {
            Storage = storage;
            Extractor = new Extractor();
            Storage.Updated += OnStorageUpdated;
        }

        private void OnStorageUpdated(object sender, DataStorageUpdatedEventArgs e)
        {
            if (e.Key == "_index")
                return;

            if (e.Context is AccountDataContext context)
            {
                Updated?.Invoke(this, new AccountUpdatedEventArgs(context.AccountId));
            }
        }

        [PublicApi("updated")]
        public event EventHandler<SerializableEventArgs> Updated;

        public Task<RaidExtractor.Core.AccountDump> GetAccountDump(string accountId)
        {
            // TODO: Get LastUpdated
            return Task.FromResult(Extractor.DumpAccount(new AccountData(Storage, accountId), new StaticDataWrapper(Storage), accountId, DateTime.UtcNow));
        }

        public Task<Resources> GetAllResources(string accountId)
        {
            return Task.FromResult(new AccountData(Storage, accountId).Resources);
        }

        public Task<DataModel.Account[]> GetAccounts()
        {
            return Task.FromResult(Storage.GetKeys(new AccountDirectoryContext()).Select(AccountFromUserAccount).ToArray());
        }

        private DataModel.Account AccountFromUserAccount(string accountId)
        {
            // TODO: Get LastUpdated
            return DataModel.Account.FromBase(new AccountData(Storage, accountId).Account, DateTime.UtcNow);
        }

        public Task<DataModel.Account> GetAccount(string accountId)
        {
            return Task.FromResult(AccountFromUserAccount(accountId));
        }

        public Task<ArenaData> GetArena(string accountId)
        {
            return Task.FromResult(new AccountData(Storage, accountId).Arena);
        }

        public Task<AcademyData> GetAcademy(string accountId)
        {
            return Task.FromResult(new AccountData(Storage, accountId).Academy);
        }

        public Task<Artifact[]> GetArtifacts(string accountId)
        {
            return Task.FromResult(new AccountData(Storage, accountId).Artifacts.Values.ToArray());
        }

        public Task<Artifact> GetArtifactById(string accountId, int artifactId)
        {
            return Task.FromResult(new AccountData(Storage, accountId).Artifacts[artifactId]);
        }

        public Task<Hero[]> GetHeroes(string accountId, bool snapshot = false)
        {
            var heroes = new AccountData(Storage, accountId).Heroes.Heroes.Values;
            return !snapshot
                ? Task.FromResult(heroes.ToArray())
                : Task.FromResult<Hero[]>(heroes.Select(hero => GetSnapshot(accountId, hero)).ToArray());
        }

        public Task<Hero> GetHeroById(string accountId, int heroId, bool snapshot = false)
        {
            var hero = new AccountData(Storage, accountId).Heroes.Heroes[heroId];
            return !snapshot ? Task.FromResult(hero) : Task.FromResult<Hero>(GetSnapshot(accountId, hero));
        }

        private static SkillSnapshot GetSkillSnapshot(DataModel.SkillType skill, int level)
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
            StaticDataWrapper staticData = new(Storage);
            AccountData accountData = new(Storage, accountId);
            HeroType type = hero.Type;
            HeroStatsCalculator stats = new(type, (int)Enum.Parse(typeof(SharedModel.Meta.Heroes.HeroGrade), hero.Rank), hero.Level);

            // arena
            var greatHallBonus = accountData.Arena.GreatHallBonuses?.FirstOrDefault(ghb => ghb.Affinity == type.Affinity);
            if (greatHallBonus != null)
                stats.ApplyBonuses(StatSource.GreatHall, greatHallBonus.Bonus.ToArray());

            if (staticData.Arena.Leagues.TryGetValue(accountData.Arena.ClassicArena.LeagueId.ToString(), out var league))
                stats.applyArenaStats(league.StatBonus);

            // masteries
            if (hero.Masteries != null)
                stats.ApplyMasteries(hero.Masteries);

            // artifacts
            var equippedArtifacts = hero.EquippedArtifactIds?.Values.Select(artifactId => accountData.Artifacts.TryGetValue(artifactId, out var value) ? value : null).Where(artifact => artifact != null);
            if (equippedArtifacts != null)
            {
                stats.ApplyArtifacts(equippedArtifacts);

                // sets
                var setCounts = equippedArtifacts.Select(artifact => artifact.SetKindId).GroupBy(setKindId => setKindId).ToDictionary(group => group.Key, group => group.Count());
                foreach (var kvp in setCounts)
                {
                    string setKindId = kvp.Key;
                    int count = kvp.Value;

                    if (!staticData.Artifacts.ArtifactSetKinds.TryGetValue(setKindId, out ArtifactSetKind setKind))
                        continue;
                    int numSets = count / setKind.ArtifactCount;

                    if (numSets > 0)
                        stats.ApplyArtifactSetBonuses(numSets, setKind.StatBonuses);
                }
            }

            List<SkillSnapshot> skillSnapshots = new();
            foreach (var skill in hero.SkillsById.Values)
            {
                if (!staticData.Skills.SkillTypes.TryGetValue(skill.TypeId, out var skillType))
                {
                    Logger.LogWarning($"Skill '{skill.TypeId}' is missing from static data");
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
