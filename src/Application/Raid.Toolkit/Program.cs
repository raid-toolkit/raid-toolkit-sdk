using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Raid.Toolkit
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            return RunProgram(args);
        }

        // for logging
        private class _Program { }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        private static int RunProgram(string[] args)
        {
            _ = Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using IHost host = AppHost.CreateHost();
            ILogger logger = host.Services.GetRequiredService<ILogger<_Program>>();
            ApplicationStartupTask applicationStartupTask = host.Services.GetRequiredService<ApplicationStartupTask>();
            ApplicationStartupCondition startCondition = applicationStartupTask.Parse(args);

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
                return applicationStartupTask.Execute();
            }
            catch (Exception e)
            {
                string errorMessage = "A fatal error occurred";
                logger.LogError(e, errorMessage);
                if ((args.Contains("--debug") || args.Contains("-d")) && !args.Contains("--no-ui") && !args.Contains("-n"))
                {
                    errorMessage += $":\n\n{e.Message}\n{e.StackTrace}";
                }
                _ = MessageBox.Show(new Form(), errorMessage, "Raid Toolkit", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
