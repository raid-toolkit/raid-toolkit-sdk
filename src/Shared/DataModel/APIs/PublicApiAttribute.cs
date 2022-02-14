using System;

namespace Raid.Toolkit.DataModel
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Event)]
    public class PublicApiAttribute : Attribute
    {
        public string Name { get; }
        public PublicApiAttribute(string name)
        {
            Name = name;
        }
    }
}
