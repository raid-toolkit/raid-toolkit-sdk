using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Il2CppToolkit.Runtime;
using Raid.Service.DataModel;

namespace Raid.Service
{
    public class RaidInstance
    {
        public static IEnumerable<RaidInstance> Instances { get { return m_instances.Values.ToArray(); } }
        private static ConcurrentDictionary<int, RaidInstance> m_instances = new();
        private Process m_process;
        private readonly Il2CsRuntimeContext m_runtime;

        public RaidInstance(Process process)
        {
            m_process = process;
            m_runtime = new Il2CsRuntimeContext(process);
            m_instances.TryAdd(process.Id, this);
            m_process.Disposed += HandleProcessDisposed;
        }

        private void HandleProcessDisposed(object sender, EventArgs e)
        {
            m_runtime.Dispose();
            m_instances.TryRemove(new(m_process.Id, this));
        }

        private Account account;
        public void Update()
        {
            var statics = Client.App.SingleInstance<Client.Model.AppModel>.method_get_Instance.GetMethodInfo(m_runtime).DeclaringClass.StaticFields
                .As<AppModelStaticFields>();
            var appModel = statics.Instance;
            var userWrapper = appModel._userWrapper;
            var accountData = userWrapper.Account.AccountData;
            var gameSettings = userWrapper.UserGameSettings.GameSettings;
            var socialWrapper = userWrapper.Social.SocialData;
            var globalId = socialWrapper.PlariumGlobalId;
            var socialId = socialWrapper.SocialId;
            account = new Account
            {
                Id = String.Join('_', globalId, socialId).Sha256(),
                Avatar = gameSettings.Avatar,
                Name = gameSettings.Name,
                Level = accountData.Level,
                Power = (int)Math.Round(accountData.TotalPower, 0)
            };
        }

        [Size(16)]
        private struct AppModelStaticFields
        {
            [Offset(8)]
#pragma warning disable 649
            public Client.Model.AppModel Instance;
#pragma warning restore 649
        }
    }
}