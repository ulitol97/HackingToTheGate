using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Game.Configuration.Validation;
using UnityEngine;

namespace Game.Configuration
{
    /// <summary>
    /// The class GameConfigurationManager implements the Singleton pattern in order to hold
    /// variables that need to be available during the whole game session. It is capable of accessing a
    /// game configuration file and validate it.
    /// </summary>
    public class GameConfigurationManager : Singleton<GameConfigurationManager>
    {
        /// <summary>
        /// Name of the file containing the configuration parameters of the connection to the remote host.
        /// </summary>
        private const string ConnectionConfigFileName = "Config/config.json";
        
        /// <summary>
        /// Name of the file containing the answers to password puzzles.
        /// </summary>
        private const string AnswersConfigFileName = "Config/answers.json";
        
        /// <summary>
        /// Name of the file containing the clues given to players to solve puzzles.
        /// </summary>
        private const string CluesConfigFileName = "Config/clues.json";
        
        public Regex ipValidation = new Regex(
            @"\b(?:(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])\.){3}(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])\b");

        /// <summary>
        /// The minimum value that the ports used for communication specified in the configuration file can take.
        /// </summary>
        private const int MinPortNumberAccepted = 1;
        /// <summary>
        /// The maximum value that the ports used for communication specified in the configuration file can take.
        /// </summary>
        private const int MaxPortNumberAccepted = 65535;

        /// <summary>
        /// Boolean flag representing if the game current configuration is valid to start a new game or not.
        /// </summary>
        public static bool IsValid;
        
        /// <summary>
        /// Reference to a ConnectionConfiguration object holding all the variables specified in the user configuration file.
        /// </summary>
        public static ConnectionConfiguration ConnectionConfig;
        public static ChallengesConfiguration ChallengesConfig;

        /// <summary>
        /// Checks for the existence of a game configuration file and if it exists proceeds to validate it.
        /// If the file does not exist or can't be read because of incorrect formatting, the configuration in the file
        /// is considered invalid (<see cref="IsValid"/>).
        /// </summary>
        public void LoadGameConfiguration()
        {
            string connectionPath = Path.Combine(Application.streamingAssetsPath, ConnectionConfigFileName);
            string cluesPath = Path.Combine(Application.streamingAssetsPath, CluesConfigFileName);
            string answersPath = Path.Combine(Application.streamingAssetsPath, AnswersConfigFileName);
            if (File.Exists(connectionPath) && File.Exists(cluesPath) && File.Exists(answersPath))
            {
                try
                {
                    ConnectionConfig = ConnectionConfiguration.CreateFromJson(File.ReadAllText(connectionPath));
                    ChallengesConfig = new ChallengesConfiguration
                    {
                        answers = ChallengesConfiguration.RetrieveAnswersFromJson(File.ReadAllText(answersPath)),
                        clues = ChallengesConfiguration.RetrieveCluesFromJson(File.ReadAllText(cluesPath))
                    };
                }
                catch (Exception)
                {
                    IsValid = false;
                    return;
                }
                ValidateConnectionSettings();
            }
            else
            {
                IsValid = false;
                ConnectionConfig = null;
            }
        }

        /// <summary>
        /// Checks that, after validating the users config file, at least a valid remote host IP and a valid
        /// username to attempt authentication are specified.
        /// </summary>
        private void ValidateConnectionSettings()
        {
            ValidateConnectionFields();
            
            // Final checks
            if (ConnectionConfig.vncConnectionInfo.targetHost.Equals(TextValidator.DefaultValue))
            {
                IsValid = false;
                return;
            }
            if (ConnectionConfig.sshConnectionInfo.username.Equals(TextValidator.DefaultValue))
            {
                IsValid = false;
                return;
            }

            IsValid = true;
        }

        /// <summary>
        /// Validates all the items present in the game configuration file, setting them to a default value if
        /// possible when the user input values are not valid.
        /// </summary>
        private void ValidateConnectionFields()
        {
            TextValidator ipValidator = new TextValidator(ipValidation);
            TextValidator textValidator = new TextValidator();
            
            IntegerValidator sshPortValidator = 
                new IntegerValidator(MinPortNumberAccepted, MaxPortNumberAccepted, 22);
            IntegerValidator vncPortValidator = 
                new IntegerValidator(MinPortNumberAccepted, MaxPortNumberAccepted, 5900);
            
            // Vnc info validation
            ConnectionConfig.vncConnectionInfo.targetHost = 
                ipValidator.Validate(ConnectionConfig.vncConnectionInfo.targetHost);
            
            ConnectionConfig.vncConnectionInfo.port =
                vncPortValidator.Validate(ConnectionConfig.vncConnectionInfo.port);
            
            ConnectionConfig.vncConnectionInfo.vncServerPassword =
                textValidator.Validate(ConnectionConfig.vncConnectionInfo.vncServerPassword);
            
            // Ssh info validation
            
            ConnectionConfig.sshConnectionInfo.username = 
                textValidator.Validate(ConnectionConfig.sshConnectionInfo.username);
            ConnectionConfig.sshConnectionInfo.password = 
                textValidator.Validate(ConnectionConfig.sshConnectionInfo.password);
            
            ConnectionConfig.sshConnectionInfo.port = 
                sshPortValidator.Validate(ConnectionConfig.sshConnectionInfo.port);
            
            ConnectionConfig.sshConnectionInfo.publicKeyAuth.path = 
                textValidator.Validate(ConnectionConfig.sshConnectionInfo.publicKeyAuth.path);
            ConnectionConfig.sshConnectionInfo.publicKeyAuth.passPhrase = 
                textValidator.Validate(ConnectionConfig.sshConnectionInfo.publicKeyAuth.passPhrase);
        }

        /// <summary>
        /// Names of the locations found in game. Each name's key is the level they're in.
        /// </summary>
        /// <remarks>Readonly since it shouldn't be modified.</remarks>
        public readonly List<string> LevelNameTable = new List<string>
        {
            "Spriggan Ruins", 
            "Spriggan Dungeon - Basement 2", 
            "Spriggan Dungeon - Basement 1", 
            "Spriggan Castle", 
            "Spriggan Castle - Outdoors", 
        };
        
        /// <summary>
        /// Represents the information held by the game configuration file in a text chain.
        /// </summary>
        /// <returns>String containing summary of the game configuration</returns>
        public new static string ToString()
        {
            string prefix = "Current connection settings...\n";
            return (ConnectionConfig == null || !IsValid) ? prefix + "Fix your config files to play" : 
                prefix + ConnectionConfig;
        }
    }
}
