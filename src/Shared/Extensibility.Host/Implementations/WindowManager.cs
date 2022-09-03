using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raid.Toolkit.Extensibility.DataServices;
using Window = System.Windows.Window;

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
            public WindowState(Window window, bool isVisible)
            {
                Visible = isVisible;
                Location = new((int)window.Left, (int)window.Top);
                Size = new((int)window.Width, (int)window.Height);
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

        public bool CanShowUI { get; set; } = true;

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
                    object visual = CreateWindow(type);
                    if (visual is Form form)
                        form.Show();
                    else if(visual is Window window)
                        window.Show();
                }
            }
        }

        public T CreateWindow<T>() where T : class, IDisposable
        {
            return CreateWindow(typeof(T)) as T;
        }

        public object CreateWindow(Type type)
        {
            Logger.LogInformation($"Creating window {type.FullName}");
            if (!Options.TryGetValue(type, out WindowOptions options))
                throw new InvalidOperationException($"Type '{type.FullName}' is not registered.");

            object visual = options.CreateInstance != null
                ? options.CreateInstance()
                : ActivatorUtilities.CreateInstance(ServiceProvider, type);
            if (visual is Form form)
                AttachEvents(options, form);
            else if (visual is Window window)
                AttachEvents(options, window);


            return visual;
        }

        private void AttachEvents(WindowOptions options, Window window)
        {
            ElementHost.EnableModelessKeyboardInterop(window);
            if (options.RememberVisibility)
            { 
                window.Closing += Window_Closing;
                window.IsVisibleChanged += (_, _) => Window_PropertyChanged(options, window);
            }
            if (options.RememberPosition)
            {
                window.LocationChanged += (_, _) => Window_PropertyChanged(options, window);
                window.SizeChanged += (_, _) => Window_PropertyChanged(options, window);
            }
        }

        private void Window_PropertyChanged(WindowOptions options, Window sender)
        {
            string senderType = sender.GetType().FullName;
            Logger.LogInformation($"Updating window {senderType} (visibility={sender.Visibility})");
            States[senderType] = new(sender, sender.Visibility == System.Windows.Visibility.Visible);
            _ = Storage.Write(AppStateDataContext.Default, "windows", States);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool isUserClose = true; // TODO: determine this for wpf, consider https://stackoverflow.com/a/13855608/8199359
            string senderType = sender.GetType().FullName;
            Logger.LogInformation($"Updating window {senderType} (closed)");
            // if closing for application exit/shutdown, then consider it still open so it will re-open on next launch
            States[senderType] = new(sender as Window, !isUserClose);
            _ = Storage.Write(AppStateDataContext.Default, "windows", States);
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
            _ = Storage.Write(AppStateDataContext.Default, "windows", States);
        }

        private void SetFormClosedState(object sender, FormClosingEventArgs e)
        {
            bool isUserClose = e.CloseReason is CloseReason.UserClosing
                or CloseReason.FormOwnerClosing;

            string senderType = sender.GetType().FullName;
            Logger.LogInformation($"Updating window {senderType} (closed)");
            // if closing for application exit/shutdown, then consider it still open so it will re-open on next launch
            States[senderType] = new(sender as Form, !isUserClose);
            _ = Storage.Write(AppStateDataContext.Default, "windows", States);
        }

        public void RegisterWindow<T>(WindowOptions options) where T : class, IDisposable
        {
            Logger.LogInformation($"Registered window {typeof(T).FullName}");
            Options.Add(typeof(T), options);
        }

        public void UnregisterWindow<T>() where T : class, IDisposable
        {
            Logger.LogInformation($"Unregistered window {typeof(T).FullName}");
            _ = Options.Remove(typeof(T));
        }
    }
}
