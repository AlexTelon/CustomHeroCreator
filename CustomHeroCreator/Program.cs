using System;
using System.Threading;
using CustomHeroCreator.CLI;
using CustomHeroCreator.GameModes;

namespace CustomHeroCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Always use . instead of , as a comma.
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

#if DEBUG
            // Running SetWindowsSize below in a linux docker containter gives a plattform not supported exception
            Console.SetWindowSize(Console.WindowWidth, Console.LargestWindowHeight / 2);

            var autoResponseConsole = new AutoResponseConsole();
            autoResponseConsole.AutomatedResponses.Add("1");

            var playerConsole = new PlayerConsole();

            var game = new AIGame(playerConsole);
#else
            var playerConsole = new PlayerConsole();
            var game = new PlayerGame(playerConsole);
#endif

            game.Init();
            game.Start();
            game.End();
        }
    }
}
