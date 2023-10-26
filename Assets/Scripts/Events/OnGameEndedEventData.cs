
namespace Arrowfist.Managers
{
    public class OnGameEndedEventData : EventData
    {
        public readonly bool Win;

        public OnGameEndedEventData(bool win) : base(EventIds.OnGameEnded)
        {
            Win = win;
        }
    }
}


