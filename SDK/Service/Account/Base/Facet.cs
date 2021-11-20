using System;

namespace Raid.Service
{
    public abstract class FacetBase<ValueType, FacetType, DataSourceType> : IFacet
        where ValueType : class
        where FacetType : FacetBase<ValueType, FacetType, DataSourceType>
        where DataSourceType : IModelDataSource
    {
        protected abstract ValueType Merge(ModelScope scope, ValueType previous = null);

        public virtual bool TryUpgrade(IModelDataSource dataSource, Version from, out ValueType upgradedData)
        {
            upgradedData = null;
            return false;
        }

        bool IFacet.TryUpgrade(IModelDataSource dataSource, Version from, out object upgradedData)
        {
            if (TryUpgrade(dataSource, from, out ValueType newValue))
            {
                upgradedData = newValue;
                return true;
            }
            upgradedData = null;
            return false;
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
