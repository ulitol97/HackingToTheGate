
namespace Game.Configuration.Validation
{
    /// <summary>
    /// The IntegerValidator class allows to validate if a given Integer number is in a certain desired range and
    /// to return a specific value in case it is not.
    /// </summary>
    public class IntegerValidator
    {
        /// <summary>
        /// Value to return if the integer validation fails.
        /// </summary>
        private int _defaultValue;
        
        /// <summary>
        /// Expected minimum value of the input integer validated.
        /// </summary>
        private int _minValue;
        
        /// <summary>
        /// Expected maximum value of the input integer validated.
        /// </summary>
        private int _maxValue;

        /// <summary>
        /// Creates a validator and specifies its max. and min. limits.
        /// </summary>
        /// <param name="minimum"><see cref="_minValue"/></param>
        /// <param name="maximum"><see cref="_maxValue"/></param>
        public IntegerValidator(int minimum, int maximum)
        {
            _minValue = minimum;
            _maxValue = maximum;
            _defaultValue = -1;
        }

        /// <summary>
        /// Creates a validator and specifies its max. and min. limits and the value to return if the validation fails.
        /// </summary>
        /// <param name="minimum"><see cref="_minValue"/></param>
        /// <param name="maximum"><see cref="_maxValue"/></param>
        /// <param name="defaultValue"><see cref="_defaultValue"/></param>
        public IntegerValidator(int minimum, int maximum, int defaultValue)
        {
            _minValue = minimum;
            _maxValue = maximum;
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// Validates a given integer input against the conditions specified in the validator object.
        /// </summary>
        /// <param name="input">Input integer to be validated.</param>
        /// <returns>A default value if the validations fails or the original input if the validation
        /// succeeded.</returns>
        public int Validate(int input)
        {
            if (input < _minValue || input > _maxValue)
                return _defaultValue;

            return input;
        }

    }
}
