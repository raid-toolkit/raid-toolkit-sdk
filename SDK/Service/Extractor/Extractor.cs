using System;
using System.Collections.Generic;
using System.Linq;
using Raid.DataModel;
using Raid.Service;

namespace RaidExtractor.Core
{
    public class Extractor
    {
        private readonly StaticDataCache StaticDataCache;
        public Extractor(StaticDataCache staticData)
        {
            StaticDataCache = staticData;
        }

        private ArtifactBonus FromStatBonus(ArtifactStatBonus bonus)
        {
            return new()
            {
                Enhancement = (float)bonus.GlyphPower,
                IsAbsolute = bonus.Absolute,
                Kind = bonus.KindId.ToString(),
                Level = bonus.Level,
                Value = bonus.Value
            };
        }

        public AccountDump DumpAccount(UserAccount account)
        {
            var accountFacet = AccountFacet.ReadValue(account);
            var artifacts = ArtifactsFacet.ReadValue(account);
            var heroes = HeroesFacet.ReadValue(account);
            var arena = ArenaFacet.ReadValue(account);
            var resources = ResourcesFacet.ReadValue(account);
            // var shards = ArenaFacet.ReadValue(account);
            var staticData = StaticDataFacet.ReadValue(StaticDataCache);

            return new AccountDump()
            {
                LastUpdated = account.LastUpdated.ToString("o"),
                ArenaLeague = arena.ClassicArena.LeagueId.ToString(),
                GreatHall = arena.GreatHallBonuses.ToDictionary(
                    bonus => bonus.Affinity.ToString().ToCamelCase(),
                    bonus => (IReadOnlyDictionary<string, int>)bonus.Levels.ToDictionary(levels => levels.Key.ToString().ToCamelCase(), levels => levels.Value)),
                Shards = resources.Shards.ToDictionary(
                    shard => shard.Key.ToString(),
                    shard => new ShardInfo() { Count = shard.Value, SummonData = new() }),
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
                    SecondaryBonuses = artifact.SecondaryBonuses?.Select(FromStatBonus).ToArray(),
                }).ToArray(),
                StagePresets = heroes.BattlePresets,
                Heroes = heroes.Heroes.Values.Where(hero => !hero.Deleted).Select(hero =>
                {
                    var heroType = staticData.HeroData.HeroTypes[hero.TypeId];
                    var multiplier = StaticResources.Multipliers.First(m => m.stars == (int)Enum.Parse<SharedModel.Meta.Heroes.HeroGrade>(hero.Rank) && m.level == hero.Level);
                    Hero newHero = new()
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
                        AssignedMasteryScrolls = hero.AssignedMasteryScrolls,
                        UnassignedMasteryScrolls = hero.UnassignedMasteryScrolls,
                        TotalMasteryScrolls = hero.TotalMasteryScrolls,
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

                    return newHero;
                }).ToArray()
            };
        }
    }
}
