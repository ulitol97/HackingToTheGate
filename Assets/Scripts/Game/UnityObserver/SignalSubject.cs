using System.Collections.Generic;
using UnityEngine;

namespace Game.UnityObserver
{
    /// <summary>
    /// The class Signal acts as an implementation of the observer pattern for Unity, precisely acts as
    /// an observed subject.
    /// </summary>
    [CreateAssetMenu]
    public class SignalSubject : ScriptableObject, IObservedSubject
    {

        private List<IObserver> _observers;
        
        public List<IObserver> Observers
        {
            get { return _observers ?? (_observers = new List<IObserver>()); }
            set { _observers = value; }
        }
        
        public void Attach(IObserver listener)
        {
            if (!Observers.Contains(listener))
                Observers.Add(listener);
        }
    
        public void Detach(IObserver listener)
        {
            if (Observers.Contains(listener))
                Observers.Remove(listener);
        }

        public void Notify()
        {
            foreach (var observer in Observers)
            {
                observer.UpdateObserver();
            }
        }
    }
}
