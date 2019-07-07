using UnityEngine;

namespace Game.Configuration.Validation
{
    public class IntegerValidator
    {
        private int _defaultValue;
        private int _minValue;
        private int _maxValue;

        public IntegerValidator(int minimum, int maximum)
        {
            _minValue = minimum;
            _maxValue = maximum;
            _defaultValue = -1;
        }
        
        public IntegerValidator(int minimum, int maximum, int defaultValue)
        {
            _minValue = minimum;
            _maxValue = maximum;
            _defaultValue = defaultValue;
        }

        public int Validate(int input)
        {
            if (input < _minValue || input > _maxValue)
                return _defaultValue;

            return input;
        }

    }
}
