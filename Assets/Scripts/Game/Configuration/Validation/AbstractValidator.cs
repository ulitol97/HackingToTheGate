namespace Game.Configuration.Validation
{
    public abstract class AbstractValidator<T> : IValidator<T>
    {
        public static T DefaultValue { get; protected set; }

        /// <summary>
        /// Validates a given input.
        /// </summary>
        /// <param name="input">Input to validate</param>
        /// <returns></returns>
        public virtual T Validate(T input)
        {
            return default(T);
        }
    }
}