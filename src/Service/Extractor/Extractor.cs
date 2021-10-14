using System.Linq;
using Raid.Service;
using Raid.Service.DataModel;

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

            return new AccountDump()
            {
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
                Heroes = heroes.Values.Select(hero => new Hero()
                {
                    // TODO: HeroBaseType
                    // Accuracy = hero
                }).ToArray()
            };
        }
    }
}