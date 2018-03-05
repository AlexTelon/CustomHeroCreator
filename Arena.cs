﻿using CustomHeroCreator.CLI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CustomHeroCreator
{
    class Arena
    {

        public static void Fight(Hero first, Hero second)
        {
            var playerPresent = (first.IsPlayer || second.IsPlayer);

            if (playerPresent)
            {
                if (first.IsPlayer)
                {
                    PlayerFight(first, second);
                }
                else
                {
                    PlayerFight(second, first);
                }
            }

            //double time = 0;
            while (first.IsAlive && second.IsAlive)
            {
                // to begin with they always attack at the same time
                first.Attack(second);
                second.Attack(first);
            }
        }

        private static void PlayerFight(Hero player, Hero enemy)
        {
            var sleep = 100;

            Console.WriteLine("Enemy stats: ");
            enemy.PrintStats();
            Console.WriteLine();

            var playerPreviousHealth = player.CurrentHealth;
            var enemyPreviousHealth = enemy.CurrentHealth;

            //double time = 0;
            while (player.IsAlive && enemy.IsAlive)
            {
                // more print stuff should be here not in hero
                //CommandLineTools.PrintHealthBars(player, enemy);

                var MaxHealth = Math.Max(player.MaxHealth, enemy.MaxHealth);

                var playerDmgTaken = playerPreviousHealth - player.CurrentHealth;
                var enemyDmgTaken = enemyPreviousHealth - enemy.CurrentHealth;


                Console.Write("Your: ");
                CommandLineTools.PrintWithColor("" + player.CurrentHealth.ToString("#.#") + "\t\t", ConsoleColor.DarkGreen);
                CommandLineTools.PrintVerticalBar(player.CurrentHealth, 0, MaxHealth, ConsoleColor.Green);
                if (playerDmgTaken != 0)
                {
                    CommandLineTools.PrintWithColor("\t\t-" + playerDmgTaken.ToString("#.#"), ConsoleColor.Red);
                }

                Console.Write("\t\t\tEnemy: ");
                CommandLineTools.PrintWithColor("" + enemy.CurrentHealth.ToString("#.#") + "\t\t", ConsoleColor.DarkGreen);
                CommandLineTools.PrintVerticalBar(enemy.CurrentHealth, 0, MaxHealth, ConsoleColor.Green);
                if (enemyDmgTaken != 0)
                {
                    CommandLineTools.PrintWithColor("\t\t-" + enemyDmgTaken.ToString("#.#"), ConsoleColor.Red);
                }

                Console.WriteLine();

                playerPreviousHealth = player.CurrentHealth;
                enemyPreviousHealth = enemy.CurrentHealth;


                // to begin with they always attack at the same time
                Thread.Sleep(sleep);
                player.Attack(enemy);
                Thread.Sleep(sleep);
                enemy.Attack(player);
            }
        }


    }
}
