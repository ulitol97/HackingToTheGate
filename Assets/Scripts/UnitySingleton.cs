using UnityEngine;

/// <summary>
/// Base class to be inherited in order to create singletons that perform with Unity MonoBehaviour.
/// It allows singletons to exists between game scenes.
/// </summary>
public class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// Boolean flag marking if the instance of the singleton class has been marked for removal by Unity and thus
    /// should not be accessed.
    /// </summary>
    private static bool _shuttingDown;
    
    /// <summary>
    /// Hold the current and only simultaneous instance of the GameObject in use by the game.
    /// </summary>
    private static T _instance;
    
    /// <summary>
    /// Dummy object to block the specific code in charge of accessing the instance
    /// </summary>
    private static object _lock = new object();
 
    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static T Instance
    {
        get { return GetInstance(false); }
        set { _instance = value; }
    }

    public static T GetInstance(bool ignoreShuttingDown)
    {
        if (_shuttingDown )
        {
            return ignoreShuttingDown ? _instance : null;
        }
        
        lock (_lock)
        {
            if (_instance == null)
            {
                // Search for existing instance.
                _instance = (T)FindObjectOfType(typeof(T));
 
                // Create new instance if one doesn't already exist.
                if (_instance == null)
                {
                    // Need to create a new GameObject to attach the singleton to.
                    var singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T) + " (Singleton)";
 
                    // Make instance persistent.
                    DontDestroyOnLoad(singletonObject);
                }
            }
 
            return _instance;
        }
    }

    /// <summary>
    /// On the event of having the GameObject set for destruction in-game,
    /// mark it to prevent its access from external code.
    /// </summary>
    private void OnDestroy()
    {
        _shuttingDown = true;
    }
 
    /// <summary>
    /// On the event of having the GameObject set for destruction due to the game closing,
    /// mark it to prevent its access from external code.
    /// </summary>
    private void OnApplicationQuit()
    {
        _shuttingDown = true;
    }
}