using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raid.Toolkit
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (IHost host = AppHost.CreateHost())
            {
                await host.StartAsync();
                await host.Services.GetRequiredService<ApplicationStartupTask>().Execute(args);
                await host.StopAsync();
            }
        }
    }
}
