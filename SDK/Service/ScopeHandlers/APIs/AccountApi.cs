using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Raid.DataModel;
using Raid.Service.DataServices;
using SharedModel.Meta.Skills;
using Extractor = RaidExtractor.Core.Extractor;

namespace Raid.Service
{
    internal class AccountApi : ApiHandler<IAccountApi>, IAccountApi
    {
        private readonly StaticArenaProvider StaticArenaData;
        private readonly StaticArtifactProvider StaticArtifactData;
        private readonly StaticSkillProvider StaticSkillData;
        private readonly AppData UserData;
        private readonly Extractor Extractor;
        private readonly AccountDataBundle AccountData;
        public AccountApi(
            ILogger<AccountApi> logger,
            AppData userData,
            AccountDataBundle accountData,
            StaticArenaProvider staticArenaData,
            StaticArtifactProvider staticArtifactData,
            StaticSkillProvider staticSkillData,
            Extractor extractor,
            EventService eventService)
            : base(logger)
        {
            UserData = userData;
            AccountData = accountData;
            StaticArenaData = staticArenaData;
            StaticArtifactData = staticArtifactData;
            StaticSkillData = staticSkillData;
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
            return Task.FromResult(Extractor.DumpAccount(AccountData, accountId, UserData.GetAccount(accountId).LastUpdated ?? DateTime.UtcNow));
        }

        public Task<Resources> GetAllResources(string accountId)
        {
            return Task.FromResult((Resources)AccountData.Resources.GetValue(accountId));
        }

        public Task<Account[]> GetAccounts()
        {
            return Task.FromResult(UserData.UserAccounts.Select(AccountFromUserAccount).ToArray());
        }

        private Account AccountFromUserAccount(UserAccount account)
        {
            AccountBase result = AccountData.AccountInfo.GetValue(account.UserId);
            return Account.FromBase(result, account.LastUpdated);
        }

        public Task<Account> GetAccount(string accountId)
        {
            UserAccount userAccount = UserData.GetAccount(accountId);
            return Task.FromResult(AccountFromUserAccount(userAccount));
        }

        public Task<ArenaData> GetArena(string accountId)
        {
            return Task.FromResult((ArenaData)AccountData.Arena.GetValue(accountId));
        }

        public Task<AcademyData> GetAcademy(string accountId)
        {
            return Task.FromResult((AcademyData)AccountData.Academy.GetValue(accountId));
        }

        public Task<Artifact[]> GetArtifacts(string accountId)
        {
            return Task.FromResult(AccountData.Artifacts.GetValue(accountId).Values.ToArray());
        }

        public Task<Artifact> GetArtifactById(string accountId, int artifactId)
        {
            return Task.FromResult(AccountData.Artifacts.GetValue(accountId)[artifactId]);
        }

        public Task<Hero[]> GetHeroes(string accountId, bool snapshot = false)
        {
            var heroes = AccountData.Heroes.GetValue(accountId).Heroes.Values;
            return !snapshot
                ? Task.FromResult(heroes.ToArray())
                : Task.FromResult<Hero[]>(heroes.Select(hero => GetSnapshot(accountId, hero)).ToArray());
        }

        public Task<Hero> GetHeroById(string accountId, int heroId, bool snapshot = false)
        {
            var hero = AccountData.Heroes.GetValue(accountId).Heroes[heroId];
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
            var staticArenaData = StaticArenaData.GetValue(StaticDataContext.Default);
            var staticArtifactData = StaticArtifactData.GetValue(StaticDataContext.Default);
            var staticSkillData = StaticSkillData.GetValue(StaticDataContext.Default);
            var arenaData = AccountData.Arena.GetValue(accountId);
            var artifactData = AccountData.Artifacts.GetValue(accountId);
            HeroType type = hero.Type;
            HeroStatsCalculator stats = new(type, (int)Enum.Parse<SharedModel.Meta.Heroes.HeroGrade>(hero.Rank), hero.Level);

            // arena
            var greatHallBonus = arenaData.GreatHallBonuses?.FirstOrDefault(ghb => ghb.Affinity == type.Affinity);
            if (greatHallBonus != null)
                stats.ApplyBonuses(StatSource.GreatHall, greatHallBonus.Bonus.ToArray());

            if (staticArenaData.Leagues.TryGetValue(arenaData.ClassicArena.LeagueId.ToString(), out var league))
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
                    if (!staticArtifactData.ArtifactSetKinds.TryGetValue(setKindId, out ArtifactSetKind setKind))
                        continue;
                    int numSets = count / setKind.ArtifactCount;

                    if (numSets > 0)
                        stats.ApplyArtifactSetBonuses(numSets, setKind.StatBonuses);
                }
            }

            List<SkillSnapshot> skillSnapshots = new();
            foreach (var skill in hero.SkillsById.Values)
            {
                if (!staticSkillData.SkillTypes.TryGetValue(skill.TypeId, out var skillType))
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
