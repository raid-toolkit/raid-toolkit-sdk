using System.Linq;
using Raid.Toolkit.DataModel;
using Raid.Toolkit.DataModel.Enums;
using Raid.DataServices;
using Raid.Toolkit.Extensibility.Providers;
using Raid.Toolkit.Extensibility;
using Il2CppToolkit.Runtime;
using Client.Model;

namespace Raid.Toolkit.Extension.Account
{
	[DataType("academy")]
	public class AcademyDataObject : AcademyData
	{
	}

	public class AcademyProvider : DataProviderBase<AccountDataContext, AcademyDataObject>
	{
		private readonly IDataResolver<StaticDataContext, CachedDataStorage<PersistedDataStorage>, StaticAcademyDataObject> StaticAcademyProvider;

		public AcademyProvider(
			IDataResolver<AccountDataContext, CachedDataStorage<PersistedDataStorage>, AcademyDataObject> storage,
			IDataResolver<StaticDataContext, CachedDataStorage<PersistedDataStorage>, StaticAcademyDataObject> academyProvider) // TODO: Replace with a simple context/type accessor interface
			: base(storage)
		{
			StaticAcademyProvider = academyProvider;
		}

		public override bool Update(Il2CsRuntimeContext runtime, AccountDataContext context)
		{
			if (!StaticAcademyProvider.TryRead(StaticDataContext.Default, out StaticAcademyDataObject academyBonuses))
				return false;

			var appModel = Client.App.SingleInstance<AppModel>.method_get_Instance
						.GetMethodInfo(runtime).DeclaringClass.StaticFields
						.As<SingleInstanceStaticFields<AppModel>>().Instance;
			var academy = appModel._userWrapper.Academy.AcademyData;
			var guardians = academy.Guardians.SlotsByFraction.UnderlyingDictionary
				.ToDictionary(
					factionPair => (HeroFraction)factionPair.Key,
					factionPair => factionPair.Value.SlotsByRarity.ToDictionary(
						rarityPair => (HeroRarity)rarityPair.Key,
						rarityPair =>
						{
							var assignedHeroes = rarityPair.Value.Where(slot => slot.FirstHero.HasValue && slot.SecondHero.HasValue).ToArray();
							return new GuardianData()
							{
								StatBonuses = academyBonuses.GuardianBonusByRarity[(HeroRarity)rarityPair.Key].Take(assignedHeroes.Length).SelectMany(bonuses => bonuses).ToArray(),
								AssignedHeroes = assignedHeroes.Select(slot => new GuardiansSlot()
								{
									FirstHero = slot.FirstHero.Value,
									SecondHero = slot.SecondHero.Value
								}).ToArray()
							};
						}
					)
				);

			return PrimaryProvider.Write(context, new AcademyDataObject
			{
				Guardians = guardians
			});
		}
	}
}
