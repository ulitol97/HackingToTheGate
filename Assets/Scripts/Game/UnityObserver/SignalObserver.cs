using UnityEngine;
using UnityEngine.Events;

namespace Game.UnityObserver
{
    /// <summary>
    /// The class SignalObserver watches for changes coming from unity events
    /// and triggers logic operations in consequence.
    /// </summary>
    public class SignalObserver : MonoBehaviour, IObserver
    {
        /// <summary>
        /// Signal observed by the signal observer.
        /// </summary>
        public SignalSubject signal;
        
        /// <summary>
        /// Unity event to be watched.
        /// </summary>
        public UnityEvent signalEvent;
        
        /// <summary>
        /// Invokes the functionality contained in the event the observer is watching.
        /// </summary>
        public void UpdateObserver()
        {
            signalEvent.Invoke();
        }

        /// <summary>
        /// Function called when the Observer is enabled in memory.
        /// Attaches itself to the signal it needs to observe.
        /// </summary>
        private void OnEnable()
        {
            signal.Attach(this);
        }
    
        /// <summary>
        /// Function called when the Observer is erased from memory.
        /// Detaches itself to the signal it needs to observe.
        /// </summary>
        private void OnDisable()
        {
            signal.Detach(this);
        }
    }
}
