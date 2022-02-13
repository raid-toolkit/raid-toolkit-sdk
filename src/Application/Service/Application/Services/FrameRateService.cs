using Client.RaidApp;
using Client.View.Views;
using Client.ViewModel.Contextes.ArtifactsUpgrade;
using Client.ViewModel.Contextes.Base;
using Client.ViewModel.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProcessMemoryUtilities.Managed;
using ProcessMemoryUtilities.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Raid.Service.Services
{
    public class FrameRateService : PollingBackgroundService
    {
        private static readonly TimeSpan kPollInterval = new(0, 0, 0, 0, 100);
        private static readonly FrameRateSettings DefaultSettings = new() { MaxFrameRate = 60, AutosetFramerate = true, ArtifactUpgradeFrameRate = 10 };
        private static readonly Dictionary<Version, ulong> VersionToOffset = new()
        {
            { Version.Parse("2020.3.16.40302"), 0x1942BB0 },
        };

        private protected override TimeSpan PollInterval => kPollInterval;
        private FrameRateSettings Settings => AppSettings.Value.FrameRate ?? DefaultSettings;
        private readonly ILogger<MainService> Logger;
        private readonly IOptions<AppSettings> AppSettings;
        private readonly IServiceProvider ServiceProvider;
        private readonly RaidInstanceFactory Factory;

        public FrameRateService(
            IOptions<AppSettings> settings,
            ILogger<MainService> logger,
            IServiceProvider serviceProvider,
            RaidInstanceFactory factory)
            : base(logger)
        {
            AppSettings = settings;
            Logger = logger;
            ServiceProvider = serviceProvider;
            Factory = factory;
        }

        protected override Task ExecuteOnceAsync(CancellationToken token)
        {
            foreach (var instance in Factory.Instances.Values)
            {
                var process = instance.Runtime.TargetProcess;
                long currentLimit = GetLimit(process);

                ModelScope scope = new(instance.Runtime);
                if (scope.RaidApplication._viewMaster is not RaidViewMaster viewMaster)
                    continue;
                ViewMeta topView = viewMaster._views[^1];
                if (topView.Key == ViewKey.ArtifactPowerUpOverlay &&
                    topView.View is OverlayView view &&
                    view.Context is ArtifactUpgradeOverlay overlay &&
                    overlay._activeTab._value == 0 && // upgrade tab
                    overlay._upgradeContext._progress._status._value == ProgressStatus.InProgress // actively upgrading
                    )
                {
                    if (currentLimit != Settings.ArtifactUpgradeFrameRate)
                        SetLimit(process, Settings.ArtifactUpgradeFrameRate);
                }
                else
                {
                    if (currentLimit != Settings.MaxFrameRate)
                        SetLimit(process, Settings.MaxFrameRate);
                }
            }
            return Task.CompletedTask;
        }

        private long GetLimit(Process proc)
        {
            return AccessMemory(proc, (hProcess, baseAddress, offset) =>
            {
                long currentFramerate = -1;
                bool success = NativeWrapper.ReadProcessMemory(hProcess, IntPtr.Add(baseAddress, (int)offset), ref currentFramerate);
                Logger.LogInformation($"Write Framerate: Success = {success}, Get Framerate = {currentFramerate}, LastError = {NativeWrapper.LastError}");
                return currentFramerate;
            });
        }

        private void SetLimit(Process proc, long framerate)
        {
            AccessMemory(proc, (hProcess, baseAddress, offset) =>
            {
                bool success = NativeWrapper.WriteProcessMemory(hProcess, IntPtr.Add(baseAddress, (int)offset), ref framerate);
                Logger.LogInformation($"Write Framerate: Success = {success}, Set Framerate = {framerate}, LastError = {NativeWrapper.LastError}");
                return true;
            });
        }

        private T AccessMemory<T>(Process proc, Func<IntPtr, IntPtr, ulong, T> fn)
        {
            ProcessModule unityPlayerModule = proc.Modules.Cast<ProcessModule>().SingleOrDefault(m => m.ModuleName == "UnityPlayer.dll");
            if (unityPlayerModule == null)
                throw new InvalidOperationException("Process not found");

            FileVersionInfo fvi = unityPlayerModule.FileVersionInfo;
            Version loadedVersion = new(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
            if (!VersionToOffset.TryGetValue(loadedVersion, out ulong offset))
                throw new InvalidOperationException($"Unknown UnityPlayer version: {loadedVersion}");

            IntPtr hProcess = NativeWrapper.OpenProcess(
                ProcessAccessFlags.Read | ProcessAccessFlags.Write,
                inheritHandle: true,
                proc.Id);
            Console.WriteLine($"OpenProcess: {hProcess}, LastError = {NativeWrapper.LastError}");
            try
            {
                return fn(hProcess, unityPlayerModule.BaseAddress, offset);
            }
            finally
            {
                NativeWrapper.CloseHandle(hProcess);
            }
        }
    }
}
