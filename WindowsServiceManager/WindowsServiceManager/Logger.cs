using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServiceManager
{
    public class Logger
    {
        public enum LoggingLevel
        {
            Critical,
            Error,
            Warning,
            Information
        }

        private readonly string path;
        private LoggingLevel loggingLevel = LoggingLevel.Information;

        /// <summary>
        /// Constructor for <see cref="Logger"/> class which will initialize the logging file and will set the default logging 
        /// level to <see cref="LoggingLevel.Information"/>.
        /// </summary>
        /// <param name="directory">The directory for logging file.</param>
        /// <param name="name">The file name for logging file.</param>
        /// <exception cref="DirectoryNotFoundException">Throw if the directory is not valid.</exception>
        public Logger(string directory, string name)
        {
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException();

            path = Path.Combine(directory, name);
            if (!File.Exists(path))
                File.Create(path).Close();
        }

        public void WriteLog(string message)
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                message = $"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()} {"|"} { Enum.GetName(typeof(LoggingLevel), loggingLevel)} {":"}{message}";
                sw.WriteLine(message);
            }
        }

        public Logger SetLogLevel(LoggingLevel level)
        {
            loggingLevel = level;
            return this;
        }
    }
}
