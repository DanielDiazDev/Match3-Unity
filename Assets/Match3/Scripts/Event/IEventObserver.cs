using Match3.Scripts.Event.Data;

namespace Match3.Scripts.Event
{
    public interface IEventObserver
    {
        void Process(EventData eventData);
    }
}