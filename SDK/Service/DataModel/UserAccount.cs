using System;
using Microsoft.Extensions.DependencyInjection;
using Raid.Service.DataServices;

namespace Raid.Service
{
    public class UserAccount
    {
        public readonly string UserId;
        public DateTime? LastUpdated { get; private set; }
        private readonly EventService EventService;
        private readonly IPersistedDataManager<AccountDataContext> DataManager;

        public UserAccount(string userId, IServiceScope serviceScope)
        {
            UserId = userId;
            EventService = serviceScope.ServiceProvider.GetRequiredService<EventService>();
            DataManager = serviceScope.ServiceProvider.GetRequiredService<IPersistedDataManager<AccountDataContext>>();
        }

        public void Load()
        {
            Upgrade();
        }

        private void Upgrade()
        {
            DataManager.Upgrade(new AccountDataContext(UserId));
        }

        public bool Update(Il2CppToolkit.Runtime.Il2CsRuntimeContext runtime)
        {
            AccountDataContext context = new(UserId);
            var updateResult = DataManager.Update(runtime, context);
            if (updateResult == UpdateResult.Updated)
            {
                if (DataManager.Index.TryRead(context, out var index) && index.LastUpdated.HasValue)
                {
                    LastUpdated = index.LastUpdated.Value;
                }
                EventService.EmitAccountUpdated(UserId);
            }
            return updateResult != UpdateResult.Failed;
        }
    }
}
