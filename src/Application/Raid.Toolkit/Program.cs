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
            try
            {
                return RunProgram(args);
            }
            finally
            {
                AsyncHookThread.DisposeCurrent();
            }
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        static int RunProgram(string[] args)
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
                host.StartAsync().Wait();
            }
            try
            {
                return host.Services.GetRequiredService<ApplicationStartupTask>().Execute();
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
                    host.StopAsync().Wait();
                }
            }
        }
    }
}
