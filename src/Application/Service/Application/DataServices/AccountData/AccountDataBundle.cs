namespace Raid.Service.DataServices
{
    public class AccountDataBundle
    {
        public readonly AcademyProvider Academy;
        public readonly AccountInfoProvider AccountInfo;
        public readonly ArenaProvider Arena;
        public readonly ArtifactsProvider Artifacts;
        public readonly HeroesProvider Heroes;
        public readonly ResourcesProvider Resources;
        public AccountDataBundle(
            AcademyProvider academy,
            AccountInfoProvider accountInfo,
            ArenaProvider arena,
            ArtifactsProvider artifacts,
            HeroesProvider heroes,
            ResourcesProvider resources
            )
        {
            Academy = academy;
            AccountInfo = accountInfo;
            Arena = arena;
            Artifacts = artifacts;
            Heroes = heroes;
            Resources = resources;
        }
    }
}