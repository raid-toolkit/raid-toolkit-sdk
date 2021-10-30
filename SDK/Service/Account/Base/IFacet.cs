using Raid.DataModel;

namespace Raid.Service
{
    public interface IFacet
    {
        object GetValue(IModelDataSource dataSource);
        object Merge(ModelScope scope, object previous = null);
    }
}