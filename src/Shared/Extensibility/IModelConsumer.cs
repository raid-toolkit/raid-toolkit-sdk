using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Raid.Toolkit.Extensibility
{
    /// <summary>
    /// Implemented on an <see cref="IExtensionPackage"/> to indicate this extension consumes the runtime model
    /// </summary>
    public interface IModelConsumer
    {
        public IEnumerable<Regex> TypeNameMatchers { get; }
    }
}
