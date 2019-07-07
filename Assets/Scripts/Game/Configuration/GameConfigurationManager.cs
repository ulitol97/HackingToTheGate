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
    /// variables that need to be available during the whole game session.
    /// </summary>
    public class GameConfigurationManager : Singleton<GameConfigurationManager>
    {
        /// <summary>
        /// Name of the file containing the game configuration.
        /// </summary>
        private const string ConfigFileName = "config.json";
        
        public Regex ipValidation = new Regex(@"\b(?:(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])\.){3}(?:[0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])\b");

        public int minPortNumberAccepted = 1;
        public int maxPortNumberAccepted = 65535;

        /// <summary>
        /// Boolean flag representing if the game current configuration is valid to start a new game or not.
        /// </summary>
        public static bool IsValid;


        public static GameConfiguration GameConfig;

        public void LoadGameConfiguration()
        {
            string configFilePath = Path.Combine(Application.streamingAssetsPath, ConfigFileName);
            if (File.Exists(configFilePath))
            {
                try
                {
                    GameConfig = GameConfiguration.CreateFromJson(File.ReadAllText(configFilePath));
                }
                catch (Exception)
                {
                    IsValid = false;
                    return;
                }
                Validate();
            }
            else
            {
                IsValid = false;
                GameConfig = null;
            }
        }

        /// <summary>
        /// Checks that after validating the users config file, at least a valid remote host IP and a valid
        /// username to attempt authentication are specified.
        /// </summary>
        private void Validate()
        {
            ValidateConfigFields();
            // Final checks

            if (GameConfig.vncConnectionInfo.targetHost.Equals(TextValidator.DefaultValue))
            {
                IsValid = false;
                return;
            }
            if (GameConfig.sshConnectionInfo.username.Equals(TextValidator.DefaultValue))
            {
                IsValid = false;
                return;
            }

            IsValid = true;

            // Pr cada pista en el array, incluir su validacion en una lista de pistas. Hacer a los carteles leer
            // su texto de la lista de pista.

            // Hacer algo similar con el nombre de cada piso de la mazmorra.
        }

        /// <summary>
        /// Validates all the items present in the game configuration file, setting them to a default value if any.
        /// </summary>
        private void ValidateConfigFields()
        {
            TextValidator ipValidator = new TextValidator(ipValidation);
            TextValidator textValidator = new TextValidator();
            IntegerValidator sshPortValidator = new IntegerValidator(minPortNumberAccepted, maxPortNumberAccepted, 22);
            IntegerValidator vncPortValidator = new IntegerValidator(minPortNumberAccepted, maxPortNumberAccepted, 5900);
            
            // Vnc info validation
            GameConfig.vncConnectionInfo.targetHost = 
                ipValidator.Validate(GameConfig.vncConnectionInfo.targetHost);
            
            GameConfig.vncConnectionInfo.port =
                vncPortValidator.Validate(GameConfig.vncConnectionInfo.port);
            
            GameConfig.vncConnectionInfo.vncServerPassword =
                textValidator.Validate(GameConfig.vncConnectionInfo.vncServerPassword);
            
            // Ssh info validation
            
            GameConfig.sshConnectionInfo.username = textValidator.Validate(GameConfig.sshConnectionInfo.username);
            GameConfig.sshConnectionInfo.password = textValidator.Validate(GameConfig.sshConnectionInfo.password);
            
            GameConfig.sshConnectionInfo.port = sshPortValidator.Validate(GameConfig.sshConnectionInfo.port);
            
            GameConfig.sshConnectionInfo.publicKeyAuth.path = 
                textValidator.Validate(GameConfig.sshConnectionInfo.publicKeyAuth.path);
            GameConfig.sshConnectionInfo.publicKeyAuth.passPhrase = 
                textValidator.Validate(GameConfig.sshConnectionInfo.publicKeyAuth.passPhrase);
        }

        /// <summary>
        /// Names of the locations found in game. Each name's key is the level they're in.
        /// </summary>
        /// <remarks>Readonly since it shouldn't be modified.</remarks>
        public readonly Dictionary<string, string> LevelNameTable = new Dictionary<string, string>
        {
            {"Level0", "Nayru Ruins"}, 
            {"Level1", "Nayru Dungeon - Basement 2"}, 
            {"Level2", "Nayru Dungeon - Basement 1"}, 
            {"Level3", "Nayru Castle"}, 
            {"Level4", "Nayru Castle - Overworld"}, 
        };

        /// <summary>
        /// Tips given by the different dialogs and signs the player may find.
        /// </summary>
        public readonly Dictionary<string, string> SignMessagesTable = new Dictionary<string, string>
        {
            {"placeholder", "\"Player\": Nothing to read here..."},
            {"warning", "DO NOT DISTURB THE BASEMENT WORKERS!"},
            {"tip0", "Nice terminal! You better know  the IP of the network camera 02 of" +
                         " the Medicine faculty if you want to use it ;)"
            },
            {"tip1", "If you can't figure out something, maybe the answer is not " +
                     "in this dungeon but elsewhere..."
            },
            {"tip2", "Mauricio hash cat on shadowfile entry."},
            {"tip3", "Did you get any cool secret nformation form mauricio?"}
        };

        public new static string ToString()
        {
            string prefix = "Current connection settings...\n";
            return (GameConfig == null || !IsValid) ? prefix + "Fix your config file to play" : 
                prefix + GameConfig;
        }
    }
}
