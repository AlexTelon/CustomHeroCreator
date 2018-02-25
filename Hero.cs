using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHeroCreator
{
    class Hero
    {
        public readonly uint SKILL_OPTIONS_PER_LVL_UP = 3;

        public string Name { get; set; }

        public uint Level { get; private set; } = 1;
        public bool IsActive { get; internal set; } = true;


        // tmp test thing, not to be a string later on
        public string Skills { get; private set; }

        /// <summary>
        /// Level up the user (and give the user the option to choose new skills
        /// </summary>
        public void LevelUp()
        {
            Console.WriteLine("========================");
            PrintStats();
            ChooseNewSkill();
            Console.WriteLine("========================");

            Level++;
        }

        private void ChooseNewSkill()
        {
            Console.WriteLine();
            Console.WriteLine("Choose one of the following or press Q to abort");

            var skillOptions = GenerateRandomSkills();

            var i = 1;
            foreach (var skill in skillOptions)
            {
                Console.WriteLine("[" + i++ + "]: " + skill);
            }


            var input = Console.ReadLine();

            // abort
            if (input == "q" || input == "Q")
            {
                IsActive = false;
            }


            int option = int.Parse(input);

            // option is 1 indexed hence the - 1
            Skills += skillOptions[option - 1] + " ";

            Console.WriteLine();
        }

        private List<string> GenerateRandomSkills()
        {
            var skills = new List<string>();

            Random rnd = new Random();

            for (int i = 0; i < SKILL_OPTIONS_PER_LVL_UP; i++)
            {
                skills.Add("Strenght " + rnd.Next(1, 5));
            }

            return skills;
        }

        public void PrintStats()
        {
            Console.WriteLine();
            Console.WriteLine(this.ToString());
            Console.WriteLine();
        }

        public override string ToString()
        {
            return "Name: " + Name + " Level: " + Level + " Skills: " + Skills;
        }

    }
}
