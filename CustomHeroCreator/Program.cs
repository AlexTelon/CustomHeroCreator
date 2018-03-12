using CustomHeroCreator.AI;
using CustomHeroCreator.CLI;
using CustomHeroCreator.Fighters;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using CustomHeroCreator.Logging;
using static CustomHeroCreator.Hero;
using CustomHeroCreator.Helpers;

using System.Diagnostics;
using System.Threading;
using CustomHeroCreator.Generators;
using CustomHeroCreator.GameModes;

namespace CustomHeroCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Always use . instead of , as a comma.
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Console.SetWindowSize(Console.WindowWidth, Console.LargestWindowHeight / 2);

#if DEBUG
            var game = new AIGame();
#else
            var game = new PlayerGame();
#endif

            game.Init();
            game.Start();
            game.End();
        }
    }
}
