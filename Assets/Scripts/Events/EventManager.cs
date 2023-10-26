using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arrowfist.Managers
{
    public class EventData
    {
        public readonly EventIds EventId;

        public EventData(EventIds eventId)
        {
            EventId = eventId;
        }
    }

    public class EventManager : MonoBehaviour, IEventManager
    {
        public static IEventManager Instance { get; private set; }

        private Dictionary<EventIds, Action<EventData>> eventDictionary;

        private void Awake()
        {
            Instance = this;

            eventDictionary ??= new Dictionary<EventIds, Action<EventData>>();
        }

        public void Subscribe(EventIds eventId, Action<EventData> listener)
        {
            if (eventDictionary.ContainsKey(eventId))
            {
                eventDictionary[eventId] += listener;
            }
            else
            {
                eventDictionary.Add(eventId, listener);
            }
        }

        public void Unsubscribe(EventIds eventId, Action<EventData> listener)
        {
            if (eventDictionary.ContainsKey(eventId))
            {
                eventDictionary[eventId] -= listener;
            }  
        }

        public void Publish(EventData eventData)
        {
            if (eventDictionary.TryGetValue(eventData.EventId, out Action<EventData> thisEvent))
            {
                thisEvent?.Invoke(eventData);
            }
        }
    }
}

