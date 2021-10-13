using System;

namespace Raid.Service
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Class)]
    public class PublicApiAttribute : Attribute
    {
        public string Name { get; }
        public PublicApiAttribute(string name)
        {
            Name = name;
        }
    }
}