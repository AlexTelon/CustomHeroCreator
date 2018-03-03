using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHeroCreator
{
    class Arena
    {

        public static void Fight(Hero first, Hero second)
        {
            //double time = 0;
            while (first.IsAlive && second.IsAlive)
            {
                // to begin with they always attack at the same time
                first.Attack(second);
                second.Attack(first);
            }
        }
    }
}
