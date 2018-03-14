using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHeroCreator.GameModes
{
    public interface IGame
    {
        void Init();

        void Start();

        void End();
    }
}
