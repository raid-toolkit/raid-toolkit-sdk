using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Il2CppToolkit.Common.Errors;
using Il2CppToolkit.Model;
using Il2CppToolkit.ReverseCompiler;
using Il2CppToolkit.Runtime;
using Microsoft.Win32;
using Raid.Model;
using RaidExtractor.Core;

namespace Raid.Extractor
{
    public static class Extensions
    {
        public static float AsFloat(this Plarium.Common.Numerics.Fixed num)
        {
            return (float)Math.Round(num.m_rawValue / (double)uint.MaxValue, 2);
        }
        public static ArtifactBonus AsBonus(this SharedModel.Meta.Artifacts.Bonuses.ArtifactBonus bonus)
        {
            return new ArtifactBonus
            {
                Kind = bonus._kindId.ToString(), // TODO: Is this equivalent?
                IsAbsolute = bonus._value._isAbsolute,
                Value = bonus._value._value.AsFloat(),
                Enhancement = bonus._powerUpValue.AsFloat(),
                Level = bonus._level
            };
        }
    }
    public class Extractor
    {
        private readonly Client.Model.AppModel m_appModel;
        private readonly Il2CsRuntimeContext m_runtime;
        private readonly IReadOnlyList<SharedModel.Meta.Artifacts.Artifact> m_artifacts;

        public Extractor(Process process)
        {
            m_runtime = new(process);
            var statics = Client.App.SingleInstance<Client.Model.AppModel>.method_get_Instance.GetMethodInfo(m_runtime).DeclaringClass.StaticFields
                .As<AppModelStaticFields>();
            m_appModel = statics.Instance;
            var artifactStorageResolver = SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageResolver.GetInstance(m_runtime);

            Client.Model.Guard.UserWrapper userWrapper = m_appModel._userWrapper;
            if (userWrapper.Artifacts.ArtifactData.StorageMigrationState == SharedModel.Meta.Artifacts.ArtifactStorage.ArtifactStorageMigrationState.Migrated)
            {
                var storage = artifactStorageResolver._implementation as Client.Model.Gameplay.Artifacts.ExternalArtifactsStorage;
                List<SharedModel.Meta.Artifacts.Artifact> innerList = new();
                foreach ((var key, var value) in storage._state._artifacts)
                {
                    innerList.Add(value);
                }

                m_artifacts = innerList;
            }
            else
            {
                m_artifacts = userWrapper.Artifacts.ArtifactData.Artifacts;
            }
        }

        public AccountDump Extract()
        {
            Console.WriteLine($"Extracting {m_artifacts.Count} artifacts...");
            List<Artifact> artifacts = new();
            foreach (var artifact in m_artifacts)
            {
                if (artifacts.Count % 100 == 0)
                {
                    Console.WriteLine($"{artifacts.Count} / {m_artifacts.Count}");
                }
                artifacts.Add(new Artifact
                {
                    Id = artifact._id,
                    SellPrice = artifact._sellPrice,
                    Price = artifact._price,
                    Level = artifact._level,
                    IsActivated = artifact._isActivated,
                    Kind = artifact._kindId.ToString(), // TODO: Is this equivalent?
                    Rank = artifact._rankId.ToString(), // TODO: Is this equivalent?
                    Rarity = artifact._rarityId.ToString(), // TODO: Is this equivalent?
                    SetKind = artifact._setKindId.ToString(), // TODO: Is this equivalent?
                    IsSeen = artifact._isSeen,
                    FailedUpgrades = artifact._failedUpgrades,
                    PrimaryBonus = artifact._primaryBonus.AsBonus(),
                    RequiredFraction = artifact._requiredFraction.ToString(), // TODO: Is this equivalent?
                    SecondaryBonuses = artifact._secondaryBonuses?.Select(bonus => bonus.AsBonus()).ToList()
                });
            }
            return new AccountDump
            {
                Artifacts = artifacts
            };
        }

        [Size(16)]
        private struct AppModelStaticFields
        {
            [Offset(8)]
            public Client.Model.AppModel Instance;
        }
    }
}