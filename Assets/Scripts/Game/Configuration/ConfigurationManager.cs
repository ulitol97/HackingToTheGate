using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Game.Configuration.Validation;
using UnityEngine;

namespace Game.Configuration
{
    /// <summary>
    /// The class ConfigurationManager implements the Singleton pattern in order to hold
    /// variables that need to be available during the whole game session. It is capable of accessing a
    /// game configuration file and validate it.
    /// </summary>
    public class ConfigurationManager : UnitySingleton<ConfigurationManager>
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
        
        private Regex _ipValidation = 
            new Regex(@"\b(?:(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])\.){3}(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])\b");

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
        public bool isValid;
        
        /// <summary>
        /// Reference to a ConnectionConfiguration object holding all the variables specified in the user
        /// configuration file.
        /// </summary>
        public ConnectionConfiguration connectionConfig;
        
        /// <summary>
        /// Reference to a ChallengesConfiguration object holding all the variables specified in the user
        /// configuration file.
        /// </summary>
        public ChallengesConfiguration challengesConfig;

        /// <summary>
        /// Checks for the existence of a game configuration file and if it exists proceeds to validate it.
        /// If the file does not exist or can't be read because of incorrect formatting, the configuration in the file
        /// is considered invalid (<see cref="isValid"/>).
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
                    connectionConfig = ConnectionConfiguration.CreateFromJson(File.ReadAllText(connectionPath));
                    challengesConfig = new ChallengesConfiguration
                    {
                        answers = ChallengesConfiguration.RetrieveAnswersFromJson(File.ReadAllText(answersPath)),
                        clues = ChallengesConfiguration.RetrieveCluesFromJson(File.ReadAllText(cluesPath))
                    };
                }
                catch (Exception)
                {
                    isValid = false;
                    return;
                }
                ValidateConfiguration();
            }
            else
            {
                isValid = false;
                connectionConfig = null;
            }
        }

        /// <summary>
        /// Checks that, after validating the users config file, at least a valid remote host IP and a valid
        /// username to attempt authentication are specified.
        /// </summary>
        private void ValidateConfiguration()
        {
            ValidateConnectionFields();
            // Final checks
            if (connectionConfig.vncConnectionInfo.targetHost.Equals(TextValidator.DefaultValue))
            {
                isValid = false;
                return;
            }
            if (connectionConfig.sshConnectionInfo.username.Equals(TextValidator.DefaultValue))
            {
                isValid = false;
                return;
            }

            isValid = true;
        }

        /// <summary>
        /// Validates all the items present in the game configuration file, setting them to a default value if
        /// possible when the user input values are not valid.
        /// </summary>
        private void ValidateConnectionFields()
        {
            // Vnc info validation
            IValidator<string> ipValidator = new TextValidator(_ipValidation);
            IValidator<string> textValidator = new TextValidator();
            IValidator<int> vncPortValidator = 
                new IntegerValidator(MinPortNumberAccepted, MaxPortNumberAccepted, 5900);
            
            connectionConfig.vncConnectionInfo.targetHost = 
                ipValidator.Validate(connectionConfig.vncConnectionInfo.targetHost);
            
            connectionConfig.vncConnectionInfo.port =
                vncPortValidator.Validate(connectionConfig.vncConnectionInfo.port);
            
            connectionConfig.vncConnectionInfo.vncServerPassword =
                textValidator.Validate(connectionConfig.vncConnectionInfo.vncServerPassword);
            
            // Ssh info validation
            
            IValidator<int> sshPortValidator = 
                new IntegerValidator(MinPortNumberAccepted, MaxPortNumberAccepted, 22);
            
            connectionConfig.sshConnectionInfo.username = 
                textValidator.Validate(connectionConfig.sshConnectionInfo.username);
            connectionConfig.sshConnectionInfo.password = 
                textValidator.Validate(connectionConfig.sshConnectionInfo.password);
            
            connectionConfig.sshConnectionInfo.port = 
                sshPortValidator.Validate(connectionConfig.sshConnectionInfo.port);
            
            connectionConfig.sshConnectionInfo.publicKeyAuth.path = 
                textValidator.Validate(connectionConfig.sshConnectionInfo.publicKeyAuth.path);
            
            connectionConfig.sshConnectionInfo.publicKeyAuth.passPhrase = 
                textValidator.Validate(connectionConfig.sshConnectionInfo.publicKeyAuth.passPhrase);
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
        /// Constant containing the name of the menu scene for quicker access.
        /// </summary>
        public const string MenuScene = "TitleMenu";

        /// <summary>
        /// Represents the information held by the game configuration file in a text chain.
        /// </summary>
        /// <returns>String containing summary of the game configuration</returns>
        public new string ToString()
        {
            string prefix = "Current connection settings...\n";
            return (connectionConfig == null || !isValid) ? prefix + "Fix your config files to play" : 
                prefix + connectionConfig;
        }
    }
}
