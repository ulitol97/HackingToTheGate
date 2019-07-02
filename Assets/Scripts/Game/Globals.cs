using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// The class globals implements the Singleton pattern in order to hold
    /// variables that need tp be available during the whole game session.
    /// </summary>
    public class Globals : Singleton<Globals>
    {
        public readonly Dictionary<string, string> LevelNameTable = new Dictionary<string, string>
        {
            {"Level1", "Nayru Dungeon - Basement 2"}, 
            {"Level2", "Nayru Dungeon - Basement 1"}, 
            {"Level3", "Nayru Castle"}, 
            {"Level4", "Nayru Castle - Overworld"}, 
        };
    }
}
