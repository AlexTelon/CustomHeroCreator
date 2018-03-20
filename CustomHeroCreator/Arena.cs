using CustomHeroCreator.CLI;
using CustomHeroCreator.Enteties;
using CustomHeroCreator.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CustomHeroCreator
{
    public class Arena
    {
        public void Fight(Hero first, Hero second)
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
            var console = DataHub.Instance.ConsoleWrapper;
            var sleep = 100;

            console.WriteLine("Enemy stats: ");
            enemy.PrintStats();
            console.WriteLine();

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


                console.Write("Your: ");
                CommandLineTools.PrintWithColor("" + player.CurrentHealth.ToString("#.#") + "\t", ConsoleColor.DarkGreen);
                CommandLineTools.PrintVerticalBar(player.CurrentHealth, 0, MaxHealth, ConsoleColor.Green);
                console.Write("\t");
                if (playerDmgTaken != 0)
                {
                    CommandLineTools.PrintWithColor("-" + playerDmgTaken.ToString("#.#"), ConsoleColor.Red);
                }

                console.Write("\tEnemy: ");
                CommandLineTools.PrintWithColor("" + enemy.CurrentHealth.ToString("#.#") + "\t", ConsoleColor.DarkGreen);
                CommandLineTools.PrintVerticalBar(enemy.CurrentHealth, 0, MaxHealth, ConsoleColor.Green);
                console.Write("\t");
                if (enemyDmgTaken != 0)
                {
                    CommandLineTools.PrintWithColor("-" + enemyDmgTaken.ToString("#.#"), ConsoleColor.Red);
                }

                console.WriteLine();

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
