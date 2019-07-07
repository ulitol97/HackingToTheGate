using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.Configuration.Validation
{
    public class TextValidator
    {
        public const string DefaultValue = "";
        private Regex _comparePattern;

        public TextValidator()
        {
            // Match everything
            _comparePattern = new Regex("(?s).*");
        }

        public TextValidator(Regex pattern)
        {
            _comparePattern = pattern;
        }

        public string Validate(string input)
        {
            if (input == null)
                return DefaultValue;
            
            string cleanInput = input.Trim();
            return _comparePattern.IsMatch(cleanInput) ? cleanInput : DefaultValue;
        }
    }
}
