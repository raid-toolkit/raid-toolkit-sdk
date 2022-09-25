using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Hosting;
using Raid.Toolkit.Extensibility.Host.Services;

namespace Raid.Toolkit.Application.Core.Host
{
    public class AppService
    {
        private readonly IHostApplicationLifetime Lifetime;
        private readonly UpdateService? UpdateService;
        private readonly TaskCompletionSource StopSignal;
        public GitHub.Schema.Release? LatestRelease { get; private set; }

        public AppService(IHostApplicationLifetime lifetime, UpdateService updateService)
        {
            Lifetime = lifetime;
            UpdateService = updateService;
            UpdateService.UpdateAvailable += OnUpdateAvailable;
            StopSignal = new();
        }

        public AppService(IHostApplicationLifetime lifetime)
        {
            Lifetime = lifetime;
            StopSignal = new();
        }

        private void OnUpdateAvailable(object? sender, UpdateService.UpdateAvailbleEventArgs e)
        {
            LatestRelease = e.Release;
        }

        public Task WaitForStop()
        {
            return StopSignal.Task;
        }

        public void InstallUpdate()
        {
            if (LatestRelease == null)
                return; // TODO: Show error message?

            InstallUpdate(LatestRelease);
        }

        public async void InstallUpdate(GitHub.Schema.Release release)
        {
            if (UpdateService == null)
                return;
            try
            {
                await UpdateService.InstallRelease(release, AppHost.ExecutableName);
                Restart(postUpdate: true);
            }
            catch { }
        }

        public void Restart(bool postUpdate, bool asAdmin = false, IWin32Window? owner = null)
        {
            if (asAdmin)
            {
                DialogResult result = MessageBox.Show(owner, "It is not recommended to run Raid or related programs as Administrator, as it creates unneccesary risk. Are you sure you want to continue?", "Warning", MessageBoxButtons.YesNoCancel);
                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            List<string> args = new() { "--wait", "30000" };
            if (postUpdate)
                args.Add("--post-update");

            ProcessStartInfo psi = new()
            {
                UseShellExecute = asAdmin,
                FileName = AppHost.ExecutableName,
                Verb = asAdmin ? "runAs" : string.Empty,
                Arguments = string.Join(" ", args)
            };
            _ = Process.Start(psi);
            Exit();
        }

        public void Exit()
        {
            _ = StopSignal.TrySetResult();
        }
    }
}
