using System;

namespace Raid.DataModel
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Class | AttributeTargets.Interface)]
    public class PublicApiAttribute : Attribute
    {
        public string Name { get; }
        public PublicApiAttribute(string name)
        {
            Name = name;
        }
    }
}