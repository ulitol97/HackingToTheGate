using UnityEngine;

/// <summary>
/// Base class to be inherited in order to create singletons that perform with Unity MonoBehaviour.
/// It allows singletons to exists between game scenes.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Check to see if we're about to be destroyed.
    private static bool _mShuttingDown;

    // Hold the current and only simultaneous instance of the GameObject in use by the game.
    private static T _mInstance;
    
    private static object _mLock = new object();
 
    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static T Instance
    {
        get { return GetInstance(false); }
    }

    public static T GetInstance(bool ignoreShuttingDown)
    {
        if (_mShuttingDown )
        {
            return ignoreShuttingDown ? _mInstance : null;
        }
        
        lock (_mLock)
        {
            if (_mInstance == null)
            {
                // Search for existing instance.
                _mInstance = (T)FindObjectOfType(typeof(T));
 
                // Create new instance if one doesn't already exist.
                if (_mInstance == null)
                {
                    // Need to create a new GameObject to attach the singleton to.
                    var singletonObject = new GameObject();
                    _mInstance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T) + " (Singleton)";
 
                    // Make instance persistent.
                    DontDestroyOnLoad(singletonObject);
                }
            }
 
            return _mInstance;
        }
    }

//    static T GetInstance(bool ignoreShuttingDown)
//    {
//        if (!ignoreShuttingDown)
//            return Instance;
//        else
//        {
//            
//        }
//
//    }

    /// <summary>
    /// On the event of having the GameObject set for destruction in-game,
    /// mark it to prevent its access from external code.
    /// </summary>
    private void OnDestroy()
    {
        _mShuttingDown = true;
    }
 
    /// <summary>
    /// On the event of having the GameObject set for destruction due to the game closing,
    /// mark it to prevent its access from external code.
    /// </summary>
    private void OnApplicationQuit()
    {
        _mShuttingDown = true;
    }
}