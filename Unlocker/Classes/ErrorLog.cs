using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteBurger.Classes
{
    internal class ErrorLog
    {

        internal void CreateLog(string ErrorMessage)
        {
            string AppDir = Environment.CurrentDirectory;

            string specificFolder = AppDir + "/error-log.txt";
            string Time = DateTime.Now.ToString(@"dd\/MM\/yyyy h\:mm tt");

            if (!File.Exists(specificFolder))
                File.Create(specificFolder).Close();

            string Content = File.ReadAllText(specificFolder);

            Content += $"[{Time}] {ErrorMessage}\n";

            File.WriteAllText(specificFolder, Content);
        }
    }
}
