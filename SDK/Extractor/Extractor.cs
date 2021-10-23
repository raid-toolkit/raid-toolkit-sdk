using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Il2CppToolkit.Runtime;
using RaidExtractor.Core;

namespace Raid.Extractor
{
    public static class Extensions
    {
        public static float AsFloat(this Plarium.Common.Numerics.Fixed num) => (float)Math.Round(num.m_rawValue / (double)uint.MaxValue, 2);

        public static ArtifactBonus Dump(this SharedModel.Meta.Artifacts.Bonuses.ArtifactBonus bonus) => new ArtifactBonus
        {
            Kind = bonus._kindId.ToString(), // TODO: Is this equivalent?
            IsAbsolute = bonus._value._isAbsolute,
            Value = bonus._value._value.AsFloat(),
            Enhancement = bonus._powerUpValue.AsFloat(),
            Level = bonus._level
        };

        public static Artifact Dump(this SharedModel.Meta.Artifacts.Artifact artifact) => new Artifact
        {
            Id = artifact._id,
            SellPrice = artifact._sellPrice,
            Price = artifact._price,
            Level = artifact._level,
            IsActivated = artifact._isActivated,
            Kind = artifact._kindId.ToString(), // TODO: Is this equivalent?
            Rank = artifact._rankId.ToString(), // TODO: Is this equivalent?
            Rarity = artifact._rarityId.ToString(), // TODO: Is this equivalent?
            SetKind = artifact._setKindId.ToString(), // TODO: Is this equivalent?
            IsSeen = artifact._isSeen,
            FailedUpgrades = artifact._failedUpgrades,
            PrimaryBonus = artifact._primaryBonus.Dump(),
            RequiredFraction = artifact._requiredFraction.ToString(), // TODO: Is this equivalent?
            SecondaryBonuses = artifact._secondaryBonuses?.Select(bonus => bonus.Dump()).ToList()
        };

        public static Skill Dump(this SharedModel.Meta.Skills.Skill skill) => new Skill
        {
            Id = skill.Id,
            TypeId = skill.TypeId,
            Level = skill.Level
        };

        public static ICollection<Hero> Dump(
            this IEnumerable<SharedModel.Meta.Heroes.Hero> heroes,
            IReadOnlyDictionary<int, SharedModel.Meta.Artifacts.HeroArtifactData> artifactData,
            IReadOnlyDictionary<int, SharedModel.Meta.Heroes.HeroType> heroData
        ) => heroes.Select(hero =>
        {
            SharedModel.Meta.Heroes.HeroType heroType = heroData[hero.TypeId];
            Hero result = new()
            {
                // instance fields
                Id = hero.Id,
                TypeId = hero.TypeId,
                Grade = hero.Grade.ToString(),
                Level = hero.Level,
                Experience = hero.Experience,
                FullExperience = hero.FullExperience,
                Locked = hero.Locked,
                InStorage = hero.InStorage,
                Marker = hero.Marker.ToString(),
                // extras
                Masteries = hero.MasteryData?.Masteries.ToList() ?? new(),
                AssignedMasteryScrolls = new(hero.MasteryData?.TotalAmount.Select(mastery => new KeyValuePair<string, int>(mastery.Key.ToString(), mastery.Value)) ?? new Dictionary<string, int>()),
                UnassignedMasteryScrolls = new(hero.MasteryData?.CurrentAmount.Select(mastery => new KeyValuePair<string, int>(mastery.Key.ToString(), mastery.Value)) ?? new Dictionary<string, int>()),
                TotalMasteryScrolls = new Dictionary<string, int>(),
                Artifacts = artifactData.TryGetValue(hero.Id, out SharedModel.Meta.Artifacts.HeroArtifactData data) ? data?.ArtifactIdByKind.Values.ToList() ?? new() : new(),
                Skills = hero.Skills?.Select(Extensions.Dump).ToList() ?? new(),
                // type fields
                Name = heroType.Name.DefaultValue,
                Fraction = heroType.Fraction.ToString(),
                Element = heroType.Element.ToString(),
                Rarity = heroType.Rarity.ToString(),
                Role = heroType.Role.ToString(),
                AwakenLevel = heroType.Id % 10,
                Accuracy = heroType.BaseStats.Accuracy.AsFloat(),
                Attack = heroType.BaseStats.Attack.AsFloat(),
                Defense = heroType.BaseStats.Defence.AsFloat(),
                Health = heroType.BaseStats.Health.AsFloat(),
                Speed = heroType.BaseStats.Speed.AsFloat(),
                Resistance = heroType.BaseStats.Resistance.AsFloat(),
                CriticalChance = heroType.BaseStats.CriticalChance.AsFloat(),
                CriticalDamage = heroType.BaseStats.CriticalDamage.AsFloat(),
                CriticalHeal = heroType.BaseStats.CriticalHeal.AsFloat(),
            };

            result.TotalMasteryScrolls = result.TotalMasteryScrolls.Concat(result.AssignedMasteryScrolls)
                    .Concat(result.UnassignedMasteryScrolls)
                    .GroupBy(x => x.Key)
                    .ToDictionary(x => x.Key, x => x.Sum(y => y.Value));

            var multiplier = StaticResources.Multipliers.First(m => m.stars == hero.Grade && m.level == hero.Level);
            result.Attack = (int)Math.Round(result.Attack * multiplier.multiplier);
            result.Defense = (int)Math.Round(result.Defense * multiplier.multiplier);
            result.Health = (int)Math.Round(result.Health * multiplier.multiplier) * 15;

            return result;
        }).ToList();
    }
    public class Extractor
    {
        private readonly Client.Model.AppModel m_appModel;
        private readonly Il2CsRuntimeContext m_runtime;
        private readonly IReadOnlyList<SharedModel.Meta.Artifacts.Artifact> m_artifacts;

        public Extractor(Process process)
        {
            m_runtime = new(process);
            var statics = Client.App.SingleInstance<Client.Model.AppModel>.method_get_Instance.GetMethodInfo(m_runtime).DeclaringClass.StaticFields
                .As<AppModelStaticFields>();
            m_appModel = statics.Instance;
            var artifactStorageResolver = SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageResolver.GetInstance(m_runtime);

            Client.Model.Guard.UserWrapper userWrapper = m_appModel._userWrapper;
            if (userWrapper.Artifacts.ArtifactData.StorageMigrationState == SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageMigrationState.Migrated)
            {
                var storage = artifactStorageResolver._implementation as Client.Model.Gameplay.Artifacts.ExternalArtifactsStorage;
                List<SharedModel.Meta.Artifacts.Artifact> innerList = new();
                foreach ((var key, var value) in storage._state._artifacts)
                {
                    innerList.Add(value);
                }

                m_artifacts = innerList;
            }
            else
            {
                m_artifacts = userWrapper.Artifacts.ArtifactData.Artifacts;
            }
        }

        public AccountDump Extract()
        {
            IReadOnlyDictionary<int, SharedModel.Meta.Artifacts.HeroArtifactData> artifactData = m_appModel._userWrapper.Artifacts.ArtifactData.ArtifactDataByHeroId;
            IReadOnlyDictionary<int, SharedModel.Meta.Heroes.HeroType> heroData = (m_appModel.StaticDataManager as Client.Model.Gameplay.StaticData.ClientStaticDataManager).StaticData.HeroData.HeroTypeById;
            List<Artifact> artifacts = m_artifacts.Select(Extensions.Dump).ToList();
            return new AccountDump
            {
                Artifacts = artifacts,
                ArenaLeague = m_appModel._userWrapper.Arena.LeagueId.HasValue ? m_appModel._userWrapper.Arena.LeagueId.Value.ToString() : null,
                GreatHall = new(m_appModel._userWrapper.Village.VillageData.CapitolBonusLevelByStatByElement.Select((kvp) =>
                    new KeyValuePair<SharedModel.Meta.Heroes.Element, Dictionary<SharedModel.Battle.Effects.StatKindId, int>>(kvp.Key, new Dictionary<SharedModel.Battle.Effects.StatKindId, int>(kvp.Value))
                )),
                Heroes = m_appModel._userWrapper.Heroes.HeroData.HeroById.Values.Dump(artifactData, heroData).ToList(),
                StagePresets = new(m_appModel._userWrapper.Heroes.HeroData.BattlePresets),
                Shards = new(m_appModel._userWrapper.Shards.ShardData.Shards.Select(shard => new KeyValuePair<string, ShardInfo>(shard.TypeId.ToString(), new ShardInfo() { Count = shard.Count, SummonData = new() })))
            };
        }

        [Size(16)]
        private struct AppModelStaticFields
        {
            [Offset(8)]
            public Client.Model.AppModel Instance;
        }
    }
}