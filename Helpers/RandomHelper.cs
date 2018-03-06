using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHeroCreator.Helpers
{
    class RandomHelper
    {

        // Use external random object to save allocation and make sure to actually have good randomness
        // I think it uses the time as a seed so in a quick loop several random objects can get the same seed.
        internal static bool RandomBool(Random rnd)
        {
            //https://stackoverflow.com/questions/19191058/fastest-way-to-generate-a-random-boolean
            return rnd.NextDouble() >= 0.5;
        }
    }
}
