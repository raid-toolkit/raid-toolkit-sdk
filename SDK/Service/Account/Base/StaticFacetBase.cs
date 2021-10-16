using Raid.Service.DataModel;

namespace Raid.Service
{
    public interface IStaticFacet : IFacet { }

    public abstract class StaticFacetBase<T, U> : FacetBase<T, U, StaticDataCache>, IStaticFacet
        where T : class
        where U : StaticFacetBase<T, U>
    { }
}