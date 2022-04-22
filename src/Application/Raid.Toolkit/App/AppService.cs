using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Extensions.Hosting;
using Raid.Toolkit.Extensibility.Host.Services;

namespace Raid.Toolkit
{
    internal class AppService
    {
        private readonly IHostApplicationLifetime Lifetime;
        private readonly UpdateService UpdateService;

        public AppService(IHostApplicationLifetime lifetime, UpdateService updateService)
        {
            Lifetime = lifetime;
            UpdateService = updateService;
        }

        public async void InstallUpdate(GitHub.Schema.Release release)
        {
            try
            {
                await UpdateService.InstallRelease(release, AppHost.ExecutableName);
                Restart(postUpdate: true);
            }
            catch { }
        }

        public void Restart(bool postUpdate, bool asAdmin = false)
        {
            List<string> args = new() { "--wait", "30000" };
            if (postUpdate)
                args.Add("--post-update");

            ProcessStartInfo psi = new()
            {
                UseShellExecute = true,
                FileName = AppHost.ExecutableName,
                Verb = asAdmin ? "runAs" : null,
                Arguments = string.Join(" ", args)
            };
            _ = Process.Start(psi);
            Exit();
        }

        public void Exit()
        {
            Application.Exit();
            Lifetime.StopApplication();
        }
    }
}
