using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Raid.Toolkit.Extensibility
{
    public class CodegenTypeFilter
    {
        private readonly Regex[] IncludeTypesArray;
        public IEnumerable<Regex> IncludeTypes => IncludeTypesArray;
        public CodegenTypeFilter(Regex[] includeTypes)
        {
            IncludeTypesArray = includeTypes;
        }
    }
}
