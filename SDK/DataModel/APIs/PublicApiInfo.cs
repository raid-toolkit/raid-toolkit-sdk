using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Raid.DataModel
{
    public class PublicApiInfo<T>
    {
        public readonly string Name;
        public readonly IReadOnlyDictionary<string, ApiMemberDefinition> Members;

        public PublicApiInfo()
        {
            Name = typeof(T).GetCustomAttribute<PublicApiAttribute>().Name;
            Members = typeof(T).GetMembers()
                    .Select(member => new ApiMemberDefinition(Name, member, member.GetCustomAttribute<PublicApiAttribute>()))
                    .Where(member => member.Attribute != null)
                    .ToDictionary(entry => entry.PublicName);
        }

        public U GetPublicApi<U>(string name, out string scope) where U : MemberInfo
        {
            if (Members.TryGetValue(name, out ApiMemberDefinition member) && member.MemberInfo is U result)
            {
                scope = member.Scope;
                return result;
            }
            throw new MissingMethodException(member?.Scope ?? "", name);
        }

        public ApiMemberDefinition GetMember<U>(string memberName) where U : MemberInfo
        {
            return Members.TryGetValue(memberName, out ApiMemberDefinition member) && member.MemberInfo is U
                ? member
                : throw new MissingMethodException(member?.Scope ?? "", memberName);
        }
    }
    public class ApiMemberDefinition
    {
        public string Scope { get; }
        public string Name => MemberInfo.Name;
        public string PublicName => Attribute.Name;
        public MemberInfo MemberInfo { get; }
        public PublicApiAttribute Attribute { get; }

        public ApiMemberDefinition(string scope, MemberInfo memberInfo, PublicApiAttribute attribute)
        {
            Scope = scope;
            MemberInfo = memberInfo;
            Attribute = attribute;
        }
    }
}