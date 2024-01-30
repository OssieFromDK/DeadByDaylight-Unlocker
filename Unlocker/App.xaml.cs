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

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Classes.CloseManager.Close();
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {

        }

        private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            Classes.CloseManager.Close(true, dispatcherUnhandledExceptionEventArgs.Exception.Message);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Exception exception = (Exception)unhandledExceptionEventArgs.ExceptionObject;
            var message = exception.Message;

            Classes.CloseManager.Close(true, message);
        }

        private void SessionEndingShutdown(object sender, SessionEndingCancelEventArgs unhandledExceptionEventArgs)
        {
            Classes.CloseManager.Close();
        }
    }
}
