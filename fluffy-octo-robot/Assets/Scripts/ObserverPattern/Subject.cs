using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObserverPattern
{
    public class Subject
    {

        // List with observers that are waiting for something to happen
        List<IObserver> observers = new();

        // Send notifications if something has happend
        public void Notify()
        {
            for (int i = 0; i < observers.Count; i++)
            {
                // notify all observers -> each observer checks if it is interested in event
                observers[i].OnNotify();
            }
        }

        public void AddObserver(IObserver observer)
        {
            observers.Add(observer);
        }

        public void RemoveObserver(IObserver observer)
        {
            observers.Remove(observer);
        }

    }



}
