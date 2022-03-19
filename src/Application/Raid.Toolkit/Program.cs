using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
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
        static async Task<int> Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (IHost host = AppHost.CreateHost())
            {
                ApplicationStartupCondition startCondition = host.Services.GetRequiredService<ApplicationStartupTask>().Parse(args);

                if (startCondition.HasFlag(ApplicationStartupCondition.Usage))
                {
                    return 255;
                }

                if (startCondition.HasFlag(ApplicationStartupCondition.Services))
                {
                    await host.StartAsync();
                }
                try
                {
                    return await host.Services.GetRequiredService<ApplicationStartupTask>().Execute();
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                    return 1;
                }
                finally
                {
                    if (startCondition.HasFlag(ApplicationStartupCondition.Services))
                    {
                        await host.StopAsync();
                    }
                }
            }
        }
    }
}
