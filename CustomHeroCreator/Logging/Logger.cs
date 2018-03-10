using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CustomHeroCreator.Logging
{
    public class Logger
    {
        private static readonly Logger instance = new Logger();

        public static Logger Instance => instance;

        static Logger()
        {
        }

        private Logger()
        {
            //hardcode to project folder for now (otherwise this ends up in bin folder).
            LoggerFile = "../../../LogFile.json";

            // clear the contents of the log file
            System.IO.File.WriteAllText(LoggerFile, string.Empty);
        }

        public string LoggerFile { get; set; }

        /// <summary>
        /// Log stuff
        /// </summary>
        /// <param name="stuff"></param>
        private void Log(StreamWriter sw, string stuff)
        {
            sw.WriteLine(stuff);
        }

        internal void Log(object obj)
        {
            using (StreamWriter sw = new StreamWriter(LoggerFile, true))
            {
                var objectAsJSON = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                sw.WriteLine(objectAsJSON);
            }
        }

        //internal void Log(List<Hero> heroes)
        //{
        //    using (StreamWriter sw = new StreamWriter(LoggerFile, true))
        //    {
        //        foreach (var hero in heroes)
        //        {
        //            var objectAsJSON = Newtonsoft.Json.JsonConvert.SerializeObject(hero);
        //            sw.WriteLine(objectAsJSON);
        //        }
        //    }
        //}
    }
}
