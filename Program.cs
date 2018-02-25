using System;

namespace CustomHeroCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Custom Hero Creator!");

            Hero hero = new Hero();


            while(hero.IsActive)
            {
                hero.LevelUp();
            }

        }

    }
}
