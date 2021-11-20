using System;

namespace Raid.Service
{
    public interface IFacet
    {
        object GetValue(IModelDataSource dataSource);
        bool TryUpgrade(IModelDataSource dataSource, Version from, out object upgradedData);
        object Merge(ModelScope scope, object previous = null);
    }
}
