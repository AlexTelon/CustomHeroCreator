using CustomHeroCreator;
using CustomHeroCreator.AI;
using CustomHeroCreator.CLI;
using CustomHeroCreator.Enteties;
using CustomHeroCreator.Repository;
using System;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace XUnitTests
{
    public class PerformanceTest
    {
        public PerformanceTest()
        {
            DataHub.Instance.ConsoleWrapper = new AutoResponseConsole();
        }
        //[Fact]
        //public void Test1()
        //{
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();
        //    var rnd = new Random();

        //    // Create the arena in which the heroes fitness will be evaluated
        //    Arena arena = new Arena();

        //    // Trials decide what type of trials the heroes will meet (single player survival mode or gladiator arena like stuff)
        //    Trials trials = new Trials();
        //    trials.MaxLevel = 5000;

        //    var evo = new Evolution(rnd, skillTreeGenerator: null);
        //    evo.NrOfHeroesInEachGeneration = 100;
        //    evo.MaxGenerations = 20;
        //    evo.HeroStartingLevel = 10;
        //    evo.AllwaysRunAllGenerations = true;

        //    evo.RunEvolution(trials, arena);

        //    sw.Stop();

        //    // when this test was made it took 13 and 11 sec just as a benchmark.
        //    // make sure we are not getting too slow here!
        //    Assert.False(sw.ElapsedMilliseconds > 20000);
        //}

        [Fact]
        public void LevelUpPerformanceTest()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var hero = new Hero();
            var agent = new Agent();

            hero.AI = agent;

            for (int i = 0; i < 100000; i++)
            {
                hero.LevelUp();
            }

            sw.Stop();

            Assert.False(sw.ElapsedMilliseconds > 250);
        }

        [Fact]
        public void FastAgentLevelUpPerformanceTest()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var hero = new Hero();
            var agent = new FastAgent();

            hero.AI = agent;

            for (int i = 0; i < 100000; i++)
            {
                hero.LevelUp();
            }

            sw.Stop();

            Assert.False(sw.ElapsedMilliseconds > 250);
        }



        [Fact]
        public void FightPerformanceTest()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var hero = new Hero();
            hero.AI = new Agent();

            var enemy = new Hero();
            enemy.AI = new Agent();

            var arena = new Arena();

            for (int i = 0; i < 100000; i++)
            {
                arena.Fight(hero, enemy);
            }

            sw.Stop();

            Assert.False(sw.ElapsedMilliseconds > 10);
        }



    }
}
