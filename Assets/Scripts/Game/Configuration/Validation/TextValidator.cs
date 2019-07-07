using System.Text.RegularExpressions;

namespace Game.Configuration.Validation
{
    /// <summary>
    /// The TextValidator class allows to validate if a given string matches a certain patterns and
    /// to return a specific value in case it is not.
    /// </summary>
    public class TextValidator
    {
        /// <summary>
        /// Value to return if the string validation fails.
        /// </summary>
        public const string DefaultValue = "";
        
        /// <summary>
        /// Pattern to compare the string against.
        /// </summary>
        private Regex _comparePattern;

        /// <summary>
        /// Creates a validator that will only check for non-null input.
        /// </summary>
        public TextValidator()
        {
            // Match everything
            _comparePattern = new Regex("(?s).*");
        }

        /// <summary>
        /// Creates a validator that will check against a specific pattern the input must match.
        /// </summary>
        /// <param name="pattern">Regex containing the pattern to be matched.</param>
        public TextValidator(Regex pattern)
        {
            _comparePattern = pattern;
        }

        /// <summary>
        /// Validates a given string input against the conditions specified in the validator object.
        /// </summary>
        /// <param name="input">Input string to be validated.</param>
        /// <returns>A default value if the validations fails or the original input trimmed if the validation
        /// succeeded.</returns>
        public string Validate(string input)
        {
            if (input == null)
                return DefaultValue;
            
            string cleanInput = input.Trim();
            return _comparePattern.IsMatch(cleanInput) ? cleanInput : DefaultValue;
        }
    }
}
