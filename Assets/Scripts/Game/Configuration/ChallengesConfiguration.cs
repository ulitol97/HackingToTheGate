using System.Collections.Generic;
using UnityEngine;

namespace Game.Configuration
{
    /// <summary>
    /// The ChallengesConfiguration contains the AnswersConfiguration and CluesConfiguration. Instances of these
    /// classes contain teh tips the player can read in-game and the solutions to text based puzzles. These
    /// instances are be generated from external JSON files so that they can be changed each game session.
    /// </summary>
    [System.Serializable]
    public class ChallengesConfiguration
    {

        public AnswersConfiguration answers;
        public CluesConfiguration clues;

        public List<string> GetAnswers()
        {
            return answers.answers;
        }
        
        public List<string> GetClues()
        {
            return clues.clues;
        }
        
        
        /// <summary>
        /// Creates an instance of the AnswersConfiguration class from a JSON file with the correct format.
        /// </summary>
        /// <param name="jsonFilePath">Path where the json file is located.</param>
        /// <returns></returns>
        public static AnswersConfiguration RetrieveAnswersFromJson(string jsonFilePath)
        {
            return JsonUtility.FromJson<AnswersConfiguration>(jsonFilePath);
        }
        
        /// <summary>
        /// Creates an instance of the CluesConfiguration class from a JSON file with the correct format.
        /// </summary>
        /// <param name="jsonFilePath">Path where the json file is located.</param>
        /// <returns></returns>
        public static CluesConfiguration RetrieveCluesFromJson(string jsonFilePath)
        {
            return JsonUtility.FromJson<CluesConfiguration>(jsonFilePath);
        }
        
        [System.Serializable]
        public class AnswersConfiguration
        {
            public List<string> answers;
        }
        
        [System.Serializable]
        public class CluesConfiguration
        {
            public List<string> clues;
        }
    }
}
