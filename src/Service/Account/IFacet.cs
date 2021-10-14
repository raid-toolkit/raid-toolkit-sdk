using System;
using System.Reflection;
using Il2CppToolkit.Runtime;
using Raid.Service.DataModel;

namespace Raid.Service
{
    public interface IFacet
    {
        object GetValue(UserAccount account);
        object Merge(ModelScope scope, object previous = null);
    }

    public abstract class Facet<T, U> : IFacet where U : Facet<T, U> where T : class
    {
        protected abstract T Merge(ModelScope scope, T previous = null);

        object IFacet.Merge(ModelScope scope, object previous)
        {
            return Merge(scope, (T)previous);
        }

        object IFacet.GetValue(UserAccount account)
        {
            return GetValue(account);
        }

        public T GetValue(UserAccount account)
        {
            return account.Get<T>(FacetAttribute.GetName(typeof(U)));
        }

        public static T ReadValue(UserAccount account)
        {
            return account.Get<T>(FacetAttribute.GetName(typeof(U)));
        }
    }

    public class FacetAttribute : Attribute
    {
        public string Name { get; }
        public FacetAttribute(string name) => Name = name;

        public static string GetName(Type type)
        {
            return type.GetCustomAttribute<FacetAttribute>().Name;
        }
    }
}