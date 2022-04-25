using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Extensibility.DataServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Raid.Toolkit.Extensibility.Host
{
    public class WindowManager : IWindowManager
    {
        public class WindowState
        {
            public WindowState()
            { }
            public WindowState(Form form, bool isVisible)
            {
                Visible = isVisible;
                Location = form.Location;
                Size = form.Size;
            }
            public bool Visible { get; set; }
            public Point Location;
            public Size Size;
        }

        private readonly Dictionary<Type, WindowOptions> Options = new();
        private readonly Dictionary<string, WindowState> States;
        private readonly IServiceProvider ServiceProvider;
        private readonly PersistedDataStorage Storage;
        private readonly ILogger Logger;

        public WindowManager(
            IServiceProvider serviceProvider,
            PersistedDataStorage storage,
            ILogger<WindowManager> logger
            )
        {
            ServiceProvider = serviceProvider;
            Storage = storage;
            Logger = logger;
            if (!Storage.TryRead(AppStateDataContext.Default, "windows", out States))
                States = new();
        }

        public void RestoreWindows()
        {
            foreach (var (type, options) in Options)
            {
                if (options.RememberVisibility
                    && States.TryGetValue(type.FullName, out WindowState state)
                    && state.Visible)
                {
                    Form window = CreateWindow(type);
                    window.Show();
                }
            }
        }

        public T CreateWindow<T>() where T : Form
        {
            return CreateWindow(typeof(T)) as T;
        }

        public Form CreateWindow(Type type)
        {
            Logger.LogInformation($"Creating window {type.FullName}");
            if (!Options.TryGetValue(type, out WindowOptions options))
                throw new InvalidOperationException($"Type '{type.FullName}' is not registered.");

            Form form = options.CreateInstance != null
                ? options.CreateInstance()
                : ActivatorUtilities.CreateInstance(ServiceProvider, type) as Form;

            AttachEvents(options, form);
            return form;
        }

        private void AttachEvents(WindowOptions options, Form form)
        {
            if (options.RememberVisibility)
            {
                form.FormClosing += SetFormClosedState;
                form.Shown += SetFormVisibleState;
            }
            if (options.RememberPosition)
            {
                if (States.TryGetValue(form.GetType().FullName, out WindowState state))
                {
                    form.Location = state.Location;
                    form.StartPosition = FormStartPosition.Manual;
                }

                form.ResizeEnd += SetFormVisibleState;
                form.Move += SetFormVisibleState;
            }
        }

        private void SetFormVisibleState(object sender, EventArgs e)
        {
            string senderType = sender.GetType().FullName;
            Logger.LogInformation($"Updating window {senderType} (open)");
            States[senderType] = new(sender as Form, true);
            Storage.Write(AppStateDataContext.Default, "windows", States);
        }

        private void SetFormClosedState(object sender, FormClosingEventArgs e)
        {
            bool isUserClose = e.CloseReason == CloseReason.UserClosing
                || e.CloseReason == CloseReason.FormOwnerClosing;

            string senderType = sender.GetType().FullName;
            Logger.LogInformation($"Updating window {senderType} (closed)");
            // if closing for application exit/shutdown, then consider it still open so it will re-open on next launch
            States[senderType] = new(sender as Form, !isUserClose);
            Storage.Write(AppStateDataContext.Default, "windows", States);
        }

        public void RegisterWindow<T>(WindowOptions options) where T : Form
        {
            Logger.LogInformation($"Registered window {typeof(T).FullName}");
            Options.Add(typeof(T), options);
        }

        public void UnregisterWindow<T>() where T : Form
        {
            Logger.LogInformation($"Unregistered window {typeof(T).FullName}");
            Options.Remove(typeof(T));
        }
    }
}
