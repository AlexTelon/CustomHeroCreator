﻿using CustomHeroCreator.CLI;
using CustomHeroCreator.Enteties;
using CustomHeroCreator.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CustomHeroCreator.Enteties.Hero;

namespace CustomHeroCreator
{
    public class Trials
    {
        public int MaxLevel { get; set; } = 500;
        public double Difficulty { get; internal set; } = 1;

        public Trials()
        {
        }

        /// <summary>
        /// Gauge the Fitness of each hero by letting them fight a series of "NPCs"
        /// </summary>
        /// <param name="list"></param>
        public void RunSinglePlayerTrials(Arena arena, List<Hero> heroes)
        {
            foreach (var hero in heroes)
            {
                RunSinglePlayerTrial(arena, hero);
            }
        }

        /// <summary>
        /// Gauge the Fitness of a hero by letting it fight a series of "NPCs"
        /// </summary>
        /// <param name="list"></param>
        public void RunSinglePlayerTrial(Arena arena, Hero hero)
        {
            var console = DataHub.Instance.ConsoleWrapper;

            var enemy = new DeterministicScrub();
            enemy.StatGain *= Difficulty;

            // create an agent that is fully random (has no intelligent preferences for one stat over another)
            //var agent = new Agent();
            //for (int i = 0; i < agent.Weights.Count(); i++)
            //{
            //    agent.Weights[i] = 1;
            //}
            //enemy.AI = agent;

            var level = 1;
            while (hero.IsAlive && level < MaxLevel)
            {
                //Make sure they are not dead before the fight
                enemy.Restore();
                hero.Restore();

                enemy.LevelUp();
                hero.LevelUp();

                if (hero.IsPlayer)
                {
                    console.Clear();
                    console.WriteLine("Level " + level);
                    console.Write("Enemy has grown stronger: ");
                    enemy.PrintStats(new List<StatTypes>() { StatTypes.MaxHealth, StatTypes.AttackDmg, StatTypes.Armor }, " ");
                    console.WriteLine();
                }

                arena.Fight(hero, enemy);
                level++;
            }

            // The fitness is how many rounds of enemies our hero survived
            hero.Fitness = enemy.Level;
        }

        public void RunTournamentTrial(Arena arena, List<Hero> heroes)
        {
            // Run tournament style trial
            // all vs all
            for (int i = 0; i < heroes.Count(); i++)
            {
                var hero = heroes[i];
                for (int j = i + 1; j < heroes.Count(); j++)
                {
                    var enemy = heroes[j];
                    arena.Fight(hero, enemy);

                    // The heroes Fitness is increased by how much health it has left at the end
                    hero.Fitness += hero.CurrentHealth;
                    enemy.Fitness += enemy.CurrentHealth;

                    //Make sure they are not dead afterwards
                    hero.Restore();
                    enemy.Restore();
                }
            }
        }


        public static void LevelUpHeroes(List<Hero> heroes, int lvl)
        {
            foreach (var hero in heroes)
            {
                LevelUpHero(hero, lvl);
            }
        }

        public static void LevelUpHero(Hero hero, int lvl)
        {
            for (int i = 0; i < lvl; i++)
            {
                if (hero.IsPlayer)
                {
                    DataHub.Instance.ConsoleWrapper.Clear();
                }
                hero.LevelUp();
            }
        }

    }
}
