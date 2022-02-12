using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProcessMemoryUtilities.Managed;
using ProcessMemoryUtilities.Native;
using Raid.Service.DataServices;
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
        private readonly Dictionary<Version, ulong> VersionToOffset = new()
        {
            { Version.Parse("2020.3.16.40302"), 0x1942BB0 },
        };

        private FrameRateSettings Settings => AppSettings.Value.FrameRate;
        private readonly ILogger<MainService> Logger;
        private readonly IOptions<AppSettings> AppSettings;
        private readonly IServiceProvider ServiceProvider;
        private readonly RaidInstanceFactory Factory;

        public FrameRateService(
            IOptions<AppSettings> settings,
            ILogger<MainService> logger,
            IServiceProvider serviceProvider,
            RaidInstanceFactory factory)
        {
            AppSettings = settings;
            Logger = logger;
            ServiceProvider = serviceProvider;
            Factory = factory;
        }

        protected override async Task ExecuteOnceAsync(CancellationToken token)
        {
            var userData = ServiceProvider.GetRequiredService<AppData>();
            var viewKeyProvider = ServiceProvider.GetRequiredService<ViewKeyInfoProvider>();
            foreach (var (id, instance) in Factory.Instances)
            {
                var process = instance.Runtime.TargetProcess;
                var accountId = instance.UserAccount.UserId;

                var viewKeyData = viewKeyProvider.GetValue(accountId);
                // viewKeyData.ViewId
            }
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
