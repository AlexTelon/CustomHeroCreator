using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHeroCreator
{
    class AI
    {

        internal string ChooseOption(object hero, Dictionary<Hero.StatTypes, int> options)
        {
            // get internal state of the hero, use that later


            // see what options we have

            // randomize the choise (for now) on how we choose an option
            var rnd = new Random();

            // 1 to count + 1 since thats the way the user chooses (1 indexed)
            return "" + rnd.Next(1, options.Count + 1);

        }
    }
}
