using CustomHeroCreator.CLI;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHeroCreator.Repository
{
    /// <summary>
    /// Dont really know if this is a repositry pattern
    /// </summary>
    public sealed class DataHub
    {
        private static readonly DataHub instance = new DataHub();
        public static DataHub Instance => instance;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static DataHub()
        {
        }

        /// <summary>
        /// A source for randomness for everyone
        /// </summary>
        public Random RandomSource { get; set; } = new Random();

        /// <summary>
        /// A common console wrapper for everyone to use
        /// </summary>
        public IConsole ConsoleWrapper { get; set; } = new PlayerConsole();


        private DataHub()
        {
        }


    }
}
