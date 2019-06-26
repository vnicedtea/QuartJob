using System;
using System.Diagnostics;
using System.IO;
using System.Text;
namespace QuartzJob
{
    public sealed class TimeWatch
    {
        public static Stopwatch stopwatch;

        TimeWatch() { stopwatch = new Stopwatch(); }
        private static readonly Lazy<TimeWatch> lazy = new Lazy<TimeWatch>(() => new TimeWatch());
        public static TimeWatch Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        public string path { get; set; }
        public string msg { get; set; }
        public static void start()
        {
            stopwatch.Reset();
            stopwatch.Start();
        }
        public static void end(string InPmsg)
        {
            stopwatch.Stop();
            TimeWatch.Instance.msg = InPmsg;
            WatchTime();
        }

        public void WriteText(string message)
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
                    WriteText(message);
                }
            }
            else
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
                {
                    file.WriteLine("\r\n");
                    file.WriteLine(DateTime.Now.ToString("G") + ": " + message);
                }
            }
        }

        public static void WatchTime()
        {
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            TimeWatch.Instance.WriteText(elapsedTime);
        }


    }
}
