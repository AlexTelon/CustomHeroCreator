namespace CustomHeroCreator.CLI
{
    public interface IConsole
    {
        void Write(string message = "");
        void WriteLine(string message = "");
        void Clear();
        string ReadLine();
    }
}
