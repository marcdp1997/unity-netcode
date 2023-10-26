using System;

namespace Arrowfist.Managers
{
    public interface IEventManager
    {
        public void Subscribe(EventIds eventId, Action<EventData> listener);

        public void Unsubscribe(EventIds eventId, Action<EventData> listener);

        public void Publish(EventData eventData);
    }
}
