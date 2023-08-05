using System.Collections.Generic;

namespace Raid.Toolkit.DataModel;

public interface IAccountData
{
    AccountBase Account { get; }
    ArenaData Arena { get; }
    AcademyData Academy { get; }
    ArtifactsDataObject Artifacts { get; }
    HeroData Heroes { get; }
    Resources Resources { get; }
}

public class ArtifactsDataObject : Dictionary<int, Artifact>
{
}
