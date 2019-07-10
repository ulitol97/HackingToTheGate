namespace Game.Configuration.Validation
{
    /// <summary>
    /// The IValidator interface serves as a contract to all the operations validator must implement.
    /// It uses generics to ensure all type of validator can be implemented regardless of the type of the value
    /// to validate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValidator <T>
    {
        T Validate(T input);
    }
}