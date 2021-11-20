using System;

namespace Raid.Service
{
    public interface IFacet
    {
        object GetValue(IModelDataSource dataSource);
        object Upgrade(IModelDataSource dataSource, Version from);
        object Merge(ModelScope scope, object previous = null);
    }
}
