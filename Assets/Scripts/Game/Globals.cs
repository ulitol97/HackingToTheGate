﻿using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// The class globals implements the Singleton pattern in order to hold
    /// variables that need tp be available during the whole game session.
    /// </summary>
    public class Globals : Singleton<Globals>
    {
        /// <summary>
        /// Names of the locations found in game. Each name's key is the level they're in.
        /// </summary>
        /// <remarks>Readonly since it shouldn't be modified.</remarks>
        public readonly Dictionary<string, string> LevelNameTable = new Dictionary<string, string>
        {
            {"Level1", "Nayru Dungeon - Basement 2"}, 
            {"Level2", "Nayru Dungeon - Basement 1"}, 
            {"Level3", "Nayru Castle"}, 
            {"Level4", "Nayru Castle - Overworld"}, 
        };
        
        /// <summary>
        /// Tips given by the different dialogs and signs the player may find.
        /// </summary>
        public readonly Dictionary<string, string> TipsTable = new Dictionary<string, string>
        {
            {"sword", "Congratulations, a sword! That opens many doors of opportunity..."}, 
        };
    }
}