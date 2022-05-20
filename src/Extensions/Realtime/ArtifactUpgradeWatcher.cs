using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Client.RaidApp;
using Client.View.Views;
using Client.ViewModel.Contextes.ArtifactsUpgrade;
using Client.ViewModel.Contextes.Base;
using Client.ViewModel.DTO;
using Microsoft.Extensions.Logging;
using ProcessMemoryUtilities.Managed;
using ProcessMemoryUtilities.Native;

namespace Raid.Toolkit.Extension.Realtime
{
    public class FrameRateSettings
    {
        public int MaxFrameRate { get; set; } = 0xF0;
        public int ArtifactUpgradeFrameRate { get; set; } = 10;
        public bool AutosetFramerate { get; set; } = true;
    }

    public class ArtifactUpgradeWatcher : IDisposable
    {
        private static readonly Dictionary<Version, ulong> VersionToOffset = new()
        {
            { Version.Parse("2020.3.16.40302"), 0x1942BB0 },
            { Version.Parse("2020.3.31.21687"), 0x196BC68 },
        };

        private readonly FrameRateSettings Settings;
        private readonly ILogger<ArtifactUpgradeWatcher> Logger;
        private bool IsDisposed;

        public ArtifactUpgradeWatcher(ILogger<ArtifactUpgradeWatcher> logger)
        {
            Settings = new();
            Logger = logger;

            RealtimeService.ViewChanged += OnViewChanged;
        }

        private void OnViewChanged(object sender, ViewChangedEventArgs e)
        {
            Process process = e.Instance.Runtime.TargetProcess;
            long currentLimit = GetLimit(process);

            ViewMeta topView = e.ViewMeta;
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

        private static long GetLimit(Process proc)
        {
            return AccessMemory(proc, (hProcess, baseAddress, offset) =>
            {
                long currentFramerate = -1;
                bool success = NativeWrapper.ReadProcessMemory(hProcess, IntPtr.Add(baseAddress, (int)offset), ref currentFramerate);
                // Logger.LogInformation($"Read Framerate: Success = {success}, Get Framerate = {currentFramerate}, LastError = {NativeWrapper.LastError}");
                return currentFramerate;
            });
        }

        private void SetLimit(Process proc, long framerate)
        {
            _ = AccessMemory(proc, (hProcess, baseAddress, offset) =>
            {
                bool success = NativeWrapper.WriteProcessMemory(hProcess, IntPtr.Add(baseAddress, (int)offset), ref framerate);
                Logger.LogInformation($"Write Framerate: Success = {success}, Set Framerate = {framerate}, LastError = {NativeWrapper.LastError}");
                return true;
            });
        }

        private static T AccessMemory<T>(Process proc, Func<IntPtr, IntPtr, ulong, T> fn)
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
            try
            {
                return fn(hProcess, unityPlayerModule.BaseAddress, offset);
            }
            finally
            {
                _ = NativeWrapper.CloseHandle(hProcess);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    RealtimeService.ViewChanged -= OnViewChanged;
                }

                IsDisposed = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
