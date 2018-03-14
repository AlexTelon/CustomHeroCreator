using System;

namespace CustomHeroCreator.CLI
{
    public interface IConsole
    {
        ConsoleColor ForegroundColor { get; set; }

        void Write(string message = "");
        void WriteLine(string message = "");
        void Clear();
        string ReadLine();
    }
}
