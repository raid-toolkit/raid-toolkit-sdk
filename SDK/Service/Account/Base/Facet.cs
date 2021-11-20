using System;

namespace Raid.Service
{
    public abstract class FacetBase<ValueType, FacetType, DataSourceType> : IFacet
        where ValueType : class
        where FacetType : FacetBase<ValueType, FacetType, DataSourceType>
        where DataSourceType : IModelDataSource
    {
        protected abstract ValueType Merge(ModelScope scope, ValueType previous = null);
        public object Upgrade(IModelDataSource dataSource, Version from)
        {
            throw new NotSupportedException("Upgrade not supported");
        }

        object IFacet.Merge(ModelScope scope, object previous)
        {
            return Merge(scope, (ValueType)previous);
        }

        object IFacet.GetValue(IModelDataSource dataSource)
        {
            return GetValue((DataSourceType)dataSource);
        }

        public ValueType GetValue(DataSourceType account)
        {
            return account.Get<ValueType>(FacetAttribute.GetName(typeof(FacetType)));
        }

        public static ValueType ReadValue(DataSourceType account)
        {
            return account.Get<ValueType>(FacetAttribute.GetName(typeof(FacetType)));
        }
    }
}
