using System;
using System.Reflection;
using Il2CppToolkit.Runtime;
using Raid.Service.DataModel;

namespace Raid.Service
{
    public abstract class FacetBase<T, U, D> : IFacet
        where T : class
        where U : FacetBase<T, U, D>
        where D : IModelDataSource
    {
        protected abstract T Merge(ModelScope scope, T previous = null);

        object IFacet.Merge(ModelScope scope, object previous)
        {
            return Merge(scope, (T)previous);
        }

        object IFacet.GetValue(IModelDataSource dataSource)
        {
            return GetValue((D)dataSource);
        }

        public T GetValue(D account)
        {
            return account.Get<T>(FacetAttribute.GetName(typeof(U)));
        }

        public static T ReadValue(D account)
        {
            return account.Get<T>(FacetAttribute.GetName(typeof(U)));
        }
    }
}