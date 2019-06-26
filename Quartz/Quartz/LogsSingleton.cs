using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzJob
{
    public sealed class LogsSingleton
    {
        private LogsSingleton() { }
        private static readonly Lazy<LogsSingleton> lazy = new Lazy<LogsSingleton>(() => new LogsSingleton());
        public static LogsSingleton Instance
        {
            get { return lazy.Value; }
        }
        public string path { get; set; }
        public static void WriteText(string message, string JobName)
        {
            if (path == null)
            {
                path = @"C:\Einvoicelogs\TimeWatch" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            }
            else if (!path.ToLower().Contains(".txt"))
            {
                path = path + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            }

            if (!File.Exists(path))
            {
                using (FileStream fs = File.Create(path))
                {
                    // Add some text to file    
                    Byte[] title = new UTF8Encoding(true).GetBytes(DateTime.Now.ToString("G") + " -----------------Log start----------------");
                    fs.Write(title, 0, title.Length);
                    fs.Close();
                    WriteText(message,JobName);
                }
            }
            else
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
                {
                    file.WriteLine("\r\n");
                    file.WriteLine(DateTime.Now.ToString("G") + " " + JobName + " :" + message);
                }
            }
        }


    }
}
