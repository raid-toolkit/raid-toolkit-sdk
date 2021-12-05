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
    internal class AccountApi : ApiHandler<IAccountApi>, IAccountApi
    {
        private readonly StaticDataCache StaticDataCache;
        private readonly UserData UserData;
        private readonly Extractor Extractor;
        public AccountApi(
            ILogger<AccountApi> logger,
            UserData userData,
            StaticDataCache staticData,
            Extractor extractor,
            EventService eventService)
            : base(logger)
        {
            UserData = userData;
            StaticDataCache = staticData;
            Extractor = extractor;
            eventService.OnAccountUpdated += OnAccountUpdated;
        }

        private void OnAccountUpdated(object sender, AccountUpdatedEventArgs e)
        {
            Updated?.Invoke(this, e);
        }

        [PublicApi("updated")]
#pragma warning disable 0067
        public event EventHandler<SerializableEventArgs> Updated;
#pragma warning restore 0067

        public Task<RaidExtractor.Core.AccountDump> GetAccountDump(string accountId)
        {
            return Task.FromResult(Extractor.DumpAccount(UserData.GetAccount(accountId)));
        }

        public Task<Resources> GetAllResources(string accountId)
        {
            return Task.FromResult(ResourcesFacet.ReadValue(UserData.GetAccount(accountId)));
        }

        public Task<Account[]> GetAccounts()
        {
            return Task.FromResult(UserData.UserAccounts.Select(account =>
            {
                AccountBase result = AccountFacet.ReadValue(account);
                return Account.FromBase(result, account.LastUpdated);
            }).ToArray());
        }

        public Task<Account> GetAccount(string accountId)
        {
            UserAccount userAccount = UserData.GetAccount(accountId);
            AccountBase account = AccountFacet.ReadValue(userAccount);
            return Task.FromResult(Account.FromBase(account, userAccount.LastUpdated));
        }

        public Task<ArenaData> GetArena(string accountId)
        {
            return Task.FromResult(ArenaFacet.ReadValue(UserData.GetAccount(accountId)));
        }

        public Task<AcademyData> GetAcademy(string accountId)
        {
            return Task.FromResult(AcademyFacet.ReadValue(UserData.GetAccount(accountId)));
        }

        public Task<Artifact[]> GetArtifacts(string accountId)
        {
            return Task.FromResult(ArtifactsFacet.ReadValue(UserData.GetAccount(accountId)).Values.ToArray());
        }

        public Task<Artifact> GetArtifactById(string accountId, int artifactId)
        {
            return Task.FromResult(ArtifactsFacet.ReadValue(UserData.GetAccount(accountId))[artifactId]);
        }

        public Task<Hero[]> GetHeroes(string accountId, bool snapshot = false)
        {
            var heroes = HeroesFacet.ReadValue(UserData.GetAccount(accountId)).Heroes.Values;
            return !snapshot
                ? Task.FromResult(heroes.ToArray())
                : Task.FromResult<Hero[]>(heroes.Select(hero => GetSnapshot(accountId, hero)).ToArray());
        }

        public Task<Hero> GetHeroById(string accountId, int heroId, bool snapshot = false)
        {
            var hero = HeroesFacet.ReadValue(UserData.GetAccount(accountId)).Heroes[heroId];
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
            var staticData = StaticDataFacet.ReadValue(StaticDataCache);
            var arenaData = ArenaFacet.ReadValue(UserData.GetAccount(accountId));
            var artifactData = ArtifactsFacet.ReadValue(UserData.GetAccount(accountId));
            HeroType type = hero.Type;
            HeroStatsCalculator stats = new(type, (int)Enum.Parse<SharedModel.Meta.Heroes.HeroGrade>(hero.Rank), hero.Level);

            // arena
            var greatHallBonus = arenaData.GreatHallBonuses?.FirstOrDefault(ghb => ghb.Affinity == type.Affinity);
            if (greatHallBonus != null)
                stats.ApplyBonuses(StatSource.GreatHall, greatHallBonus.Bonus.ToArray());

            if (staticData.ArenaData.Leagues.TryGetValue(arenaData.ClassicArena.LeagueId.ToString(), out var league))
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
