using System;
using System.Collections.Generic;
using System.Linq;
using Raid.Service;
using Raid.Service.DataModel;
using SharedModel.Meta.Masteries;

namespace RaidExtractor.Core
{
    public static class Extractor
    {
        private static ArtifactBonus FromStatBonus(ArtifactStatBonus bonus) => new ArtifactBonus()
        {
            Enhancement = (float)bonus.GlyphPower,
            IsAbsolute = bonus.Absolute,
            Kind = bonus.KindId.ToString(),
            Level = bonus.Level,
            Value = bonus.Value
        };

        public static AccountDump DumpAccount(UserAccount account)
        {
            var artifacts = ArtifactsFacet.ReadValue(account);
            var heroes = HeroesFacet.ReadValue(account);
            var arena = ArenaFacet.ReadValue(account);
            var resources = ResourcesFacet.ReadValue(account);
            // var shards = ArenaFacet.ReadValue(account);
            var staticData = StaticDataFacet.ReadValue(StaticDataCache.Instance);

            return new AccountDump()
            {
                ArenaLeague = arena.LeagueId.ToString(),
                GreatHall = arena.GreatHallBonuses.ToDictionary(bonus => bonus.Affinity, bonus => bonus.Levels),
                Shards = resources.Shards.ToDictionary(shard => shard.Key.ToString(), shard => new ShardInfo() { Count = shard.Value, SummonData = new() }),
                Artifacts = artifacts.Values.Select(artifact => new Artifact()
                {
                    Id = artifact.Id,
                    Kind = artifact.KindId.ToString(),
                    SetKind = artifact.SetKindId.ToString(),
                    Level = artifact.Level,
                    Rank = artifact.Rank.ToString(),
                    Rarity = artifact.RarityId.ToString(),
                    IsActivated = artifact.Activated,
                    IsSeen = artifact.Seen,
                    FailedUpgrades = artifact.FailedUpgrades,
                    RequiredFraction = artifact.Faction.ToString(),
                    SellPrice = artifact.SellPrice,
                    Price = artifact.Price,
                    PrimaryBonus = FromStatBonus(artifact.PrimaryBonus),
                    SecondaryBonuses = artifact.SecondaryBonuses.Select(FromStatBonus).ToArray(),
                }).ToArray(),
                Heroes = heroes.Values.Select(hero =>
                {
                    var heroType = staticData.HeroData.HeroTypes[hero.TypeId];
                    var multiplier = StaticResources.Multipliers.First(m => m.stars == hero.Rank && m.level == hero.Level);
                    Hero newHero = new Hero()
                    {
                        // instance fields
                        Id = hero.Id,
                        TypeId = hero.TypeId,
                        Grade = hero.Rank.ToString(),
                        Level = hero.Level,
                        Experience = hero.Experience,
                        FullExperience = hero.FullExperience,
                        Locked = hero.Locked,
                        InStorage = hero.InVault,
                        Marker = hero.Marker.ToString(),
                        // extras
                        Masteries = hero.Masteries?.Cast<int>().ToList() ?? new(),
                        AssignedMasteryScrolls = hero.AssignedMasteryScrolls?.ToEnumDictionary() ?? new(),
                        UnassignedMasteryScrolls = hero.UnassignedMasteryScrolls?.ToEnumDictionary() ?? new(),
                        TotalMasteryScrolls = hero.TotalMasteryScrolls?.ToEnumDictionary() ?? new(),
                        Artifacts = hero.EquippedArtifactIds?.Values.ToArray(),
                        Skills = hero.SkillsById?.Values.Select(skill => new Skill() { TypeId = skill.TypeId, Id = skill.Id, Level = skill.Level, }).ToList() ?? new(),
                        // type fields
                        Name = heroType.Name.DefaultValue,
                        Fraction = heroType.Faction.ToString(),
                        Element = heroType.Affinity.ToString(),
                        Rarity = heroType.Rarity.ToString(),
                        Role = heroType.Role.ToString(),
                        AwakenLevel = heroType.TypeId % 10,
                        Accuracy = heroType.UnscaledStats.Accuracy,
                        Attack = (int)Math.Round(heroType.UnscaledStats.Attack * multiplier.multiplier),
                        Defense = (int)Math.Round(heroType.UnscaledStats.Defense * multiplier.multiplier),
                        Health = (int)Math.Round(heroType.UnscaledStats.Health * multiplier.multiplier) * 15,
                        Speed = heroType.UnscaledStats.Speed,
                        Resistance = heroType.UnscaledStats.Resistance,
                        CriticalChance = heroType.UnscaledStats.CriticalChance,
                        CriticalDamage = heroType.UnscaledStats.CriticalDamage,
                        CriticalHeal = heroType.UnscaledStats.CriticalHeal,
                    };

                    newHero.TotalMasteryScrolls = (Dictionary<MasteryPointType, int>)(newHero.TotalMasteryScrolls.Concat(newHero.AssignedMasteryScrolls)
                        .Concat(newHero.UnassignedMasteryScrolls)
                        .GroupBy(x => x.Key)
                        .ToDictionary(x => x.Key, x => x.Sum(y => y.Value)));

                    return newHero;
                }).ToArray()
            };
        }
    }
}