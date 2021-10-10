using Il2CppToolkit.Runtime;

namespace Raid.Service
{
    public interface IFacet
    {
        string Id { get; }
        object Merge(ModelScope scope, object previous = null);
    }
    public abstract class Facet<T> : IFacet where T : class
    {
        public abstract string Id { get; }
        protected abstract T Merge(ModelScope scope, T previous = null);

        object IFacet.Merge(ModelScope scope, object previous)
        {
            return Merge(scope, (T)previous);
        }
    }
}