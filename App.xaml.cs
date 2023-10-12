using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FortniteBurger
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Current.DispatcherUnhandledException += DispatcherOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            SessionEnding += SessionEndingShutdown;
        }

        private void Application_Exit(object sender, EventArgs e)
        {
            Classes.FiddlerCore.StopFiddlerCore();
            Classes.Settings.SaveConfig();
            Classes.Settings.SaveSettings();
            Classes.Settings.SaveMods();

            if (Overlay.timer != null)
            {
                Overlay.StopTimer();
            }
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            Classes.FiddlerCore.StopFiddlerCore();
            Classes.Settings.SaveConfig();
            Classes.Settings.SaveSettings();
            Classes.Settings.SaveMods();


            if (Overlay.timer != null)
            {
                Overlay.StopTimer();
            }
        }

        private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            Classes.FiddlerCore.StopFiddlerCore();
            Classes.Settings.SaveConfig();
            Classes.Settings.SaveSettings();
            Classes.Settings.SaveMods();

            if(Overlay.timer != null)
            {
                Overlay.StopTimer();
            }
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Classes.FiddlerCore.StopFiddlerCore();
            Classes.Settings.SaveConfig();
            Classes.Settings.SaveSettings();
            Classes.Settings.SaveMods();


            if (Overlay.timer != null)
            {
                Overlay.StopTimer();
            }
        }

        private void SessionEndingShutdown(object sender, SessionEndingCancelEventArgs unhandledExceptionEventArgs)
        {
            Classes.FiddlerCore.StopFiddlerCore();
            Classes.Settings.SaveConfig();
            Classes.Settings.SaveSettings();
            Classes.Settings.SaveMods();


            if (Overlay.timer != null)
            {
                Overlay.StopTimer();
            }
        }
    }
}
