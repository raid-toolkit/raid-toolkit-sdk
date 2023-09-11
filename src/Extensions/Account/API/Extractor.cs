using System;
using System.Collections.Generic;
using System.Linq;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.Extension.Account;
using StaticDataWrapper = Raid.Toolkit.Extension.Account.StaticDataWrapper;

namespace RaidExtractor.Core
{
    public class Extractor
    {
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

        private static bool IsFactionGuardian(Raid.Toolkit.DataModel.Hero hero, AcademyData academy)
        {
            var guardiansByFaction = academy?.Guardians;
            return guardiansByFaction != null && hero.Type.Faction != null && hero.Type.Rarity != null
                && guardiansByFaction.TryGetValue(hero.Type.Faction.GetValueOrDefault(), out var guardiansByRarity)
                && guardiansByRarity.TryGetValue(hero.Type.Rarity.GetValueOrDefault(), out var data)
                && data.AssignedHeroes.Any(slot => slot.FirstHero == hero.Id || slot.SecondHero == hero.Id);
        }

        public AccountDump DumpAccount(AccountData accountData, StaticDataWrapper staticData, DateTime lastUpdated)
        {
            var accountFacet = accountData.Account;
            var artifacts = accountData.Artifacts;
            var heroes = accountData.Heroes;
            var arena = accountData.Arena;
            var resources = accountData.Resources;
            var academy = accountData.Academy;
            var heroTypes = staticData.HeroTypes.HeroTypes;

            return new AccountDump()
            {
                Id = accountFacet.Id,
                Name = accountFacet.Name,
                LastUpdated = lastUpdated.ToString("o"),
                ArenaLeague = arena.ClassicArena?.LeagueId.ToString(),
                GreatHall = arena.GreatHallBonuses.ToDictionary(
                    bonus => bonus.Affinity.ToString().ToCamelCase(),
                    bonus => (IReadOnlyDictionary<string, int>)bonus.Levels.ToDictionary(levels => levels.Key.ToString().ToCamelCase(), levels => levels.Value)),
                FactionGuardians = academy?.Guardians?.ToDictionary(
                    factionPair => factionPair.Key.ToString().ToCamelCase(),
                    factionPair => (IReadOnlyDictionary<string, int>)factionPair.Value.ToDictionary(
                        rarityPair => rarityPair.Key.ToString().ToCamelCase(),
                        rarityPair => rarityPair.Value.AssignedHeroes.Length
                    )),
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
                    var heroType = heroTypes[hero.TypeId];
                    var multiplier = StaticResources.Multipliers.First(m => m.stars == (int)Enum.Parse(typeof(SharedModel.Meta.Heroes.HeroGrade), hero.Rank) && m.level == hero.Level);
                    Hero newHero = new()
                    {
                        // instance fields
                        Id = hero.Id,
                        TypeId = hero.TypeId,
                        Grade = hero.Rank.ToString(),
                        Level = hero.Level,
                        EmpowerLevel = hero.EmpowerLevel,
                        Experience = hero.Experience,
                        FullExperience = hero.FullExperience,
                        Locked = hero.Locked,
                        InStorage = hero.InVault,
                        IsGuardian = IsFactionGuardian(hero, academy),
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
                        Fraction = heroType.Faction.GetValueOrDefault(),
                        Element = heroType.Affinity.GetValueOrDefault(),
                        Rarity = heroType.Rarity.GetValueOrDefault(),
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
