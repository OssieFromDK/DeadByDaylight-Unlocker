using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;

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

            string currentExePath = Process.GetCurrentProcess().MainModule.FileName;
            string directory = Path.GetDirectoryName(currentExePath);
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string flagDir = AppData + "/FortniteBurger/Flags";

            if (!Directory.Exists(flagDir))
                Directory.CreateDirectory(flagDir);

            // Assume currentExePath holds the full path of the currently running executable,
            // and directory/flagDir are defined appropriately.
            string[] args = e.Args;


            if (args.Length >= 2 && args[0] == "--delete-old")
            {
                string oldExePath = args[1];
                string FileName = Path.GetFileNameWithoutExtension(oldExePath);
                string NetTemp = AppData + "/Temp/.net/" + FileName;

                System.Threading.Thread.Sleep(2000);

                if (File.Exists(oldExePath))
                    File.Delete(oldExePath);

                if (Directory.Exists(NetTemp))
                    Directory.Delete(NetTemp, true);
            }

            string renameFlag = Path.Combine(flagDir, "renamed.flag");

            if (!File.Exists(renameFlag))
            {
                // Generate a random executable name.
                string randomName = Path.GetRandomFileName().Replace(".", "") + ".exe";
                string newExePath = Path.Combine(directory, randomName);

                // Store the new name in the registry.
                RegistryKey mainKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\FortniteBurger");
                using (RegistryKey key = mainKey.CreateSubKey("Info"))
                {
                    if (key != null)
                    {
                        key.SetValue("Name", randomName);
                    }
                }

                // Copy the current executable to the new location.
                File.Copy(currentExePath, newExePath);

                // Create the flag file to indicate the renaming/copying has been done.
                File.WriteAllText(renameFlag, "renamed");

                AddRandomData(newExePath);

                // Start the new executable, passing the current executable path as an argument for deletion.
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = newExePath,
                    Arguments = $"--delete-old \"{currentExePath}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                Process.Start(psi);

                // Exit the current process.
                Environment.Exit(0);
            }
        }

        private static void AddRandomData(string filePath)
        {
            int dataLength = RandomNumberGenerator.GetInt32(16, 513);

            byte[] randomData = new byte[dataLength];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomData);
            }

            using (var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            {
                stream.Write(randomData, 0, randomData.Length);
            }
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
