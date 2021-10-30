using Raid.DataModel;

namespace Raid.Service
{
    public interface IAccountFacet : IFacet { }

    public abstract class UserAccountFacetBase<T, U> : FacetBase<T, U, UserAccount>, IAccountFacet
        where T : class
        where U : UserAccountFacetBase<T, U>
    { }
}