using System;
using System.IO;

namespace Synergi_nflint
{
    class Logging
    {
        private static readonly string FilePathLogs = @".\Logs\Log_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss") + ".json";

        /// <summary>
        /// Method to output log details to file
        /// </summary>
        /// <param name="severity"> Type of Log </param>
        /// <param name="logContent">Log Message</param>
        public static void OutputLog(string severity, string logContent)
        {
            try
            {
                string newestLog = (DateTime.Now + " : " + severity.ToUpper() + " : " + logContent + "\n");
                using (StreamWriter sw = File.AppendText(FilePathLogs))
                {
                    sw.WriteLine(newestLog);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Filed to output logs");
                throw;
            }
        }
    }
}
