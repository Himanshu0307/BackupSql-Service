using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Backup
{
    public class LogService
    {
        String filename = "Backup.log";

        public LogService()
        {
            if (!File.Exists(filename))
                File.Create(filename);

        }
        public LogService(string BaseDir)
        {
            this.filename = $"{BaseDir}{filename}";
            if (!File.Exists(filename))
                File.Create(filename);

        }

        public void LogInformation(String s)
        {
            using (StreamWriter s1 = new StreamWriter(filename, append: true))
            {
                s1.WriteLine("Info:" + DateTime.Now + ":" + s);
            }
        }
        public void LogError(String s)
        {
            using (StreamWriter s1 = new StreamWriter(filename, append: true))
            {
                s1.WriteLine("Error:" + DateTime.Now + ":" + s);
            }
        }
    }
}