using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raid.Toolkit.Common;
using Raid.Toolkit.Injection;

namespace Raid.Toolkit
{
    internal static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            int returnValue = 0;
            SingleThreadedSynchronizationContext.Await(async () =>
            {
                returnValue = await RunProgram(args);
                AsyncHookThread.DisposeCurrent();
            });
            return returnValue;
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        static async Task<int> RunProgram(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using IHost host = AppHost.CreateHost();
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
