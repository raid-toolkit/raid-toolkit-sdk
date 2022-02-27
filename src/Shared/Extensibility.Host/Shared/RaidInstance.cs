using Client.Model;
using Il2CppToolkit.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raid.Toolkit.Extensibility.Shared
{
	public class RaidInstance : IRaidInstance
	{
        public string Id { get; }
        private string AccountName;
		public Il2CsRuntimeContext Runtime { get; }

		public RaidInstance(Process proc)
		{
			Runtime = new(proc);
            (Id, AccountName) = GetAccountIdAndName();
		}

        private (string, string) GetAccountIdAndName()
        {
            var appModel = Client.App.SingleInstance<AppModel>.method_get_Instance
                        .GetMethodInfo(Runtime).DeclaringClass.StaticFields
                        .As<SingleInstanceStaticFields<AppModel>>().Instance;

            var userWrapper = appModel._userWrapper;
            var socialWrapper = userWrapper.Social.SocialData;
            var globalId = socialWrapper.PlariumGlobalId;
            var socialId = socialWrapper.SocialId;
            return (string.Join('_', globalId, socialId).Sha256(), userWrapper.UserGameSettings.GameSettings.Name);
        }
    }
}
