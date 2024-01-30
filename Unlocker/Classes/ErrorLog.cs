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

            using (var fs = File.Open(specificFolder, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.SetLength(0);
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(Time + ": " + ErrorMessage);
                }
            }
        }
    }
}
