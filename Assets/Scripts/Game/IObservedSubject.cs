using System.Collections.Generic;

namespace Game
{
    public interface IObservedSubject
    {
        /// <summary>
        /// Represents the list of observers subscribed to the observed subject changes.
        /// </summary>
        List<IObserver> Observers { get; set; }

        /// <summary>
        /// Subscribes a new observer object to the observed subject.
        /// </summary>
        /// <param name="observer">Observer object subscribed.</param>
        void Attach(IObserver observer);
        
        /// <summary>
        /// Un-subscribes a new observer object to the observed subject.
        /// </summary>
        /// <param name="observer">Observer object unsubscribed.</param>
        void Detach(IObserver observer);
       
        /// <summary>
        /// Notifies all observers registered in <see cref="Observers"/> that they should update their status
        /// to match the observed subject.
        /// </summary>
        void Notify();
    }
}